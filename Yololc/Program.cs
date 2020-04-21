using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using Yolol.Analysis;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace Yololc
{
    public class Options
    {
        [Option('i', "input", HelpText = "File to read YOLOL code from", Required = false)]
        public string? InputFile { get; set; }

        [Option('v', "verbose", HelpText = "Control if output of non-code messages are displayed", Required = false, Default = false)]
        public bool Verbose { get; set; }

        [Option('a', "ast", HelpText = "Emit Cylon AST instead of Yolol code")]
        public bool EmitAst { get; set; }

        [Option("iterations", HelpText = "Maximum number of times to apply the optimisation passes", Required = false, Default = 10)]
        public int MaxIterations { get; set; }


        [Option("disable_astpasses", HelpText = "Disable all passes based on AST transforms", Required = false, Default = false)]
        public bool DisableAstTransformPasses { get; set; }

        [Option("disable_cfgpasses", HelpText = "Disable all passes based on CFG analysis", Required = false, Default = false)]
        public bool DisableSimpleAstPasses { get; set; }






        [Option("disable_constantfolding", HelpText = "Do not replace constant expressions/statements with their result", Required = false)]
        public bool DisableConstantFolding { get; set; }

        [Option("disable_constanthoisting", HelpText = "Do not convert repeated inline constants into variables with value of constant", Required = false)]
        public bool DisableConstantHoisting { get; set; }

        [Option("disable_constantcompression", HelpText = "Do not replace large constants with smaller equivalent mathematical expressions", Required = false)]
        public bool DisableConstantCompression { get; set; }


        [Option("disable_variablenamesimplification", HelpText = "Do not replace variables with simpler names", Required = false)]
        public bool DisableVariableNameSimplification { get; set; }



        [Option("disable_disabletrailinggotocompression", HelpText = "Do not replace a final `if x then goto y end` statement with a smaller equivalent", Required = false)]
        public bool DisableTrailingConditionalGotoAnyLineCompression { get; set; }

        [Option("disable_ifthengotocompression", HelpText = "Do not replace `if X then goto Y else goto Z end` with a smaller equivalent", Required = false)]
        public bool DisableIfThenGotoCompression { get; set; }



        [Option("disable_deadpostgotoelimination", HelpText = "Do not eliminate code which is unreachable due to preceding unconditional gotos", Required = false)]
        public bool DisableDeadPostGotoElimination { get; set; }

        [Option("disable_eolgotonextlineelimination", HelpText = "Do not eliminate goto statements at the end of a line which go to the next line", Required = false)]
        public bool DisableEolGotoElimination { get; set; }
    }

    public class Program
    {
        private static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static string ReadInput(Options options)
        {
            if (options.InputFile == null)
            {
                // Read from stdin
                var result = new StringBuilder();
                string line;
                while ((line = Console.ReadLine()) != null)
                    result.AppendLine(line);

                return result.ToString();
            }
            else
            {
                // Read from file
                if (!File.Exists(options.InputFile))
                    throw new FileNotFoundException("Input file does not exist", options.InputFile);

                return File.ReadAllText(options.InputFile);
            }
        }

        private static Yolol.Grammar.AST.Program? TryParseStringInput(string input)
        {
            Yolol.Grammar.AST.Program? TryParseAsYolol(ICollection<string> log)
            {
                var result = Yolol.Grammar.Parser.ParseProgram(input);
                if (result.IsOk)
                    return result.Ok;

                log.Add($"{result.Err.Message} @ {result.Err.Cursor}");
                return null;
            }

            Yolol.Grammar.AST.Program? TryParseAsAst(ICollection<string> log)
            {
                try
                {
                    return new Yolol.Cylon.Deserialisation.AstDeserializer().Parse(input);
                }
                catch (Exception e)
                {
                    log.Add(e.ToString());
                    return null;
                }
            }

            var logYolol = new List<string>();
            var logAst = new List<string>();

            var y = TryParseAsYolol(logYolol);
            if (y != null)
                return y;

            var a = TryParseAsAst(logAst);
            if (a != null)
                return a;

            Console.Error.WriteLine("Failed to parse input as Yolol or JSON AST");
            Console.Error.WriteLine("## Yolol Errors:");
            foreach (var item in logYolol)
                Console.Error.WriteLine(item);
            Console.Error.WriteLine();
            Console.Error.WriteLine("## JSON AST Errors:");
            foreach (var item in logAst)
                Console.Error.WriteLine(item);

            return null;
        }

        private static void Run(Options options)
        {
            var input = ReadInput(options);
            var startLength = input.Length;

            var ast = TryParseStringInput(input);
            if (ast == null)
                return;

            var i = 0;
            ast = ast.Fixpoint(options.MaxIterations, a => {

                i++;
                if (options.Verbose)
                    Console.WriteLine($"Starting Iteration: {i}");

                var timer = new Stopwatch();
                timer.Start();

                // Apply AST transform passes
                if (!options.DisableConstantFolding && !options.DisableAstTransformPasses)
                    a = a.FoldConstants();
                if (!options.DisableConstantHoisting && !options.DisableAstTransformPasses)
                    a = a.HoistConstants();
                if (!options.DisableEolGotoElimination && !options.DisableAstTransformPasses)
                    a = a.TrailingGotoNextLineElimination();
                if (!options.DisableTrailingConditionalGotoAnyLineCompression && !options.DisableAstTransformPasses)
                    a = a.TrailingConditionalGotoAnyLineCompression();
                if (!options.DisableIfThenGotoCompression && !options.DisableAstTransformPasses)
                    a = a.ConditionalGotoCompression(new RandomNameGenerator(1));
                if (!options.DisableDeadPostGotoElimination && !options.DisableAstTransformPasses)
                    a = a.DeadPostGotoElimination();
                if (!options.DisableConstantCompression && !options.DisableAstTransformPasses)
                    a = a.CompressConstants();
                if (!options.DisableVariableNameSimplification && !options.DisableAstTransformPasses)
                    a = a.SimplifyVariableNames();

                if (options.Verbose)
                    Console.WriteLine($"{timer.ElapsedMilliseconds}ms");

                return a;
            });

            var output = ast.ToString();
            if (options.Verbose)
            {
                Console.WriteLine("Done");
                Console.Write($"reduced from {startLength} characters to {output.Length} characters");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            if (options.EmitAst)
            {
                ast = ast.Fixpoint(a => new FlattenStatementLists().Visit(a));
                Console.WriteLine(new Yolol.Cylon.Serialisation.AstSerializer().Serialize(ast).ToString(Formatting.Indented));
            }
            else
                Console.WriteLine(output);
        }
    }
}
