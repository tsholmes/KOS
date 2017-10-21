using kOS.Safe.Utilities;

namespace kOS.Safe.Encapsulation.Suffixes
{
    public class ClampSetSuffix<T, TValue> : SetSuffix<T, TValue> where T : Structure where TValue : ScalarValue
    {
        private readonly double min;
        private readonly double max;
        private readonly float stepIncrement;

        public ClampSetSuffix(GetDel get, SetDel set, double min, double max, float stepIncrement, string description = "") 
            : this(get, set, min, max, description)
        {
            this.stepIncrement = stepIncrement;
        }

        public ClampSetSuffix(GetDel get, SetDel set, double min, double max, string description = "")
            : base(get, set, description)
        {
            this.min = min;
            this.max = max;
        }

        public override void Set(Structure structure, object value)
        {
            var dblValue = System.Convert.ToDouble(value);

            base.Set(structure, System.Math.Abs(stepIncrement) < 0.0001
                ? KOSMath.Clamp(dblValue, min, max)
                : KOSMath.ClampToIndent(dblValue, min, max, stepIncrement));
        }
    }
}