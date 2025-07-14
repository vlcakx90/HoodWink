using System.Collections.Generic;

namespace HoodWink.Models.Base
{
    public abstract class FormatExe
    {
        public abstract string Description { get; }
        public abstract List<string> FileDependencies { get; }
        public abstract string Using { get; }
        public abstract string NamespaceAndClassHeader { get; }
        public abstract string MainHeader { get; }
        public abstract string MainBody { get; }
        public abstract string MainFooter { get; }
        public abstract string NamespaceAndClassFooter { get; }
    }
}