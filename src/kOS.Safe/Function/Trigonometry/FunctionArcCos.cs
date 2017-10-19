using System;

namespace kOS.Safe.Function.Trigonometry
{
    [Function("arccos")]
    public class FunctionArcCos : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            double argument = GetDouble(PopValueAssert(shared));
            AssertArgBottomAndConsume(shared);
            double result = RadiansToDegrees(Math.Acos(argument));
            ReturnValue = result;
        }
    }
}