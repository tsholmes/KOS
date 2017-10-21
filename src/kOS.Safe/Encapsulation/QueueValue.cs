using System.Collections.Generic;
using System.Linq;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Serialization;

namespace kOS.Safe.Encapsulation
{
    [kOS.Safe.Utilities.KOSNomenclature("Queue")]
    public class QueueValue : EnumerableValue<Queue<Structure>>
    {
        private static readonly SuffixMap suffixes;

        static QueueValue()
        {
            suffixes = EnumerableSuffixes<QueueValue>();

            suffixes.AddSuffix("CLEAR", new NoArgsVoidSuffix<QueueValue>((queue) => queue.InnerEnumerable.Clear));

            suffixes.AddSuffix("COPY", new NoArgsSuffix<QueueValue, QueueValue>((queue) => queue.Copy));

            suffixes.AddSuffix("PUSH", new OneArgsVoidSuffix<QueueValue, Structure>((queue) => queue.Push));
            suffixes.AddSuffix("POP", new NoArgsSuffix<QueueValue, Structure>((queue) => queue.Pop));
            suffixes.AddSuffix("PEEK", new NoArgsSuffix<QueueValue, Structure>((queue) => queue.InnerEnumerable.Peek));
            suffixes.AddSuffix("CLEAR", new NoArgsVoidSuffix<QueueValue>((queue) => queue.InnerEnumerable.Clear));
        }
        
        public QueueValue() : this(new Queue<Structure>())
        {
        }

        public QueueValue(IEnumerable<Structure> queueValue) : base("QUEUE", new Queue<Structure>(queueValue), suffixes)
        {
        }

        public Structure Pop()
        {
            return InnerEnumerable.Dequeue();
        }

        public void Push(Structure val)
        {
            InnerEnumerable.Enqueue(val);
        }

        public QueueValue Copy()
        {
            return new QueueValue(this);
        }

        public override void LoadDump(Dump dump)
        {
            InnerEnumerable.Clear();

            List<object> values = (List<object>)dump[kOS.Safe.Dump.Items];

            foreach (object item in values)
            {
                InnerEnumerable.Enqueue((Structure)FromPrimitive(item));
            }
        }

        public static QueueValue CreateQueue<T>(IEnumerable<T> list)
        {
            return new QueueValue(list.Cast<Structure>());
        }
    }
}