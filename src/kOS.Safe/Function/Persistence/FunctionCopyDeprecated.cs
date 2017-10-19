using kOS.Safe.Encapsulation;

namespace kOS.Safe.Function.Persistence
{
    [Function("copy_deprectated")]
    public class FunctionCopyDeprecated : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object arg3 = PopValueAssert(shared, true);
            object arg2 = PopValueAssert(shared, true);
            object arg1 = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            string fromName, toName;

            if (arg2.ToString() == "from")
            {
                fromName = arg3.ToString() + ":/" + arg1.ToString();
                toName = "";
            }
            else
            {
                fromName = arg1.ToString();
                toName = arg3.ToString() + ":/" + fromName;
            }

            shared.Logger.LogWarningAndScreen(
                string.Format( "WARNING: COPY {0} {1} {2} is deprecated as of kOS v1.0.0.  Use COPYPATH(\"{3}\", \"{4}\") instead.",
                              arg1.ToString(), arg2.ToString(), arg3.ToString(), fromName, toName));

            // Redirect into a call to the copypath function, so as to keep all
            // the copy file logic there in one unified location.  This is slightly slow,
            // but we don't care because this is just to support deprecation:
            shared.Cpu.PushStack(new kOS.Safe.Execution.KOSArgMarkerType());
            shared.Cpu.PushStack(fromName);
            shared.Cpu.PushStack(toName);
            shared.Cpu.CallBuiltinFunction("copypath");
        }
    }
}
