using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Safe.Encapsulation
{
    public interface ISuffix
    {
        ISuffixResult Get(Structure s);
        string Description { get; }
    }
}