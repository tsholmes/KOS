using kOS.Safe.Exceptions;
using kOS.Safe.Persistence;

namespace kOS.Safe.Function.Persistence
{
    [Function("volume")]
    public class FunctionVolume : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            int remaining = CountRemainingArgs(shared);

            Volume volume;

            if (remaining == 0)
            {
                volume = shared.VolumeMgr.CurrentVolume;
            }
            else
            {
                object volumeId = PopValueAssert(shared, true);
                volume = shared.VolumeMgr.GetVolume(volumeId);

                if (volume == null)
                {
                    throw new KOSPersistenceException("Could not find volume: " + volumeId);
                }
            }

            AssertArgBottomAndConsume(shared);

            ReturnValue = volume;
        }
    }
}
