using kOS.Safe.Compilation;

namespace kOS.Safe.Function.Persistence
{
    [Function("scriptpath")]
    public class FunctionScriptPath : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            AssertArgBottomAndConsume(shared);

            int currentOpcode = shared.Cpu.GetCallTrace()[0];
            Opcode opcode = shared.Cpu.GetOpcodeAt(currentOpcode);

            ReturnValue = new PathValue(opcode.SourcePath, shared);
        }
    }
}
