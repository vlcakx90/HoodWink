using System.Collections.Generic;

namespace HoodWink.Models.Base
{
    public abstract class Technique
    {
        public abstract string Description { get; }
        public abstract List<string> FileDependencies { get; }
        public abstract string Using { get; }
        public abstract string ApiImports { get; }
        public abstract string MainLogic { get; }
        public abstract string AdditionalFunctions { get; }
    }
}