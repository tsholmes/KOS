namespace kOS.Safe.Function.Misc
{
    [Function("debugdump")]
    public class DebugDump : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            AssertArgBottomAndConsume(shared);
            ReturnValue = shared.Cpu.DumpVariables();
        }
    }
}