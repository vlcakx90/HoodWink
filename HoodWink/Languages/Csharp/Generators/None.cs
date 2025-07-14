using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Protections
{
    public class None : Models.Base.Protections
    {
        public override string Description => "No Encryption (just base64)";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"";

        public override string MainLogic => @"byte[] payload = Decode(b64);";

        public override string AdditionalFunctions => @"public static byte[] Decode(string dataBase64)
        {
            return Convert.FromBase64String(dataBase64);
        }";
    }
}