using kOS.Safe.Compilation;
using kOS.Safe.Exceptions;
using kOS.Safe.Execution;
using kOS.Safe.Persistence;
using System;
using System.Collections.Generic;

namespace kOS.Safe.Function.Misc
{
    [Function("run")]
    public class FunctionRun : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            // run() is strange.  It needs two levels of args - the args to itself, and the args it is meant to
            // pass on to the program it's invoking.  First, these are the args to run itself:
            object volumeId = PopValueAssert(shared, true);
            object pathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            // Now the args it is going to be passing on to the program:
            var progArgs = new List<object>();
            int argc = CountRemainingArgs(shared);
            for (int i = 0; i < argc; ++i)
                progArgs.Add(PopValueAssert(shared, true));
            AssertArgBottomAndConsume(shared);

            if (shared.VolumeMgr == null) return;

            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathObject);
            Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);
            VolumeFile volumeFile = volume.Open(path) as VolumeFile;

            FileContent content = volumeFile != null ? volumeFile.ReadAll() : null;

            if (content == null) throw new Exception(string.Format("File '{0}' not found", path));

            if (shared.ScriptHandler == null) return;

            if (volumeId != null)
            {
                throw new KOSObsoletionException("v1.0.2", "run [file] on [volume]", "None", "");
            }
            else
            {
                // clear the "program" compilation context
                shared.Cpu.StartCompileStopwatch();
                shared.ScriptHandler.ClearContext("program");
                //string filePath = shared.VolumeMgr.GetVolumeRawIdentifier(shared.VolumeMgr.CurrentVolume) + "/" + fileName;
                var options = new CompilerOptions { LoadProgramsInSameAddressSpace = true, FuncManager = shared.FunctionManager };
                var programContext = shared.Cpu.SwitchToProgramContext();

                List<CodePart> codeParts;
                if (content.Category == FileCategory.KSM)
                {
                    string prefix = programContext.Program.Count.ToString();
                    codeParts = content.AsParts(path, prefix);
                    programContext.AddParts(codeParts);
                    shared.Cpu.StopCompileStopwatch();
                }
                else
                {
                    shared.Cpu.YieldProgram(YieldFinishedCompile.RunScript(path, 1, content.String, "program", options));
                }
            }

            // Because run() returns FIRST, and THEN the CPU jumps to the new program's first instruction that it set up,
            // it needs to put the return stack in a weird order.  Its return value needs to be buried UNDER the args to the
            // program it's calling:
            UsesAutoReturn = false;

            shared.Cpu.PushStack(0); // dummy return that all functions have.

            // Put the args for the program being called back on in the same order they were in before (so read the list backward):
            shared.Cpu.PushStack(new KOSArgMarkerType());
            for (int i = argc - 1; i >= 0; --i)
                shared.Cpu.PushStack(progArgs[i]);
        }
    }
}