using kOS.Safe.Persistence;
using System;

namespace kOS.Safe.Function.Persistence
{
    [Function("deletepath")]
    public class FunctionDeletePath : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object pathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

            ReturnValue = volume.Delete(path);
        }
    }
}