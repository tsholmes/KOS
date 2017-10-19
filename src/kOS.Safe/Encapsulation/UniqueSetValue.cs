using System;
using System.Collections.Generic;
using System.Linq;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Serialization;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("UniqueSet")]
    public class UniqueSetValue<T> : CollectionValue<T, HashSet<T>>
        where T : Structure
    {
        public UniqueSetValue()
            : this(new HashSet<T>())
        {
        }

        public UniqueSetValue(IEnumerable<T> setValue) : base("UNIQUESET", new HashSet<T>(setValue))
        {
            SetInitializeSuffixes();
        }

        public void Add(T item)
        {
            Collection.Add(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
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
                Collection.Add((T)FromPrimitive(item));
            }
        }

        private void SetInitializeSuffixes()
        {
            AddSuffix("COPY",     new NoArgsSuffix<UniqueSetValue<T>>         (() => new UniqueSetValue<T>(this)));
            AddSuffix("ADD",      new OneArgsSuffix<T>                      (toAdd => Collection.Add(toAdd)));
            AddSuffix("REMOVE",   new OneArgsSuffix<BooleanValue, T>        (toRemove => Collection.Remove(toRemove)));
       }
    }

    [kOS.Safe.Utilities.KOSNomenclature("UniqueSet", KOSToCSharp = false)] // one-way because the generic templated UniqueSetValue<T> is the canonical one.
    public class UniqueSetValue : UniqueSetValue<Structure>
    {
        public UniqueSetValue()
        {
            InitializeSuffixes();
        }

        public UniqueSetValue(IEnumerable<Structure> toCopy)
            : base(toCopy)
        {
            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("COPY", new NoArgsSuffix<UniqueSetValue>(() => new UniqueSetValue(this)));
        }
    }
}