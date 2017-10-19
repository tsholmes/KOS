using kOS.Safe.Compilation;
using kOS.Safe.Encapsulation;
using kOS.Safe.Exceptions;
using System;

namespace kOS.Safe.Function.KOSMath
{
    [Function("min")]
    public class FunctionMin : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            Structure argument1 = PopStructureAssertEncapsulated (shared);
            Structure argument2 = PopStructureAssertEncapsulated (shared);
            AssertArgBottomAndConsume (shared);
            Type scalarCompare = typeof (ScalarValue);
            Type stringCompare = typeof (StringValue);
            if (scalarCompare.IsInstanceOfType (argument1) && scalarCompare.IsInstanceOfType (argument2)) {
                double d1 = ((ScalarValue)argument1).GetDoubleValue ();
                double d2 = ((ScalarValue)argument2).GetDoubleValue ();
                ReturnValue = Math.Min (d1, d2);
            } else if (stringCompare.IsInstanceOfType (argument1) && stringCompare.IsInstanceOfType (argument2)) {
                string arg1 = argument1.ToString ();
                string arg2 = argument2.ToString ();
                int compareNum = string.Compare (arg1, arg2, StringComparison.OrdinalIgnoreCase);
                ReturnValue = (compareNum < 0) ? arg1 : arg2;
            } else {
                throw new KOSException ("Argument Mismatch: the function MIN only accepts matching arguments of type Scalar or String");
            }
        }
    }
}