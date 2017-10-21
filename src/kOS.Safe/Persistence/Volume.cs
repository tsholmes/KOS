using System.Collections.Generic;
using System.Linq;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;

namespace kOS.Safe.Persistence
{
    [kOS.Safe.Utilities.KOSNomenclature("Volume")]
    public abstract class Volume : Structure
    {
        protected static SuffixMap VolumeSuffixes<T>() where T : Volume
        {
            SuffixMap suffixes = StructureSuffixes<T>();

            suffixes.AddSuffix("FREESPACE" , new Suffix<T, ScalarValue>((volume) => () => volume.FreeSpace));
            suffixes.AddSuffix("CAPACITY" , new Suffix<T, ScalarValue>((volume) => () => volume.Capacity));
            suffixes.AddSuffix("NAME" , new SetSuffix<T, StringValue>((volume) => () => volume.Name, (volume) => (newName) => volume.Name = newName));
            suffixes.AddSuffix("RENAMEABLE" , new Suffix<T, BooleanValue>((volume) => () => volume.Renameable));
            suffixes.AddSuffix("POWERREQUIREMENT" , new Suffix<T, ScalarValue>((volume) => () => volume.RequiredPower()));

            suffixes.AddSuffix("ROOT" , new Suffix<T, VolumeDirectory>((volume) => () => volume.Root));
            suffixes.AddSuffix("EXISTS" , new OneArgsSuffix<T, BooleanValue, StringValue>((volume) => (path) => volume.Exists(path)));
            suffixes.AddSuffix("FILES" , new Suffix<T, Lexicon>((volume) => volume.ListAsLexicon));
            suffixes.AddSuffix("CREATE" , new OneArgsSuffix<T, VolumeFile, StringValue>((volume) => (path) => volume.CreateFile(path)));
            suffixes.AddSuffix("CREATEDIR" , new OneArgsSuffix<T, VolumeDirectory, StringValue>((volume) => (path) => volume.CreateDirectory(path)));
            suffixes.AddSuffix("OPEN" , new OneArgsSuffix<T, Structure, StringValue>((volume) => (path) => volume.OpenSafe(path)));
            suffixes.AddSuffix("DELETE" , new OneArgsSuffix<T, BooleanValue, StringValue>((volume) => (path) => volume.Delete(path)));

            return suffixes;
        }

        public const string TEXT_EXTENSION = "txt";
        public const string KERBOSCRIPT_EXTENSION = "ks";
        public const string KOS_MACHINELANGUAGE_EXTENSION = "ksm";
        protected const long BASE_CAPACITY = 10000;
        protected const float BASE_POWER = 0.04f;
        public const int INFINITE_CAPACITY = -1;

        private string name;

        public abstract VolumeDirectory Root { get; }
        public long Capacity { get; protected set; }
        public long FreeSpace
        {
            get
            {
                return Capacity == INFINITE_CAPACITY ? INFINITE_CAPACITY : Capacity - Size;
            }
        }
        public long Size
        {
            get
            {
                return Root.Size;
            }
        }
        public bool Renameable { get; protected set; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (Renameable)
                {
                    name = value;
                }
                else
                {
                    throw new KOSException("Volume name can't be changed");
                }
            }
        }

        protected Volume(SuffixMap suffixes) : base(suffixes)
        {
            Renameable = true;
            Capacity = -1;
            Name = "";
        }

        protected void InitializeName(string name)
        {
            this.name = name;
        }

        public abstract void Clear();

        public VolumeItem Open(string pathString, bool ksmDefault = false)
        {
            return Open(VolumePath.FromString(pathString), ksmDefault);
        }

        public Structure OpenSafe(string pathString, bool ksmDefault = false)
        {
            VolumeItem item = Open(VolumePath.FromString(pathString), ksmDefault);

            return item != null ? (Structure)item : BooleanValue.False;
        }

        /// <summary>
        /// Get a file given its name
        /// </summary>
        /// <param name="name">filename to get.  if it has no filename extension, one will be guessed at, ".ks" usually.</param>
        /// <param name="ksmDefault">in the scenario where there is no filename extension, do we prefer the .ksm over the .ks?  The default is to prefer .ks</param>
        /// <returns>VolumeFile or VolumeDirectory. Null if not found.</returns>
        public abstract VolumeItem Open(VolumePath path, bool ksmDefault = false);

        public VolumeDirectory CreateDirectory(string pathString)
        {
            return CreateDirectory(VolumePath.FromString(pathString));
        }

        public abstract VolumeDirectory CreateDirectory(VolumePath path);

        public VolumeFile CreateFile(string pathString)
        {
            return CreateFile(VolumePath.FromString(pathString));
        }

        public abstract VolumeFile CreateFile(VolumePath path);

        public VolumeDirectory OpenOrCreateDirectory(VolumePath path)
        {
            VolumeDirectory directory = Open(path) as VolumeDirectory;

            if (directory == null)
            {
                directory = CreateDirectory(path);
            }

            return directory;
        }

        public VolumeFile OpenOrCreateFile(VolumePath path, bool ksmDefault = false)
        {
            VolumeFile file = Open(path, ksmDefault) as VolumeFile;

            if (file == null)
            {
                file = CreateFile(path);
            }

            return file;
        }

        public bool Exists(string pathString, bool ksmDefault = false)
        {
            return Exists(VolumePath.FromString(pathString), ksmDefault);
        }

        public abstract bool Exists(VolumePath path, bool ksmDefault = false);

        public bool Delete(string pathString, bool ksmDefault = false)
        {
            return Delete(VolumePath.FromString(pathString), ksmDefault);
        }

        public abstract bool Delete(VolumePath path, bool ksmDefault = false);

        public VolumeFile SaveFile(VolumeFile volumeFile)
        {
            return SaveFile(volumeFile.Path, volumeFile.ReadAll());
        }

        public abstract VolumeFile SaveFile(VolumePath path, FileContent content, bool verifyFreeSpace = true);

        public bool IsRoomFor(VolumePath path, FileContent fileContent)
        {
            VolumeItem existing = Open(path);

            if (existing is VolumeDirectory)
            {
                throw new KOSPersistenceException("'" + path + "' is a directory");
            }

            VolumeFile existingFile = existing as VolumeFile;

            int usedByThisFile = 0;

            if (existingFile != null)
            {
                usedByThisFile = existingFile.ReadAll().Size;
            }

            return INFINITE_CAPACITY == FreeSpace || FreeSpace + usedByThisFile >= fileContent.Size;
        }

        public virtual float RequiredPower()
        {
            var multiplier = (float)Capacity / BASE_CAPACITY;
            var powerRequired = BASE_POWER * multiplier;

            return powerRequired;
        }

        public Lexicon ListAsLexicon()
        {
            return Root.ListAsLexicon();
        }

        public override string ToString()
        {
            return "Volume(" + Name + ", " + Capacity + ")";
        }
    }
}
