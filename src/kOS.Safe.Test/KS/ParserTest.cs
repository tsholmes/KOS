using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using kOS.Safe.Compilation.KS;
using NUnit.Framework;

namespace kOS.Safe.Test.KS
{
    [TestFixture]
    public class ParserTest
    {
        Scanner scanner;
        Parser parser;

        [SetUp]
        public void Setup()
        {
            scanner = new Scanner();
            parser = new Parser(scanner);
        }

        [Test, Timeout(2000)]
        public void ParsesLargeNumbers()
        {
            parser.Parse("012345678901234567890123456789");
            parser.Parse("// 012345678901234567890123456789");
        }
        
        [Test]
        public void ParsedAllTestFiles() {
          List<string> contents = new List<string>();
          
          // string baseDir = "../../../../kerboscript_tests";
          string baseDir = "../../../../../../dunbaratu/kerboscripts";
          
          foreach(string file in Directory.GetFiles(baseDir, "*.ks", SearchOption.AllDirectories)) {
            if (file.EndsWith("dock_passive_target.ks")) continue;
            contents.Add(File.ReadAllText(file));
          }
          
          Stopwatch stopwatch = Stopwatch.StartNew();
          
          foreach (string script in contents) {
            ParseTree tree = parser.Parse(script);
            if (tree.Errors.Count > 0) {
              Console.WriteLine(script);
              throw new Exception(tree.Errors[0].Message);
            }
          }
          
          stopwatch.Stop();
          
          Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}
