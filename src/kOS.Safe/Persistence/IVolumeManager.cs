using System.Collections.Generic;

namespace kOS.Safe.Persistence
{
    public interface IVolumeManager
    {
        Dictionary<int, Volume> Volumes { get; }
        Volume CurrentVolume { get; }
        VolumeDirectory CurrentDirectory { get; set; }
        bool VolumeIsCurrent(Volume volume);
        int GetVolumeId(Volume volume);
        Volume GetVolume(object volumeId);
        Volume GetVolume(string name);
        Volume GetVolume(int id);
        Volume GetVolumeFromPath(GlobalPath path);
        void Add(Volume volume);
        void Remove(string name);
        void Remove(int id);
        void SwitchTo(Volume volume);
        void UpdateVolumes(List<Volume> attachedVolumes);
        bool CheckCurrentVolumeRange();
        bool CheckRange(Volume volume);
        string GetVolumeBestIdentifier(Volume volume);
        bool Copy(GlobalPath sourcePath, GlobalPath destinationPath, bool verifyFreeSpace = true);
        bool Move(GlobalPath sourcePath, GlobalPath destinationPath);

        /// <summary>
        /// This creates a proper, absolute GlobalPath from the given object (which is assumed to come from the user).
        /// This handles volumes, files, directories, absolute paths (for example 'volume:/some/path'),
        /// paths relative to current volume ('/some/path') and paths relative to current directory ('../some/path', 'some/path').
        ///
        /// Relative paths need current volume and current directory for resolution, that's why this method is part of this
        /// interface.
        /// </summary>
        /// <returns>GlobalPath instance</returns>
        /// <param name="pathString">Path string.</param>
        GlobalPath GlobalPathFromObject(object pathObject);

        /// <summary>
        /// Like GetVolumeBestIdentifier, but without the extra string formatting.
        /// </summary>
        /// <param name="volume"></param>
        /// <returns>The Volume's Identifier without pretty formatting</returns>
        string GetVolumeRawIdentifier(Volume volume);
    }
}