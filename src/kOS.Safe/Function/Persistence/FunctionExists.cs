using kOS.Safe.Persistence;

namespace kOS.Safe.Function.Persistence
{
    [Function("exists")]
    public class FunctionExists : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object pathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

            ReturnValue = volume.Exists(path);
        }
    }
}