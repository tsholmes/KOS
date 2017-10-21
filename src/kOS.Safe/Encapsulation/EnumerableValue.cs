using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Enumerable")]
    public abstract class EnumerableValue<TE> : SerializableStructure, IEnumerable<Structure>
        where TE : IEnumerable<Structure>
    {
        protected static SuffixMap EnumerableSuffixes<T>() where T : EnumerableValue<TE>
        {
            SuffixMap suffixes = StructureSuffixes<T>();

            suffixes.AddSuffix("ITERATOR", new NoArgsSuffix<T, Enumerator>((enumerable) => () => new Enumerator(enumerable.InnerEnumerable.GetEnumerator())));
            suffixes.AddSuffix("REVERSEITERATOR", new NoArgsSuffix<T, Enumerator>((enumerable) => () => new Enumerator(Enumerable.Reverse(enumerable.InnerEnumerable).GetEnumerator())));
            suffixes.AddSuffix("LENGTH", new NoArgsSuffix<T, ScalarValue>((enumerable) => () => enumerable.InnerEnumerable.Count()));
            suffixes.AddSuffix("CONTAINS", new OneArgsSuffix<T, BooleanValue, Structure>((enumerable) => (n) => enumerable.Contains(n)));
            suffixes.AddSuffix("EMPTY", new NoArgsSuffix<T, BooleanValue>((enumerable) => () => !enumerable.InnerEnumerable.Any()));
            suffixes.AddSuffix("DUMP", new NoArgsSuffix<T, StringValue>((enumerable) => () => new StringValue(enumerable.ToString())));

            return suffixes;
        }

        protected TE InnerEnumerable { get; private set; }
        private readonly string label;

        protected EnumerableValue(string label, TE enumerable, SuffixMap suffixes) : base(suffixes)
        {
            this.label = label;
            InnerEnumerable = enumerable;
        }

        public virtual IEnumerator<Structure> GetEnumerator()
        {
            return InnerEnumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(Structure item)
        {
            return InnerEnumerable.Contains(item);
        }

        public int Count()
        {
            return InnerEnumerable.Count();
        }

        public override string ToString()
        {
            return new SafeSerializationMgr(null).ToString(this);
        }

        public override Dump Dump()
        {
            var result = new DumpWithHeader
            {
                Header = label + " of " + InnerEnumerable.Count() + " items:"
            };

            result.Add(kOS.Safe.Dump.Items, InnerEnumerable.Cast<object>().ToList());

            return result;
        }
    }
}