using System;
using kOS.Safe.Persistence;
using System.Collections.Generic;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using System.Collections;

namespace kOS.Safe
{
    [kOS.Safe.Utilities.KOSNomenclature("VolumeDirectory")]
    public abstract class VolumeDirectory : VolumeItem, IEnumerable<VolumeItem>
    {
        protected static SuffixMap VolumeDirectorySuffixes<T>() where T : VolumeDirectory
        {
            SuffixMap suffixes = VolumeItemSuffixes<T>();

            suffixes.AddSuffix("ITERATOR", new NoArgsSuffix<T, Enumerator>((vdir) => () => new Enumerator(vdir.GetEnumerator())));
            suffixes.AddSuffix("LIST", new Suffix<T, Lexicon>((vdir) => vdir.ListAsLexicon));

            return suffixes;
        }

        protected VolumeDirectory(Volume volume, VolumePath path, SuffixMap suffixes) : base(volume, path, suffixes)
        {
        }

        public Lexicon ListAsLexicon()
        {
            Lexicon result = new Lexicon();

            foreach (KeyValuePair<string, VolumeItem> entry in List())
            {
                result.Add(new StringValue(entry.Key), entry.Value);
            }

            return result;
        }

        public abstract IDictionary<string, VolumeItem> List();

        public IEnumerator<VolumeItem> GetEnumerator()
        {
            return List().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
