﻿using kOS.Execution;
using kOS.Safe.Compilation;
using kOS.Safe.Exceptions;
using kOS.Safe.Execution;
using kOS.Safe.Function;
using kOS.Safe.Module;
using kOS.Safe.Persistence;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using System;
using System.Text;
using System.Collections.Generic;
using kOS.Suffixed.PartModuleField;
using kOS.Module;
using kOS.Safe.Compilation.KS;
using kOS.Safe.Encapsulation;
using KSP.UI.Screens;
using kOS.Safe;

namespace kOS.Function
{
    [Function("clearscreen")]
    public class FunctionClearScreen : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            AssertArgBottomAndConsume(shared);
            shared.Window.ClearScreen();
        }
    }

    [Function("print")]
    public class FunctionPrint : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            string textToPrint = PopValueAssert(shared).ToString();
            AssertArgBottomAndConsume(shared);
            shared.Screen.Print(textToPrint);
        }
    }

    [Function("hudtext")]
    public class FunctionHudText : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            bool      echo      = Convert.ToBoolean(PopValueAssert(shared));
            RgbaColor rgba      = GetRgba(PopValueAssert(shared));
            int       size      = Convert.ToInt32(PopValueAssert(shared));
            int       style     = Convert.ToInt32(PopValueAssert(shared));
            int       delay     = Convert.ToInt32(PopValueAssert(shared));
            string    textToHud = PopValueAssert(shared).ToString();
            AssertArgBottomAndConsume(shared);
            string htmlColour = rgba.ToHexNotation();
            switch (style)
            {
                case 1:
                    ScreenMessages.PostScreenMessage("<color=" + htmlColour + "><size=" + size + ">" + textToHud + "</size></color>", delay, ScreenMessageStyle.UPPER_LEFT);
                    break;

                case 2:
                    ScreenMessages.PostScreenMessage("<color=" + htmlColour + "><size=" + size + ">" + textToHud + "</size></color>", delay, ScreenMessageStyle.UPPER_CENTER);
                    break;

                case 3:
                    ScreenMessages.PostScreenMessage("<color=" + htmlColour + "><size=" + size + ">" + textToHud + "</size></color>", delay, ScreenMessageStyle.UPPER_RIGHT);
                    break;

                case 4:
                    ScreenMessages.PostScreenMessage("<color=" + htmlColour + "><size=" + size + ">" + textToHud + "</size></color>", delay, ScreenMessageStyle.LOWER_CENTER);
                    break;

                default:
                    ScreenMessages.PostScreenMessage("*" + textToHud, 3f, ScreenMessageStyle.UPPER_CENTER);
                    break;
            }
            if (echo)
            {
                shared.Screen.Print("HUD: " + textToHud);
            }
        }
    }
    
    [Function("printat")]
    public class FunctionPrintAt : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            int row = Convert.ToInt32(PopValueAssert(shared));
            int column = Convert.ToInt32(PopValueAssert(shared));
            string textToPrint = PopValueAssert(shared).ToString();
            AssertArgBottomAndConsume(shared);
            shared.Screen.PrintAt(textToPrint, row, column);
        }
    }

    [Function("toggleflybywire")]
    public class FunctionToggleFlyByWire : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            bool enabled = Convert.ToBoolean(PopValueAssert(shared));
            string paramName = PopValueAssert(shared).ToString();
            AssertArgBottomAndConsume(shared);
            ((CPU)shared.Cpu).ToggleFlyByWire(paramName, enabled);
        }
    }

    [Function("selectautopilotmode")]
    public class FunctionSelectAutopilotMode : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            string autopilotMode = PopValueAssert(shared).ToString();
            AssertArgBottomAndConsume(shared);
            ((CPU)shared.Cpu).SelectAutopilotMode(autopilotMode);
        }
    }

    [Function("stage")]
    public class FunctionStage : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            AssertArgBottomAndConsume(shared);
            if (StageManager.CanSeparate && shared.Vessel.isActiveVessel)
            {
                StageManager.ActivateNextStage();
                shared.Cpu.YieldProgram(new YieldFinishedNextTick());
            }
            else if (!StageManager.CanSeparate)
            {
                SafeHouse.Logger.Log("FAIL SILENT: Stage is called before it is ready, Use STAGE:READY to check first if staging rapidly");
            }
            else if (!shared.Vessel.isActiveVessel)
            {
                throw new KOSCommandInvalidHereException(LineCol.Unknown(), "STAGE", "a non-active SHIP, KSP does not support this", "Core is on the active vessel");
            }
        }
    }

    [Function("run")]
    public class FunctionRun : FunctionBase
    {
        public override void Execute(SharedObjects shared)
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

            shared.Cpu.PushArgumentStack(0); // dummy return that all functions have.

            // Put the args for the program being called back on in the same order they were in before (so read the list backward):
            shared.Cpu.PushArgumentStack(new KOSArgMarkerType());
            for (int i = argc - 1; i >= 0; --i)
                shared.Cpu.PushArgumentStack(progArgs[i]);
        }
    }

    [FunctionAttribute("load")]
    public class FunctionLoad : FunctionBase
    {
        public override void Execute(SharedObjects shared)
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
                    shared.Cpu.PushArgumentStack(programAddress);
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
                        shared.Cpu.PushArgumentStack(programAddress);
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

    [Function("add")]
    public class FunctionAddNode : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            var node = (Node)PopValueAssert(shared);
            AssertArgBottomAndConsume(shared);
            node.AddToVessel(shared.Vessel);
        }
    }

    [Function("remove")]
    public class FunctionRemoveNode : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            var node = (Node)PopValueAssert(shared);
            AssertArgBottomAndConsume(shared);
            node.Remove();
        }
    }

    [Function("logfile")]
    public class FunctionLogFile : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            string pathString = PopValueAssert(shared, true).ToString();
            string toAppend = PopValueAssert(shared).ToString();
            AssertArgBottomAndConsume(shared);

            if (shared.VolumeMgr != null)
            {
                GlobalPath path = shared.VolumeMgr.GlobalPathFromObject(pathString);
                Volume volume = shared.VolumeMgr.GetVolumeFromPath(path);

                VolumeItem volumeItem = volume.Open(path) as VolumeFile;
                VolumeFile volumeFile = null;

                if (volumeItem == null)
                {
                    volumeFile = volume.CreateFile(path);
                }
                else if (volumeItem is VolumeDirectory)
                {
                    throw new KOSFileException("Can't append to file: path points to a directory");
                }
                else
                {
                    volumeFile = volumeItem as VolumeFile;
                }

                if (!volumeFile.WriteLn(toAppend))
                {
                    throw new KOSFileException("Can't append to file: not enough space or access forbidden");
                }

            }
        }
    }

    [Function("reboot")]
    public class FunctionReboot : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            if (shared.Processor != null)
            {
                AssertArgBottomAndConsume(shared); // not sure if this matters when rebooting anwyway.
                shared.Cpu.GetCurrentOpcode().AbortProgram = true;
                shared.Processor.SetMode(ProcessorModes.OFF);
                shared.Processor.SetMode(ProcessorModes.READY);
            }
        }
    }

    [Function("shutdown")]
    public class FunctionShutdown : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            AssertArgBottomAndConsume(shared); // not sure if this matters when shutting down anwyway.
            shared.Cpu.GetCurrentOpcode().AbortProgram = true;
            if (shared.Processor != null) shared.Processor.SetMode(ProcessorModes.OFF);
        }
    }

    [Function("debugdump")]
    public class DebugDump : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            AssertArgBottomAndConsume(shared);
            ReturnValue = shared.Cpu.DumpVariables();
        }
    }
    
    [Function("debugfreezegame")]
    /// <summary>
    /// Deliberately cause physics lag by making the main game thread sleep.
    /// Clearly not something there's a good reason to do *except* when
    /// trying to test a script that only fails when the game is lagging.
    /// This is not the same thing as executing a WAIT command, because
    /// a WAIT command will return control to KSP and in a later physics
    /// tick, resume when the timer expires.  This function, on the other
    /// hand, refuses to return control back to the main game, suspending
    /// the KSP main game's thread for the number of suggested milliseconds.
    /// (Warning: Thread.Sleep() is incapable of being as precise as it may
    /// seem.  It takes milliseconds, but often can only sleep in 15 or 30 ms
    /// increments, rounding the sleep time up to the nearest increment chunk.)
    /// </summary>
    public class DebugFreezeGame : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            // How many milliseconds of extra sleep to cause?
            int ms = GetInt(PopValueAssert(shared));
            AssertArgBottomAndConsume(shared);
            System.Threading.Thread.Sleep(ms);
        }
    }

    [Function("profileresult")]
    public class ProfileResult : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            AssertArgBottomAndConsume(shared);
            if (shared.Cpu.ProfileResult == null || shared.Cpu.ProfileResult.Count == 0)
            {
                ReturnValue = "<no profile data available>";
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (string textLine in shared.Cpu.ProfileResult)
            {
                if (sb.Length > 0 )
                    sb.Append("\n");
                sb.Append(textLine);
            }
            ReturnValue = sb.ToString();
        }
    }

    [Function("warpto")]
    public class WarpTo : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            // TODO: As of KSP v1.0.2, the maxTimeWarping and minTimeWarping parameters behave as time limiters, not actual warp limiters
            int args = CountRemainingArgs(shared);
            double ut;
            switch (args)
            {
                case 1:
                    ut = GetDouble(PopValueAssert(shared));
                    break;

                default:
                    throw new KOSArgumentMismatchException(new[] { 1 }, args);
            }
            AssertArgBottomAndConsume(shared);
            TimeWarpValue.Instance.WarpTo(ut);
        }
    }

    [Function("processor")]
    public class FunctionProcessor : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            object processorTagOrVolume = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            kOSProcessor processor;

            if (processorTagOrVolume is Volume)
            {
                processor = shared.ProcessorMgr.GetProcessor(processorTagOrVolume as Volume);
            }
            else if (processorTagOrVolume is string || processorTagOrVolume is StringValue)
            {
                processor = shared.ProcessorMgr.GetProcessor(processorTagOrVolume.ToString());
            }
            else
            {
                throw new KOSInvalidArgumentException("processor", "processorId", "String or Volume expected");
            }

            if (processor == null)
            {
                throw new KOSInvalidArgumentException("processor", "processorId", "Processor with that volume or name was not found");
            }

            ReturnValue = PartModuleFieldsFactory.Construct(processor, shared);
        }
    }

    [Function("pidloop")]
    public class PIDLoopConstructor : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
            int args = CountRemainingArgs(shared);
            double kd;
            double ki;
            double kp;
            double maxoutput;
            double minoutput;
            switch (args)
            {
                case 0:
                    this.ReturnValue = new PIDLoop();
                    break;
                case 1:
                    kp = GetDouble(PopValueAssert(shared));
                    this.ReturnValue = new PIDLoop(kp, 0, 0);
                    break;
                case 3:
                    kd = GetDouble(PopValueAssert(shared));
                    ki = GetDouble(PopValueAssert(shared));
                    kp = GetDouble(PopValueAssert(shared));
                    this.ReturnValue = new PIDLoop(kp, ki, kd);
                    break;
                case 5:
                    maxoutput = GetDouble(PopValueAssert(shared));
                    minoutput = GetDouble(PopValueAssert(shared));
                    kd = GetDouble(PopValueAssert(shared));
                    ki = GetDouble(PopValueAssert(shared));
                    kp = GetDouble(PopValueAssert(shared));
                    this.ReturnValue = new PIDLoop(kp, ki, kd, maxoutput, minoutput);
                    break;
                default:
                    throw new KOSArgumentMismatchException(new[] { 0, 1, 3, 5 }, args);
            }
            AssertArgBottomAndConsume(shared);
        }
    }

    [Function("makebuiltindelegate")]
    public class MakeBuiltinDelegate : FunctionBase
    {
        public override void Execute(SharedObjects shared)
        {
           string name = PopValueAssert(shared).ToString();
           AssertArgBottomAndConsume(shared);

           ReturnValue = new BuiltinDelegate(shared.Cpu, name);
        }
    }
}
