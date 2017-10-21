using System.Collections.Generic;
using System.Linq;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Serialization;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Stack")]
    public class StackValue : EnumerableValue<Stack<Structure>>
    {
        private static readonly SuffixMap suffixes;

        static StackValue()
        {
            suffixes = EnumerableSuffixes<StackValue>();

            suffixes.AddSuffix("COPY",     new NoArgsSuffix<StackValue, StackValue>((stack) => stack.Copy));
            suffixes.AddSuffix("PUSH",     new OneArgsVoidSuffix<StackValue, Structure>((stack) => stack.Push));
            suffixes.AddSuffix("POP",      new NoArgsSuffix<StackValue, Structure>((stack) => stack.Pop));
            suffixes.AddSuffix("PEEK",     new NoArgsSuffix<StackValue, Structure>((stack) => stack.Peek));
            suffixes.AddSuffix("CLEAR",    new NoArgsVoidSuffix<StackValue>((stack) => stack.Clear));
        }

        public StackValue() : this(new Stack<Structure>())
        {
        }

        public StackValue(IEnumerable<Structure> stackValue) : base("STACK", new Stack<Structure>(stackValue), suffixes)
        {
        }

        public override IEnumerator<Structure> GetEnumerator()
        {
            return InnerEnumerable.Reverse().GetEnumerator();
        }

        public void Clear()
        {
            InnerEnumerable.Clear();
        }

        public StackValue Copy()
        {
            return new StackValue(this);
        }

        public Structure Pop()
        {
            return InnerEnumerable.Pop();
        }

        public Structure Peek()
        {
            return InnerEnumerable.Peek();
        }

        public void Push(Structure val)
        {
            InnerEnumerable.Push(val);
        }

        public override void LoadDump(Dump dump)
        {
            InnerEnumerable.Clear();

            List<object> values = ((List<object>)dump[kOS.Safe.Dump.Items]);

            values.Reverse();

            foreach (object item in values)
            {
                InnerEnumerable.Push((Structure)Structure.FromPrimitive(item));
            }
        }

        public static StackValue CreateStack<TU>(IEnumerable<TU> list)
        {
            return new StackValue(list.Cast<Structure>());
        }
    }
}