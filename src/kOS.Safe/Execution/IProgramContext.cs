﻿using System;
using System.Collections.Generic;
using kOS.Safe.Compilation;
using kOS.Safe.Execution;

namespace kOS.Safe.Execution
{
    public interface IProgramContext
    {
        void AddParts(IEnumerable<CodePart> parts);
        int AddObjectParts(IEnumerable<CodePart> parts, string objectFilename);
        int GetAlreadyCompiledEntryPoint(string fileID);
        int ContextId { get; set; }
        void ToggleFlyByWire(string paramName, bool enabled);
        List<string> GetCodeFragment(int contextLines);
        List<string> GetCodeFragment(int start, int stop, bool doProfile = false);
        List<Opcode> Program { get; set; }
        int InstructionPointer { get; set; }
        bool Silent { get; set; }
    }
}
