using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Extras
{
    public class AmsiBypass : Models.Base.Extras
    {
        public override string Description => "Not implemented yet :(";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"//Amsi Using";

        public override string MainLogic => @"//Amsi MainLogic";

        public override string AdditionalFunctions => @"//Amsi Additional Functions";
    }
}