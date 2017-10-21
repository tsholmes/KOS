using System;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Safe.Encapsulation
{
    public interface ISuffixMap
    {
        ISuffixResult GetSuffix(string suffixName, Structure structure);
        bool SetSuffix(string suffixName, Structure structure, object value);
        bool HasSuffix(string suffixName);
        ListValue GetSuffixNames();
    }
}
