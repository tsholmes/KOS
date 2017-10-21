using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Safe.Communication
{
    [kOS.Safe.Utilities.KOSNomenclature("Connection")]
    public abstract class Connection : Structure
    {
        static SuffixMap ConnectionSuffixes<T>() where T : Connection
        {
            SuffixMap suffixes = StructureSuffixes<T>();
            
            suffixes.AddSuffix("ISCONNECTED", new Suffix<T, BooleanValue>((connection) => () => connection.Connected));
            suffixes.AddSuffix("DELAY", new Suffix<T, ScalarValue>((connection) => () => connection.Delay));
            suffixes.AddSuffix("SENDMESSAGE", new OneArgsSuffix<T, BooleanValue, Structure>((connection) => connection.SendMessage));
            suffixes.AddSuffix("DESTINATION", new NoArgsSuffix<T, Structure>((connection) => connection.Destination));

            return suffixes;
        }

        public abstract bool Connected { get; }
        public abstract double Delay { get; }

        protected Connection(SuffixMap suffixes) : base(suffixes)
        {
        }

        protected abstract Structure Destination();
        protected abstract BooleanValue SendMessage(Structure content);
    }
}
