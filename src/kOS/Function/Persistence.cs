using kOS.Safe.Encapsulation;
using kOS.Safe.Exceptions;
using kOS.Safe.Function;
using kOS.Safe.Persistence;
using kOS.Safe.Serialization;
using kOS.Safe.Utilities;
using kOS.Serialization;
using System;
using KSP.IO;
using kOS.Safe;
using kOS.Safe.Compilation;
using System.Collections.Generic;

namespace kOS.Function
{
    [Function("edit")]
    public class FunctionEdit : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            object pathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume vol = shared.VolumeMgr.GetVolumeFromPath(path);
            shared.Window.OpenPopupEditor(vol, path);

        }
    }

    [Function("cd", "chdir")]
    public class FunctionCd : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            int remaining = CountRemainingArgs(shared);

            VolumeDirectory directory;

            if (remaining == 0)
            {
                directory = shared.VolumeMgr.CurrentVolume.Root;
            }
            else
            {
                object pathObject = PopValueAssert(shared, true);

                GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
                Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

                directory = volume.Open(path) as VolumeDirectory;

                if (directory == null)
                {
                    throw new KOSException("Invalid directory: " + pathObject);
                }

            }

            AssertArgBottomAndConsume(shared);

            shared.VolumeMgr.CurrentDirectory = directory;
        }
    }


    [Function("writejson")]
    public class FunctionWriteJson : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            object pathObject = PopValueAssert(shared, true);
            SerializableStructure serialized = PopValueAssert(shared, true) as SerializableStructure;
            AssertArgBottomAndConsume(shared);

            if (serialized == null)
            {
                throw new KOSException("This type is not serializable");
            }

            string serializedString = new SerializationMgr(shared).Serialize(serialized, JsonFormatter.WriterInstance);

            FileContent fileContent = new FileContent(serializedString);

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

            ReturnValue = volume.SaveFile(path, fileContent);
        }
    }

    [Function("readjson")]
    public class FunctionReadJson : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            object pathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

            VolumeFile volumeFile = volume.Open(path) as VolumeFile;

            if (volumeFile == null)
            {
                throw new KOSException("File does not exist: " + path);
            }

            Structure read = new SerializationMgr(shared).Deserialize(volumeFile.ReadAll().String, JsonFormatter.ReaderInstance) as SerializableStructure;
            ReturnValue = read;
        }
    }
}