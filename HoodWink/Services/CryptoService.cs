/*
Encrypt Shellcode with AES and Base64 encoded
  - Reference: https://stackoverflow.com/questions/53653510/c-sharp-aes-encryption-byte-array

Note: 
  Using StreamReader in the Crypto functions can cause some bytes to get mangled (change)
     So I dont do that here, see note below next to the shellcode
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HoodWink.Services
{
    public static class CryptoService
    {
        public static string Encrypt(byte[] data, out string keyBase64, out string ivBase64)
        {
            using (Aes aes = Aes.Create())
            {
                // Set Key and IV
                keyBase64 = Convert.ToBase64String(aes.Key);
                ivBase64 = Convert.ToBase64String(aes.IV);

                //Console.WriteLine($"~ Aes Cipher Mode : {aes.Mode}");
                //Console.WriteLine($"~ Aes Padding Mode: {aes.Padding}");
                //Console.WriteLine($"~ Aes Key Size :    {aes.KeySize}");
                //Console.WriteLine($"~ Aes Block Size :  {aes.BlockSize}");
                //Console.WriteLine($"~ Aes IV Length :   {aes.IV.Length}");                
                //Console.WriteLine();

                // Create encrypter and run
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    return Convert.ToBase64String(DoCrypto(data, encryptor));
                }
            }
        }

        public static byte[] Decrypt(string dataBase64, string keyBase64, string ivBase64)
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
        public static byte[] DoCrypto(byte[] data, ICryptoTransform crypter)
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

        public static string Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public static string Decode(string data)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(data));
        }

        public static bool Equals(ref byte[] e, ref byte[] d)
        {
            if (e.Length != d.Length)
            {
                Console.WriteLine("Lengths do not match");
                return false;
            }

            for (int i = 0; i < e.Length; i++)
            {
                if (e[i] != d[i])
                {
                    Console.WriteLine("Error at index = " + i + " >> " + e[i] + " != " + d[i]);
                    return false;
                }
            }

            return true;
        }
    }
}