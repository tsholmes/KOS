using kOS.Safe.Persistence;

namespace kOS.Safe.Function.Persistence
{
    [Function("rename_volume_deprecated")]
    public class FunctionRenameVolumeDeprecated : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object newArg = PopValueAssert(shared, true);
            object oldArg = PopValueAssert(shared, true);
            PopValueAssert(shared, true); // This gets ignored because we already know its "volume".

            string newName = newArg.ToString();
            string oldName = oldArg.ToString();
            AssertArgBottomAndConsume(shared);

            Volume volume = oldArg is Volume ? oldArg as Volume : shared.VolumeMgr.GetVolume(oldName);

            shared.Logger.LogWarningAndScreen(
                string.Format( "WARNING: RENAME VOLUME {0} TO {1} is deprecated as of kOS v1.0.0.  Use SET VOLUME({2}):NAME TO \"{3}\" instead.",
                              oldName, newName, volume.Name, newName));

            volume.Name = newName;
        }
    }
}
