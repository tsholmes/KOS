﻿using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Serialization;
using System;
using System.Collections.Generic;

namespace kOS.Safe
{
    [kOS.Safe.Utilities.KOSNomenclature("Range")]
    public class RangeValue : EnumerableValue<Range>
    {
        private static readonly SuffixMap suffixes;

        static RangeValue()
        {
            suffixes = EnumerableSuffixes<RangeValue>();

            suffixes.AddSuffix("START", new NoArgsSuffix<RangeValue, ScalarValue>((range) => () => range.InnerEnumerable.Start));
            suffixes.AddSuffix("STOP", new NoArgsSuffix<RangeValue, ScalarValue>((range) => () => range.InnerEnumerable.Stop));
            suffixes.AddSuffix("STEP", new NoArgsSuffix<RangeValue, ScalarValue>((range) => () => range.InnerEnumerable.Step));
        }

        private const string DumpStart = "start";
        private const string DumpStop = "stop";
        private const string DumpStep = "step";
        private const string Label = "RANGE";

        public static readonly int DEFAULT_START = 0;
        public static readonly int DEFAULT_STOP = 1;
        public static readonly int DEFAULT_STEP = 1;

        public RangeValue()
            : this(DEFAULT_STOP)
        {
        }

        public RangeValue(int stop)
            : this(DEFAULT_START, stop)
        {
        }

        public RangeValue(int start, int stop)
            : this(start, stop, DEFAULT_STEP)
        {
        }

        public RangeValue(int start, int stop, int step)
            : base(Label, new Range(start, stop, step), suffixes)
        {
            if (step < 1)
            {
                throw new KOSException("Step must be a positive integer");
            }
        }

        public override void LoadDump(Dump dump)
        {
            InnerEnumerable.Stop = Convert.ToInt32(dump[DumpStop]);
            InnerEnumerable.Start = Convert.ToInt32(dump[DumpStart]);
            InnerEnumerable.Step = Convert.ToInt32(dump[DumpStep]);
        }

        public override Dump Dump()
        {
            DumpWithHeader result = new DumpWithHeader();

            result.Header = "RANGE";

            result.Add(DumpStop, InnerEnumerable.Stop);
            result.Add(DumpStart, InnerEnumerable.Start);
            result.Add(DumpStep, InnerEnumerable.Step);

            return result;
        }

        public override string ToString()
        {
            return "RANGE(" + InnerEnumerable.Start + ", " + InnerEnumerable.Stop + ", " + InnerEnumerable.Step + ")";
        }
    }

    public class Range : IEnumerable<Structure>
    {
        public int Start { get; set; }
        public int Stop { get; set; }
        public int Step { get; set; }

        public Range(int start, int stop, int step)
        {
            Start = start;
            Stop = stop;
            Step = step;
        }

        IEnumerator<Structure> IEnumerable<Structure>.GetEnumerator()
        {
            if (Start < Stop)
            {
                for (int i = Start; i < Stop; i += Step)
                {
                    yield return (ScalarIntValue)i;
                }
            }
            else
            {
                for (int i = Start; i > Stop; i -= Step)
                {
                    yield return (ScalarIntValue)i;
                }
            }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}