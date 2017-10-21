using System;
using kOS.Safe.Serialization;
using kOS.Safe.Persistence;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Encapsulation;
using System.Linq;

namespace kOS.Safe
{
    /// <summary>
    /// Contains suffixes related to GlobalPath.
    ///
    /// This exists as a separate class because some of the suffixes require an instance of VolumeManager to work. I think
    /// it would be counter-productive to pass around an instance of VolumeManager whenever we're dealing with GlobalPath internally.
    /// Instances of this class are on the other hand created only for the user.
    /// </summary>
    [kOS.Safe.Utilities.KOSNomenclature("Path")]
    public class PathValue : SerializableStructure, IHasSafeSharedObjects
    {
        private static readonly SuffixMap suffixes;

        static PathValue()
        {
            suffixes = StructureSuffixes<PathValue>();

            suffixes.AddSuffix("VOLUME", new Suffix<PathValue, Volume>((path) => () => path.sharedObjects.VolumeMgr.GetVolumeFromPath(path.Path)));
            suffixes.AddSuffix("SEGMENTS", new Suffix<PathValue, ListValue>((path) => () => new ListValue(path.Path.Segments.Select((s) => (Structure)new StringValue(s)))));
            suffixes.AddSuffix("LENGTH", new Suffix<PathValue, ScalarIntValue>((path) => () => path.Path.Length));
            suffixes.AddSuffix("NAME", new Suffix<PathValue, StringValue>((path) => () => path.Path.Name));
            suffixes.AddSuffix("HASEXTENSION", new Suffix<PathValue, BooleanValue>((path) => () => string.IsNullOrEmpty(path.Path.Extension)));
            suffixes.AddSuffix("EXTENSION", new Suffix<PathValue, StringValue>((path) => () => path.Path.Extension));
            suffixes.AddSuffix("ROOT", new Suffix<PathValue, PathValue>((path) => () => path.FromPath(path.Path.RootPath())));
            suffixes.AddSuffix("PARENT", new Suffix<PathValue, PathValue>((path) => () => path.FromPath(path.Path.GetParent())));

            suffixes.AddSuffix("ISPARENT", new OneArgsSuffix<PathValue, BooleanValue, PathValue>((path) => (p) => path.Path.IsParent(p.Path)));
            suffixes.AddSuffix("CHANGENAME", new OneArgsSuffix<PathValue, PathValue, StringValue>((path) => (n) => path.FromPath(path.Path.ChangeName(n))));
            suffixes.AddSuffix("CHANGEEXTENSION", new OneArgsSuffix<PathValue, PathValue, StringValue>((path) => (e) => path.FromPath(path.Path.ChangeExtension(e))));
            suffixes.AddSuffix("COMBINE", new VarArgsSuffix<PathValue, PathValue, Structure>((path) => path.Combine));
        }

        private const string DumpPath = "path";

        public GlobalPath Path { get; private set; }
        private SafeSharedObjects sharedObjects;

        public SafeSharedObjects Shared {
            set
            {
                sharedObjects = value;
            }
        }

        public PathValue() : base(suffixes)
        {
        }

        public PathValue(GlobalPath path, SafeSharedObjects sharedObjects) : this()
        {
            Path = path;
            this.sharedObjects = sharedObjects;
        }

        public PathValue FromPath(GlobalPath path)
        {
            return new PathValue(path, sharedObjects);
        }

        public PathValue FromPath(VolumePath volumePath, string volumeId)
        {
            return new PathValue(GlobalPath.FromVolumePath(volumePath, volumeId), sharedObjects);
        }

        public PathValue Combine(params Structure[] segments)
        {
            if (segments.All(s => s.GetType() == typeof(StringValue)))
            {
                return Combine(segments.Cast<StringValue>().ToArray());
            }
            throw new Exceptions.KOSInvalidArgumentException("PATH:COMBINE", "SEGMENTS", "all segments must be strings");
        }

        public PathValue Combine(params StringValue[] segments)
        {
            return FromPath(Path.Combine(segments.Select(s => s.ToString()).ToArray()));
        }

        public override Dump Dump()
        {
            return new Dump { { DumpPath, Path.ToString() } };
        }

        public override void LoadDump(Dump dump)
        {
            Path = GlobalPath.FromString(dump[DumpPath] as string);
        }

        public override string ToString()
        {
            return Path.ToString();
        }

        public override bool Equals(object other)
        {
            PathValue pVal = other as PathValue;
            if (!ReferenceEquals(pVal,null)) // ReferenceEquals prevents infinite recursion with overloaded == operator.
                return Path == pVal.Path;
            GlobalPath gVal = other as GlobalPath;
            if (!ReferenceEquals(gVal,null)) // ReferenceEquals prevents infinite recursion with overloaded == operator.
                return Path == gVal;

            // fallback:
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            if (!ReferenceEquals(Path,null)) // ReferenceEquals prevents infinite recursion with overloaded == operator.
                return Path.GetHashCode();
            return base.GetHashCode();
        }

        public static bool operator ==(PathValue left, PathValue right)
        {
            if (ReferenceEquals(left,null) || ReferenceEquals(right,null)) // ReferenceEquals prevents infinite recursion with overloaded == operator.
                return ReferenceEquals(left, null) && ReferenceEquals(right, null); // ReferenceEquals prevents infinite recursion with overloaded == operator.
            return left.Equals(right);
        }
        public static bool operator !=(PathValue left, PathValue right)
        {
            return !(left == right);
        }

    }
}

