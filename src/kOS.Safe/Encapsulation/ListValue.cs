using System;
using System.Collections.Generic;
using System.Linq;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Properties;
using kOS.Safe.Serialization;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("List")]
    public class ListValue : CollectionValue<IList<Structure>>, IIndexable
    {
        private static readonly SuffixMap suffixes;

        static ListValue()
        {
            suffixes = CollectionSuffixes<ListValue>();

            suffixes.AddSuffix("COPY", new NoArgsSuffix<ListValue, ListValue>((list) => list.Copy));
            suffixes.AddSuffix("ADD", new OneArgsVoidSuffix<ListValue, Structure>((list) => list.Add, Resources.ListAddDescription));
            suffixes.AddSuffix("INSERT", new TwoArgsVoidSuffix<ListValue, ScalarValue, Structure>((list) => list.Insert));
            suffixes.AddSuffix("REMOVE", new OneArgsVoidSuffix<ListValue, ScalarValue>((list) => (toRemove) => list.RemoveAt(toRemove)));
            suffixes.AddSuffix("SUBLIST", new TwoArgsSuffix<ListValue, ListValue, ScalarValue, ScalarValue>((list) => list.SubListMethod));
            suffixes.AddSuffix("JOIN", new OneArgsSuffix<ListValue, StringValue, StringValue>((list) => list.Join));
        }

        public ListValue()
            : this(new List<Structure>())
        {
        }

        public ListValue(IEnumerable<Structure> listValue) : base("LIST", new List<Structure>(listValue), suffixes)
        {
        }

        public void Add(Structure item)
        {
            Collection.Add(item);
        }

        public void Insert(ScalarValue index, Structure item)
        {
            Collection.Insert(index, item);
        }

        public ListValue Copy()
        {
            return new ListValue(this);
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

        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
        }

        public Structure this[int index]
        {
            get { return Collection[index]; }
            set { Collection[index] = value; }
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

        // This test case was added to ensure there was an example method with more than 1 argument.
        private ListValue SubListMethod(ScalarValue start, ScalarValue runLength)
        {
            var subList = new ListValue();
            for (int i = start; i < Collection.Count && i < start + runLength; ++i)
            {
                subList.Add(Collection[i]);
            }
            return subList;
        }

        public static ListValue CreateList<TU>(IEnumerable<TU> list)
        {
            return new ListValue(list.Cast<Structure>());
        }

        public Structure GetIndex(int index)
        {
            return Collection[index];
        }

        public Structure GetIndex(Structure index)
        {
            if (index is ScalarValue)
            {
                int i = Convert.ToInt32(index);  // allow expressions like (1.0) to be indexes
                return GetIndex(i);
            }
            // Throw cast exception with ScalarIntValue, instead of just any ScalarValue
            throw new KOSCastException(index.GetType(), typeof(ScalarIntValue));
        }

        public void SetIndex(Structure index, Structure value)
        {
            int idx;
            try
            {
                idx = Convert.ToInt32(index);
            }
            catch
            {
                throw new KOSException("The index must be an integer number");
            }
            Collection[idx] = (Structure)value;
        }

        public void SetIndex(int index, Structure value)
        {
            Collection[index] = value;
        }

        private StringValue Join(StringValue separator)
        {
            return string.Join(separator, Collection.Select(i => i.ToString()).ToArray());
        }
    }
}



