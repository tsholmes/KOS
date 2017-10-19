using System;

namespace kOS.Safe.Function.Trigonometry
{
    [Function("cos")]
    public class FunctionCos : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            double degrees = GetDouble(PopValueAssert(shared));
            AssertArgBottomAndConsume(shared);
            double radians = DegreesToRadians(degrees);
            double result = Math.Cos(radians);
            ReturnValue = result;
        }
    }
}