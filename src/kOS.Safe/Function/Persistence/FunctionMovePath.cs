using kOS.Safe.Persistence;
using System;

namespace kOS.Safe.Function.Persistence
{
    [Function("movepath")]
    public class FunctionMovePath : SafeFunctionBase
    {
        public override void Execute(SafeSharedObjects shared)
        {
            object destinationPathObject = PopValueAssert(shared, true);
            object sourcePathObject = PopValueAssert(shared, true);
            AssertArgBottomAndConsume(shared);

            GlobalPath sourcePath = shared.VolumeMgr.GlobalPathFromObject(sourcePathObject);
            GlobalPath destinationPath = shared.VolumeMgr.GlobalPathFromObject(destinationPathObject);

            ReturnValue = shared.VolumeMgr.Move(sourcePath, destinationPath);
        }
    }
}