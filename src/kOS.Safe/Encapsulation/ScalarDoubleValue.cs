namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Scalar", KOSToCSharp = false)]
    public class ScalarDoubleValue : ScalarValue
    {
        private static readonly SuffixMap suffixes;

        static ScalarDoubleValue()
        {
            suffixes = ScalarSuffixes<ScalarDoubleValue>();

            Zero = new ScalarDoubleValue(0);
        }

        public static ScalarDoubleValue Zero;

        public override bool IsDouble
        {
            get { return true; }
        }

        public override bool IsInt
        {
            get { return false; }
        }

        public override bool BooleanMeaning
        {
            get { return (double)Value != 0d; }
        }

        public ScalarDoubleValue(double value) : base(suffixes)
        {
            Value = value;
        }

        public static implicit operator ScalarDoubleValue(double val)
        {
            return new ScalarDoubleValue(val);
        }

        public static ScalarDoubleValue MinValue()
        {
            return new ScalarDoubleValue(double.MinValue);
        }

        public static ScalarDoubleValue MaxValue()
        {
            return new ScalarDoubleValue(double.MaxValue);
        }
    }
}