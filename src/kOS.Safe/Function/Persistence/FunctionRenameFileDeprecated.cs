using kOS.Safe.Encapsulation;

namespace kOS.Safe.Function.Persistence
{
    [Function("rename_file_deprecated")]
    public class FunctionRenameFileDeprecated : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object newArg = PopValueAssert(shared, true);
            object oldArg = PopValueAssert(shared, true);
            PopValueAssert(shared, true); // This gets ignored because we already know its "file".

            string newName = newArg.ToString();
            string oldName = oldArg.ToString();
            AssertArgBottomAndConsume(shared);

            shared.Logger.LogWarningAndScreen(
                string.Format( "WARNING: RENAME FILE {0} TO {1} is deprecated as of kOS v1.0.0.  Use MOVEPATH(\"{2}\", \"{3}\") instead.",
                              oldName, newName, oldName, newName));

            // Redirect into a call to the movepath function, so as to keep all
            // the file logic there in one unified location.  This is slightly slow,
            // but we don't care because this is just to support deprecation:
            shared.Cpu.PushStack(new kOS.Safe.Execution.KOSArgMarkerType());
            shared.Cpu.PushStack(oldName);
            shared.Cpu.PushStack(newName);
            shared.Cpu.CallBuiltinFunction("movepath");
        }
    }
}
