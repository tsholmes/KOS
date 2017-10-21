using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Execution;
using kOS.Safe.UserIO;

namespace kOS.Safe.Encapsulation
{
    /// <summary>
    /// Handles the keyboard (any maybe other types later??) input
    /// into the terminal.
    /// </summary>
    [kOS.Safe.Utilities.KOSNomenclature("TerminalInput")]
    public class TerminalInput : Structure
    {
        private static readonly SuffixMap suffixes;

        static TerminalInput()
        {
            suffixes = StructureSuffixes<TerminalInput>();

            suffixes.AddSuffix("GETCHAR", new Suffix<TerminalInput, StringValue>((input) => input.GetChar));
            suffixes.AddSuffix("HASCHAR", new Suffix<TerminalInput, BooleanValue>((input) => input.HasChar));
            suffixes.AddSuffix("CLEAR", new NoArgsVoidSuffix<TerminalInput>((input) => input.Clear));

            // Aliases for special characters:
            foreach (UnicodeCommand code in codesToExpose)
            {
                suffixes.AddSuffix(code.ToString(), new Suffix<TerminalInput, StringValue>((input) => () => new StringValue((char)code)));
            }
            suffixes.AddSuffix("BACKSPACE", new Suffix<TerminalInput, StringValue>((input) => () => new StringValue((char)0x008)));
            suffixes.AddSuffix(new[] { "RETURN", "ENTER" }, new Suffix<TerminalInput, StringValue>((input) => () => new StringValue((char)0x00D)));
        }

        private SafeSharedObjects shared;

        /// <summary>
        /// The subset of values within the UnicodeCommand enum that
        /// we want to expose to kOS scripts to see as suffixes
        /// to TerminalInput.
        /// </summary>
        private static UnicodeCommand[] codesToExpose =
            {
                UnicodeCommand.UPCURSORONE,
                UnicodeCommand.DOWNCURSORONE,
                UnicodeCommand.LEFTCURSORONE,
                UnicodeCommand.RIGHTCURSORONE,
                UnicodeCommand.HOMECURSOR,
                UnicodeCommand.ENDCURSOR,
                UnicodeCommand.PAGEUPCURSOR,
                UnicodeCommand.PAGEDOWNCURSOR,
                UnicodeCommand.DELETERIGHT
            };

        public TerminalInput(SafeSharedObjects shared) : base(suffixes)
        {
            this.shared = shared;
        }

        /// <summary>
        /// Get the next char in the keyboard input queue.  This is a blocking
        /// I/O if called from a kerboscript opcode.  It will pause the program
        /// until such a time as input exists.
        /// </summary>
        /// <returns></returns>
        public StringValue GetChar()
        {
            var q = shared.Screen.CharInputQueue;
            if (q.Count > 0)
            {
                // Just return the input char now without wait.
                return new StringValue(q.Dequeue());
            }
            else
            {
                // Blocks until input exists.
                shared.Cpu.YieldProgram(new YieldFinishedGetChar());

                // Note the user should never see this value.  The value will
                // get popped from the stack and replaced by the real value.  This
                // is done by YieldFinishedGetChar when it is finished waiting:
                return new StringValue((char)0);
            }
        }

        public void Clear()
        {
            shared.Screen.CharInputQueue.Clear();
        }

        public BooleanValue HasChar()
        {
            var q = shared.Screen.CharInputQueue;
            if (q.Count > 0)
                return true;
            else
                return false;
        }
    }
}