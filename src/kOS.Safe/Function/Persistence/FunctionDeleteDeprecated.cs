using kOS.Safe.Encapsulation;

namespace kOS.Safe.Function.Persistence
{
    [Function("delete_deprecated")]
    public class FunctionDeleteDeprecated : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object volumeId = PopValueAssert(shared, true);
            object fileName = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            string pathName;
            if (volumeId != null)
                pathName = volumeId.ToString() + ":/" + fileName.ToString();
            else
                pathName = fileName.ToString();

            shared.Logger.LogWarningAndScreen(
                string.Format( "WARNING: DELETE {0}{1} is deprecated as of kOS v1.0.0.  Use DELETEPATH(\"{2}\") instead.",
                              fileName.ToString(), (volumeId == null ? "" : (" FROM " + volumeId.ToString())), pathName));

            // Redirect into a call to the deletepath function, so as to keep all
            // the file logic there in one unified location.  This is slightly slow,
            // but we don't care because this is just to support deprecation:
            shared.Cpu.PushStack(new kOS.Safe.Execution.KOSArgMarkerType());
            shared.Cpu.PushStack(pathName);
            shared.Cpu.CallBuiltinFunction("deletepath");
        }
    }
}
