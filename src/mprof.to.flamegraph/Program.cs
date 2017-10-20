using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace mprof.to.flamegraph
{
    class MainClass
    {
        public static void Main (string [] args)
        {
            while (Console.ReadLine() != "Method call summary") {}

            Console.ReadLine();

            string callMatcher = @"^\s+\d+\s+(\d+)\s+(\d+)\s+(.+)$";
            string callsFromMatcher = @"^\s+(\d+) calls from:\s*$";

            List<string> output = new List<string>();

            string line = Console.ReadLine();
            while(true)
            {
                if (line.StartsWith("Total calls"))
                {
                    break;
                }
                Match callMatch = Regex.Match(line, callMatcher);
                if (!callMatch.Success)
                {
                    throw new Exception("Unexpected line: " + line);
                }
                int self = int.Parse(callMatch.Groups[1].Value);
                int calls = int.Parse(callMatch.Groups[2].Value);
                string func = callMatch.Groups[3].Value;

                line = Console.ReadLine();

                bool done = false;

                int callsRem = calls;
                int msRem = self;

                while (!done)
                {
                    Match fromCountMatch = Regex.Match(line, callsFromMatcher);
                    if (!fromCountMatch.Success)
                    {
                        break;
                    }
                    int fromCount = int.Parse(fromCountMatch.Groups[1].Value);
                    string stack = "";

                    int thisMs = msRem * fromCount / callsRem;

                    callsRem -= fromCount;
                    msRem -= thisMs;

                    while (true)
                    {
                        line = Console.ReadLine();
                        if (Regex.Match(line, callMatcher).Success || line.StartsWith("Total calls"))
                        {
                            done = true;
                            break;
                        }
                        if (Regex.Match(line, callsFromMatcher).Success)
                        {
                            break;
                        }
                        stack += line.Trim() + ";";
                    }
                    output.Add(stack + func + " " + thisMs);
                }
                if (callsRem != 0)
                {
                    output.Add(func + " " + self);
                }
            }

            //output.Sort();

            foreach (string stack in output)
            {
                Console.WriteLine(stack);
            }
        }
    }
}
