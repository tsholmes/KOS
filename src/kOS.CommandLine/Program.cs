using kOS.Safe.Execution;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS.CommandLine
{
    class Program
    {
        //static string run = "run perf_scalaradd3.ks. log profileresult() to \"profile-{0}.csv\". shutdown.";
        //static string scalarAddBenchmark = 
        //    @"set x to 1.
        //    set y to 2.
        //    set z to 0.
        //    print x + y.
        //    local count is 200000.
        //    until count = 0 {
        //      set z to z + x + y.
        //      set count to count - 1.
        //    }
        //    shutdown.";
        //static string fib =
        //    @"
        //    set a to 0.
        //    set b to 1.
        //    until b > 10000000 {
        //      print(b).
        //      local c to a + b.
        //      set a to b.
        //      set b to c.
        //    }
        //    shutdown.";
        //static string simpleTest = "set x to 0.";
        static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            //Console.WindowHeight = 50;
            //Console.WindowWidth = 160;
            //System.Threading.Thread.Sleep(2000);
            var worker = new Execution.kOSWorker();
            int totalInstructions = 0;
            worker.FixedUpdate();
            totalInstructions += worker.Shared.Cpu.InstructionsThisUpdate;
            //worker.PushScript(simpleTest);
            //worker.PushScript(scalarAddBenchmark);
            //worker.PushScript(fib);
            worker.PushScript("runpath(\"perf/sort.ks\").");
            //worker.PushScript(string.Format(run, DateTime.Now.ToString("yyyy-MM-dd-HH-mm")));
            while (worker.GetMode() == Safe.Module.ProcessorModes.READY)
            {
                worker.FixedUpdate();
                totalInstructions += worker.Shared.Cpu.InstructionsThisUpdate;
                //System.Threading.Thread.Sleep(20);
            }
            Console.WriteLine("Total Instructions: " + totalInstructions);
            //var result = worker.Shared.Cpu.ProfileResult;
            //if (result != null) Console.WriteLine(string.Join("\r\n", result.ToArray()));
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }
    }
}
