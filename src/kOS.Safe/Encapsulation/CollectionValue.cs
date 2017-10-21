using System;
using kOS.Safe.Encapsulation;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Serialization;

namespace kOS.Safe
{
    [kOS.Safe.Utilities.KOSNomenclature("Collection")]
    public abstract class CollectionValue<C> : EnumerableValue<C> where C : ICollection<Structure>
    {
        protected static SuffixMap CollectionSuffixes<T>() where T : CollectionValue<C>
        {
            SuffixMap suffixes = EnumerableSuffixes<T>();

            suffixes.AddSuffix("CLEAR", new NoArgsVoidSuffix<T>((collection) => collection.Collection.Clear));

            return suffixes;
        }

        protected readonly C Collection;

        public CollectionValue(string label, C collection, SuffixMap suffixes) : base(label, collection, suffixes)
        {
            this.Collection = collection;
        }
    }
}

