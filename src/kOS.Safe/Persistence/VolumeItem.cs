using System;
using kOS.Safe.Persistence;
using System.Linq;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Safe.Persistence
{
    [kOS.Safe.Utilities.KOSNomenclature("VolumeItem")]
    public abstract class VolumeItem : Structure
    {
        protected static SuffixMap VolumeItemSuffixes<T>() where T : VolumeItem
        {
            SuffixMap suffixes = StructureSuffixes<T>();

            suffixes.AddSuffix("NAME", new Suffix<T, StringValue>((vitem) => () => vitem.Name));
            suffixes.AddSuffix("SIZE", new Suffix<T, ScalarIntValue>((vitem) => () => new ScalarIntValue(vitem.Size)));
            suffixes.AddSuffix("EXTENSION", new Suffix<T, StringValue>((vitem) => () => vitem.Extension));
            suffixes.AddSuffix("ISFILE", new Suffix<T, BooleanValue>((vitem) => () => vitem is VolumeFile));

            return suffixes;
        }

        public Volume Volume { get; set; }
        public VolumePath Path { get; set; }

        public string Name
        {
            get
            {
                return Path.Name;
            }
        }

        public string Extension
        {
            get
            {
                return Path.Extension;
            }
        }

        protected VolumeItem(Volume volume, VolumePath path, SuffixMap suffixes) : base(suffixes)
        {
            Volume = volume;
            Path = path;
        }

        protected VolumeItem(Volume volume, VolumePath parentPath, String name, SuffixMap suffixes) : base(suffixes)
        {
            Volume = volume;
            Path = VolumePath.FromString(name, parentPath);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Path.Name) ? "Root directory" : Path.Name;
        }

        public abstract int Size { get; }
    }
}

