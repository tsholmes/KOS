using kOS.Safe.Compilation;
using kOS.Safe.Execution;
using kOS.Safe.Persistence;
using System.Collections.Generic;

namespace kOS.Safe.Function.Misc
{
    [FunctionAttribute("load")]
    public class FunctionLoad : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            // NOTE: The built-in load() function actually ends up returning
            // two things on the stack: on top is a boolean for whether the program
            // was already loaded, and under that is an integer for where to jump to
            // to call it.  The load() function is NOT meant to be called directly from
            // a user script.
            // (unless it's being called in compile-only mode, in which case it
            // returns the default dummy zero on the stack like everything else does).

            UsesAutoReturn = true;
            bool defaultOutput = false;
            bool justCompiling = false; // is this load() happening to compile, or to run?
            GlobalPath outPath = null;
            object topStack = PopValueAssert(shared, true); // null if there's no output file (output file means compile, not run).
            if (topStack != null)
            {
                justCompiling = true;
                string outputArg = topStack.ToString();
                if (outputArg.Equals("-default-compile-out-"))
                    defaultOutput = true;
                else
                    outPath = shared.VolumeMgr.GlobalPathFromObject(outputArg);
            }

            object skipAlreadyObject = PopValueAssert(shared, false);
            bool skipIfAlreadyCompiled = (skipAlreadyObject is bool) ? (bool)skipAlreadyObject : false;

            object pathObject = PopValueAssert(shared, true);

            AssertArgBottomAndConsume(shared);

            if (pathObject == null)
                throw new KOSFileException("No filename to load was given.");

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

            VolumeFile file = volume.Open(path, !justCompiling) as VolumeFile; // if running, look for KSM first.  If compiling look for KS first.
            if (file == null) throw new KOSFileException(string.Format("Can't find file '{0}'.", path));
            path = GlobalPath.FromVolumePath(file.Path, shared.VolumeMgr.GetVolumeId(volume));

            if (skipIfAlreadyCompiled && !justCompiling)
            {
                var programContext = shared.Cpu.SwitchToProgramContext();
                int programAddress = programContext.GetAlreadyCompiledEntryPoint(path.ToString());
                if (programAddress >= 0)
                {
                    // TODO - The check could also have some dependancy on whether the file content changed on
                    //     disk since last time, but that would also mean having to have a way to clear out the old
                    //     copy of the compiled file from the program context, which right now doesn't exist. (Without
                    //     that, doing something like a loop that re-wrote a file and re-ran it 100 times would leave
                    //     100 old dead copies of the compiled opcodes in memory, only the lastmost copy being really used.)

                    // We're done here.  Skip the compile.  Point the caller at the already-compiled version.
                    shared.Cpu.PushStack(programAddress);
                    this.ReturnValue = true; // tell caller that it already existed.
                    return;
                }
            }

            FileContent fileContent = file.ReadAll();

            // filename is now guaranteed to have an extension.  To make default output name, replace the extension with KSM:
            if (defaultOutput)
                outPath = path.ChangeExtension(Volume.KOS_MACHINELANGUAGE_EXTENSION);

            if (path.Equals(outPath))
                throw new KOSFileException("Input and output paths must differ.");

            if (shared.VolumeMgr == null) return;
            if (shared.VolumeMgr.CurrentVolume == null) throw new KOSFileException("Volume not found");

            if (shared.ScriptHandler != null)
            {
                shared.Cpu.StartCompileStopwatch();
                var options = new CompilerOptions { LoadProgramsInSameAddressSpace = true, FuncManager = shared.FunctionManager };
                // add this program to the address space of the parent program,
                // or to a file to save:
                if (justCompiling)
                {
                    // since we've already read the file content, use the volume from outPath instead of the source path
                    volume = shared.VolumeMgr.GetVolumeFromPath(outPath);
                    shared.Cpu.YieldProgram(YieldFinishedCompile.CompileScriptToFile(path, 1, fileContent.String, options, volume, outPath));
                }
                else
                {
                    var programContext = shared.Cpu.SwitchToProgramContext();
                    List<CodePart> parts;
                    if (fileContent.Category == FileCategory.KSM)
                    {
                        string prefix = programContext.Program.Count.ToString();
                        parts = fileContent.AsParts(path, prefix);
                        int programAddress = programContext.AddObjectParts(parts, path.ToString());
                        // push the entry point address of the new program onto the stack
                        shared.Cpu.PushStack(programAddress);
                    }
                    else
                    {
                        UsesAutoReturn = false;
                        shared.Cpu.YieldProgram(YieldFinishedCompile.LoadScript(path, 1, fileContent.String, "program", options));
                    }
                    this.ReturnValue = false; // did not already exist.
                }
                shared.Cpu.StopCompileStopwatch();
            }
        }
    }
}