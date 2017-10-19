using kOS.Safe.Persistence;

namespace kOS.Safe.Function.Persistence
{
    [Function("createdir")]
    public class FunctionCreateDir : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object pathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

            VolumeDirectory volumeDirectory = volume.CreateDirectory(path);

            ReturnValue = volumeDirectory;
        }
    }
}