using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Version")]
    public class VersionInfo : Structure
    {
        private static readonly SuffixMap suffixes;

        private readonly int major;
        private readonly int minor;
        private readonly int patch;
        private readonly int build;

        static VersionInfo()
        {
            suffixes = StructureSuffixes<VersionInfo>();

            suffixes.AddSuffix("MAJOR", new NoArgsSuffix<VersionInfo, ScalarValue>((version) => () => version.major));
            suffixes.AddSuffix("MINOR", new NoArgsSuffix<VersionInfo, ScalarValue>((version) => () => version.minor));
            suffixes.AddSuffix("PATCH", new NoArgsSuffix<VersionInfo, ScalarValue>((version) => () => version.patch));
            suffixes.AddSuffix("BUILD", new NoArgsSuffix<VersionInfo, ScalarValue>((version) => () => version.build));
        }

        public VersionInfo(int major, int minor, int patch, int build) : base(suffixes)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
            this.build = build;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", major, minor, patch, build);
        }
    }
}
