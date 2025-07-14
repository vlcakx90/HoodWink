using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Formats
{
    public class Exe : Models.Base.FormatExe
    {
        public override string Description => "Default Format for Exe";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"//Format Using";

        public override string NamespaceAndClassHeader => @"namespace Wink
{
    public class Program
    {";
        public override string MainHeader => @"static void Main(string[] args)
        {";

        public override string MainBody => @"//Format MainBody";

        public override string MainFooter => @"        }";

        public override string NamespaceAndClassFooter => @"   }
}";
    }
}