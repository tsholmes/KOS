using System;
using System.Linq;
using System.Collections.Generic;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;

namespace kOS.Safe.Encapsulation
{
    public class SuffixMap : ISuffixMap
    {
        private readonly IDictionary<string, ISuffix> suffixes = new Dictionary<string, ISuffix>(StringComparer.OrdinalIgnoreCase);

        public ISuffixResult GetSuffix(string suffixName, Structure structure)
        {
            ISuffix suffix;
            if (!suffixes.TryGetValue(suffixName, out suffix))
            {
                throw new KOSSuffixUseException("get", suffixName, structure);
            }
            return suffix.Get(structure);
        }

        public bool SetSuffix(string suffixName, Structure structure, object value)
        {
            ISuffix suffix;
            if (!suffixes.TryGetValue(suffixName, out suffix))
            {
                return false;
            }
            var settable = suffix as ISetSuffix;
            if (settable != null)
            {
                settable.Set(structure, value);
                return true;
            }
            throw new KOSSuffixUseException("set", suffixName, this);
        }

        public bool HasSuffix(string suffixName)
        {
            return suffixes.ContainsKey(suffixName);
        }

        public ListValue GetSuffixNames()
        {
            List<StringValue> names = new List<StringValue>();            

            names.AddRange(suffixes.Keys.Select(item => (StringValue)item));

            // Return the list alphabetized by suffix name.  The key lookups above, since they're coming
            // from a hashed dictionary, won't be in any predictable ordering:
            return new ListValue(names.Cast<Structure>().OrderBy(item => item.ToString()));
        }

        public void AddSuffix(string suffixName, ISuffix suffix)
        {
            AddSuffix(new string[]{ suffixName }, suffix);
        }

        public void AddSuffix(IEnumerable<string> suffixNames, ISuffix suffix)
        {
            foreach (string name in suffixNames)
            {
                suffixes[name] = suffix;
            }
        }
    }
}
