using System;
using System.Collections.Generic;

namespace HoodWink.Languages.Cpp.Formats
{
    public class Exe : Models.Base.FormatExe
    {
        public override string Description => "Default Format for Exe";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"//Format Inclue";

        public override string NamespaceAndClassHeader => @"";
        public override string MainHeader => @"int main(int argc, const char* argv[])
{";

        public override string MainBody => @"//Format MainBody";

        public override string MainFooter => @"     return 0;
}";

        public override string NamespaceAndClassFooter => @"// Format Footer";
    }
}