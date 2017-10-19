using kOS.Safe.Persistence;

namespace kOS.Safe.Function.Persistence
{
    [Function("path")]
    public class FunctionPath : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            int remaining = CountRemainingArgs(shared);

            GlobalPath path;

            if (remaining == 0)
            {
                path = GlobalPath.FromVolumePath(shared.VolumeMgr.CurrentDirectory.Path,
                                                 shared.VolumeMgr.GetVolumeRawIdentifier(shared.VolumeMgr.CurrentVolume));
            }
            else
            {
                object pathObject = PopValueAssert(shared, true);
                path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            }

            AssertArgBottomAndConsume(shared);

            ReturnValue = new PathValue(path, shared);
        }
    }
}
