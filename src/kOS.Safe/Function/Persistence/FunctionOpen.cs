using kOS.Safe.Persistence;
using kOS.Safe.Exceptions;

namespace kOS.Safe.Function.Persistence
{
    [Function("open")]
    public class FunctionOpen : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object pathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

            VolumeItem volumeItem = volume.Open(path);

            if (volumeItem == null)
            {
                throw new KOSException("File or directory does not exist: " + path);
            }

            ReturnValue = volumeItem;
        }
    }
}