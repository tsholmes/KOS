using System;
using System.Collections.Generic;
using System.Linq;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Serialization;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("UniqueSet")]
    public class UniqueSetValue : CollectionValue<HashSet<Structure>>
    {
        private static readonly SuffixMap suffixes;

        static UniqueSetValue()
        {
            suffixes = CollectionSuffixes<UniqueSetValue>();

            suffixes.AddSuffix("COPY", new NoArgsSuffix<UniqueSetValue, UniqueSetValue>((uniqueSet) => uniqueSet.Copy));
            suffixes.AddSuffix("ADD", new OneArgsVoidSuffix<UniqueSetValue, Structure>((uniqueSet) => uniqueSet.Add));
            suffixes.AddSuffix("REMOVE", new OneArgsSuffix<UniqueSetValue, BooleanValue, Structure>((uniqueSet) => (item) => uniqueSet.Remove(item)));
        }

        public UniqueSetValue()
            : this(new HashSet<Structure>())
        {
        }

        public UniqueSetValue(IEnumerable<Structure> setValue) : base("UNIQUESET", new HashSet<Structure>(setValue), suffixes)
        {
        }

        public void Add(Structure item)
        {
            Collection.Add(item);
        }

        public UniqueSetValue Copy()
        {
            return new UniqueSetValue(this);
        }

        public void CopyTo(Structure[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(Structure item)
        {
            return Collection.Remove(item);
        }

        public void Clear()
        {
            Collection.Clear();
        }

        public override void LoadDump(Dump dump)
        {
            Collection.Clear();

            List<object> values = (List<object>)dump[kOS.Safe.Dump.Items];

            foreach (object item in values)
            {
                Collection.Add((Structure)FromPrimitive(item));
            }
        }
    }
}