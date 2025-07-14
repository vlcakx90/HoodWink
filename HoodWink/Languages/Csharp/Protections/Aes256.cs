using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Protections
{
    public class Aes256 : Models.Base.Protections
    {
        public override string Description => "AES 256 in CBC mode";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"using System.IO;
using System.Text;
using System.Security.Cryptography;";

        public override string MainLogic => @"byte[] payload = Decrypt(b64, key, iv);";

        public override string AdditionalFunctions => @"public static byte[] Decrypt(string dataBase64, string keyBase64, string ivBase64)
        {
            using (Aes aes = Aes.Create())
            {
                // Decode data
                byte[] data = Convert.FromBase64String(dataBase64);

                // Set Key and IV
                aes.Key = Convert.FromBase64String(keyBase64);
                aes.IV = Convert.FromBase64String(ivBase64);

                // Console.WriteLine($""Aes Cipher Mode : {aes.Mode}"");
                // Console.WriteLine($""Aes Padding Mode: {aes.Padding}"");
                // Console.WriteLine($""Aes Key Size :    {aes.KeySize}"");
                // Console.WriteLine($""Aes Block Size :  {aes.BlockSize}"");
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
        }";
    }
}