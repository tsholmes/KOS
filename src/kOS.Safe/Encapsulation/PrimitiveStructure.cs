using System;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Structure", KOSToCSharp = false)]
    public abstract class PrimitiveStructure : Structure
    {
        protected PrimitiveStructure(SuffixMap suffixes) : base(suffixes)
        {
        }

        public abstract object ToPrimitive();
    }
}

