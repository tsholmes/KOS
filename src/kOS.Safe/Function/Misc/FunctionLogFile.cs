using kOS.Safe.Encapsulation;
using kOS.Safe.Persistence;

namespace kOS.Safe.Function.Misc
{
    [Function("logfile")]
    public class FunctionLogFile : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            string pathString = PopValueAssert(shared, true).ToString();
            string toAppend = PopValueAssert(shared).ToString();
            AssertArgBottomAndConsume(shared);

            if (shared.VolumeMgr != null)
            {
                GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathString);
                Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

                VolumeItem volumeItem = volume.Open(path) as VolumeFile;
                VolumeFile volumeFile = null;

                if (volumeItem == null)
                {
                    volumeFile = volume.CreateFile(path);
                }
                else if (volumeItem is VolumeDirectory)
                {
                    throw new KOSFileException("Can't append to file: path points to a directory");
                }
                else
                {
                    volumeFile = volumeItem as VolumeFile;
                }

                if (!volumeFile.WriteLn(toAppend))
                {
                    throw new KOSFileException("Can't append to file: not enough space or access forbidden");
                }

            }
        }
    }
}