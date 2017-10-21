using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using System.Linq;
using kOS.Safe.Encapsulation;

namespace kOS.Safe.Persistence
{
    [kOS.Safe.Utilities.KOSNomenclature("VolumeFile")]
    public abstract class VolumeFile : VolumeItem
    {
        protected static SuffixMap VolumeFileSuffixes<T>() where T : VolumeFile
        {
            SuffixMap suffixes = VolumeItemSuffixes<T>();

            suffixes.AddSuffix("READALL", new Suffix<T, FileContent>((vfile) => vfile.ReadAll));
            suffixes.AddSuffix("WRITE", new OneArgsSuffix<T, BooleanValue, Structure>((vfile) => (str) => vfile.WriteObject(str)));
            suffixes.AddSuffix("WRITELN", new OneArgsSuffix<T, BooleanValue, StringValue>((vfile) => (str) => new BooleanValue(vfile.WriteLn(str))));
            suffixes.AddSuffix("CLEAR", new NoArgsVoidSuffix<T>((vfile) => vfile.Clear));

            return suffixes;
        }

        protected VolumeFile(Volume volume, VolumePath path, SuffixMap suffixes) : base(volume, path, suffixes)
        {
        }

        public abstract FileContent ReadAll();

        public abstract bool Write(byte[] content);

        public bool Write(string content)
        {
            return Write(FileContent.EncodeString(content));
        }

        public bool WriteLn(string content)
        {
            return Write(content + FileContent.NewLine);
        }

        public abstract void Clear();

        public override string ToString()
        {
            return Name;
        }

        private bool WriteObject(Structure content)
        {
            if (content is StringValue)
            {
                return Write(content.ToString());
            }

            var stringValue = content as FileContent;
            if (stringValue != null)
            {
                FileContent fileContent = stringValue;
                return Write(fileContent.Bytes);
            }

            throw new KOSException("Only instances of string and FileContent can be written");
        }
    }
}