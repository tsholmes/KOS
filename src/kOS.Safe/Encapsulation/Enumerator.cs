using System.Collections;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Iterator")]
    [kOS.Safe.Utilities.KOSNomenclature("Enumerator", CSharpToKOS = false)] // one-way mapping makes it just another alias, not canonical.
    public class Enumerator : Structure
    {
        private static readonly SuffixMap suffixes;

        static Enumerator()
        {
            suffixes = StructureSuffixes<Enumerator>();

            suffixes.AddSuffix("NEXT", new NoArgsSuffix<Enumerator, BooleanValue>((enumerator) => enumerator.Next));
            suffixes.AddSuffix("ATEND", new NoArgsSuffix<Enumerator, BooleanValue>((enumerator) => () => !enumerator.status));
            suffixes.AddSuffix("INDEX", new NoArgsSuffix<Enumerator, ScalarValue>((enumerator) => () => enumerator.index));
            suffixes.AddSuffix("VALUE", new NoArgsSuffix<Enumerator, Structure>((enumerator) => () => FromPrimitiveWithAssert(enumerator.enumerator.Current)));
        }

        private readonly IEnumerator enumerator;
        private int index = -1;
        private bool status;

        public Enumerator(IEnumerator enumerator) : base(suffixes)
        {
            this.enumerator = enumerator;
        }

        public BooleanValue Next()
        {
            status = enumerator.MoveNext();
            index++;
            return status;
        }

        public override string ToString()
        {
            return string.Format("{0} Iterator", base.ToString());
        }
    }
}