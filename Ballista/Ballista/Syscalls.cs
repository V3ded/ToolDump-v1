using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

using static Ballista.Native;
using static Ballista.Releases;

namespace Ballista
{
    //Syscall inspiration from hxxps://jhalon.github.io/utilizing-syscalls-in-csharp-1/
    class Syscalls
    {
        private static byte[] bSyscallSkeleton =
        {
            0x4C, 0x8B, 0xD1,                   // mov r10, rcx
            0xB8, 0xff, 0x00, 0x00, 0x00,       // mov eax, 0xff (0xff is a placeholder byte at index 4 for other syscalls)
            0x0F, 0x05,                         // syscall
            0xC3                                // ret
        };

        public struct Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate NTSTATUS NtOpenProcess(ref IntPtr ProcessHandle, UInt32 AccessMask, ref OBJECT_ATTRIBUTES ObjectAttributes, ref CLIENT_ID ClientId);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate NTSTATUS NtCreateThreadEx(ref IntPtr threadHandle, UInt32 desiredAccess, IntPtr objectAttributes, IntPtr processHandle, IntPtr startAddress, IntPtr parameter, bool inCreateSuspended, Int32 stackZeroBits, Int32 sizeOfStack, Int32 maximumStackSize, IntPtr attributeList);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate NTSTATUS NtCreateSection(ref IntPtr SectionHandle, UInt32 DesiredAccess, IntPtr ObjectAttributes, ref UInt32 MaximumSize, UInt32 SectionPageProtection, UInt32 AllocationAttributes, IntPtr FileHandle);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate NTSTATUS NtMapViewOfSection(IntPtr SectionHandle, IntPtr ProcessHandle, ref IntPtr BaseAddress, UIntPtr ZeroBits, UIntPtr CommitSize, out ulong SectionOffset, out uint ViewSize, uint InheritDisposition, uint AllocationType, uint Win32Protect);
        }

        public static NTSTATUS NtOpenProcess(int release, ref IntPtr ProcessHandle, UInt32 AccessMask, ref OBJECT_ATTRIBUTES ObjectAttributes, ref CLIENT_ID ClientId)
        {
            bSyscallSkeleton[4] = syscallOffsets[release]["NtOpenProcess"]; //NtOpenProcess
            byte[] syscall = bSyscallSkeleton;

            unsafe
            {
                fixed (byte* ptr = syscall)
                {
                    IntPtr memoryAddress = (IntPtr)ptr;

                    if (!VirtualProtect(memoryAddress, (UIntPtr)syscall.Length, PAGE_EXECUTE_READWRITE, out uint lpflOldProtect))
                    {
                        throw new Win32Exception();
                    }

                    Delegates.NtOpenProcess fp = (Delegates.NtOpenProcess)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.NtOpenProcess));

                    return (NTSTATUS)fp(ref ProcessHandle, AccessMask, ref ObjectAttributes, ref ClientId);
                }
            }
        }
        public static NTSTATUS NtCreateThreadEx(int release, ref IntPtr threadHandle, UInt32 desiredAccess, IntPtr objectAttributes, IntPtr processHandle, IntPtr startAddress, IntPtr parameter, bool inCreateSuspended, Int32 stackZeroBits, Int32 sizeOfStack, Int32 maximumStackSize, IntPtr attributeList)
        {
            bSyscallSkeleton[4] = syscallOffsets[release]["NtCreateThreadEx"]; //NtCreateThreadEx
            byte[] syscall = bSyscallSkeleton;

            unsafe
            {
                fixed (byte* ptr = syscall)
                {
                    IntPtr memoryAddress = (IntPtr)ptr;

                    if (!VirtualProtect(memoryAddress, (UIntPtr)syscall.Length, PAGE_EXECUTE_READWRITE, out uint lpflOldProtect))
                    {
                        throw new Win32Exception();
                    }

                    Delegates.NtCreateThreadEx fp = (Delegates.NtCreateThreadEx)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.NtCreateThreadEx));

                    return (NTSTATUS)fp(ref threadHandle, desiredAccess, objectAttributes, processHandle, startAddress, parameter, inCreateSuspended, stackZeroBits, sizeOfStack, maximumStackSize, attributeList);
                }
            }
        }
        public static NTSTATUS NtCreateSection(int release, ref IntPtr SectionHandle, UInt32 DesiredAccess, IntPtr ObjectAttributes, ref UInt32 MaximumSize, UInt32 SectionPageProtection, UInt32 AllocationAttributes, IntPtr FileHandle)
        {
            bSyscallSkeleton[4] = syscallOffsets[release]["NtCreateSection"];
            byte[] syscall = bSyscallSkeleton;

            unsafe
            {
                fixed (byte* ptr = syscall)
                {
                    IntPtr memoryAddress = (IntPtr)ptr;

                    if (!VirtualProtect(memoryAddress, (UIntPtr)syscall.Length, PAGE_EXECUTE_READWRITE, out uint lpflOldProtect))
                    {
                        throw new Win32Exception();
                    }

                    Delegates.NtCreateSection fp = (Delegates.NtCreateSection)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.NtCreateSection));

                    return (NTSTATUS)fp(ref SectionHandle, DesiredAccess, ObjectAttributes, ref MaximumSize, SectionPageProtection, AllocationAttributes, FileHandle);
                }
            }
        }
        public static NTSTATUS NtMapViewOfSection(int release, IntPtr SectionHandle, IntPtr ProcessHandle, ref IntPtr BaseAddress, UIntPtr ZeroBits, UIntPtr CommitSize, out ulong SectionOffset, out uint ViewSize, uint InheritDisposition, uint AllocationType, uint Win32Protect)
        {
            bSyscallSkeleton[4] = syscallOffsets[release]["NtMapViewOfSection"];
            byte[] syscall = bSyscallSkeleton;

            unsafe
            {
                fixed (byte* ptr = syscall)
                {
                    IntPtr memoryAddress = (IntPtr)ptr;

                    if (!VirtualProtect(memoryAddress, (UIntPtr)syscall.Length, PAGE_EXECUTE_READWRITE, out uint lpflOldProtect))
                    {
                        throw new Win32Exception();
                    }

                    Delegates.NtMapViewOfSection fp = (Delegates.NtMapViewOfSection)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.NtMapViewOfSection));

                    return (NTSTATUS)fp(SectionHandle, ProcessHandle, ref BaseAddress, ZeroBits, CommitSize, out SectionOffset, out ViewSize, InheritDisposition, AllocationType, Win32Protect);
                }
            }
        }
    }
}
