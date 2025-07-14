using System.Collections.Generic;

namespace HoodWink.Models.Base
{
    public abstract class Compiler
    {
        public abstract string Description { get; }
        public abstract string CompilerPath { get; }

        public abstract string Compile(string sourcePath, List<string> dependencies);
    }
}