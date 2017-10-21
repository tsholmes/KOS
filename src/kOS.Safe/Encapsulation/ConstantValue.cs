using System;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Constant")]
    public class ConstantValue : Structure
    {
        /// <summary>
        /// kiloPascals to atmospheres
        /// </summary>
        public const double KpaToAtm = 0.00986923266716012830002467308167;

        private static readonly SuffixMap suffixes;

        static ConstantValue()
        {
            suffixes = StructureSuffixes<ConstantValue>();

            suffixes.AddSuffix("G", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => 6.67384*Math.Pow(10,-11)));
            suffixes.AddSuffix("E", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => Math.E));
            suffixes.AddSuffix("PI", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => Math.PI));
            suffixes.AddSuffix("C", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => 299792458.0, "Speed of light in m/s")); 
            suffixes.AddSuffix("ATMTOKPA", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => 101.325, "atmospheres to kiloPascals" ));
            suffixes.AddSuffix("KPATOATM", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => KpaToAtm, "kiloPascals to atmospheres"));

            // pi/180 :
            suffixes.AddSuffix("DEGTORAD", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => 0.01745329251994329576923690768489, "degrees to radians"));
            
            // 180/pi :
            suffixes.AddSuffix("RADTODEG", new NoArgsSuffix<ConstantValue, ScalarValue>((constants) => () => 57.295779513082320876798154814105, "radians to degrees"));
        }

        public ConstantValue() : base(suffixes)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} Constants", base.ToString());
        }
    }

}