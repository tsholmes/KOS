using System;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Scalar", KOSToCSharp = false)]
    public class ScalarIntValue : ScalarValue
    {
        private static readonly SuffixMap suffixes;

        static ScalarIntValue()
        {
            suffixes = ScalarSuffixes<ScalarIntValue>();

            Zero = new ScalarIntValue(0);
            One = new ScalarIntValue(1);
            Two = new ScalarIntValue(2);
        }

        // those are handy especially in tests
        public static ScalarIntValue Zero;
        public static ScalarIntValue One;
        public static ScalarIntValue Two;

        public override bool IsDouble
        {
            get { return false; }
        }

        public override bool IsInt
        {
            get { return true; }
        }

        public override bool BooleanMeaning
        {
            get { return (int)Value != 0; }
        }

        public ScalarIntValue(int value) : base(suffixes)
        {
            Value = value;
        }

        public static implicit operator ScalarIntValue(int val)
        {
            return new ScalarIntValue(val);
        }

        public static ScalarIntValue MinValue()
        {
            return new ScalarIntValue(int.MinValue);
        }

        public static ScalarIntValue MaxValue()
        {
            return new ScalarIntValue(int.MaxValue);
        }

    }
}