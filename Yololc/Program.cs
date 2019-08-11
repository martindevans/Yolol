using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using CommandLine;
using JetBrains.Annotations;
using Yolol.Analysis;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Grammar;

namespace Yololc
{
    public class Options
    {
        [Option('i', "input", HelpText = "File to read YOLOL code from", Required = false)]
        public string InputFile { get; set; }

        [Option('v', "verbose", HelpText = "Control if output of non-code messages are displayed", Required = false, Default = false)]
        public bool Verbose { get; set; }

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

        [Option("disable_compoundincrementsubstitution", HelpText = "Do not replace a+=1 with a++", Required = false)]
        public bool DisableCompoundIncrementSubstitution { get; set; }


        [Option("disable_disabletrailinggotocompression", HelpText = "Do not replace a final `if x then goto y end` statement with a smaller equivalent", Required = false)]
        public bool DisableTrailingConditionalGotoAnyLineCompression { get; set; }

        [Option("disable_disableconditionalassignmentcompression", HelpText = "Do not replace `if a then b = c end` with a smaller equivalent", Required = false)]
        public bool DisableCompressConditionalAssignment { get; set; }
        


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

        private static string ReadInput([NotNull] Options options)
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

        private static void Run([NotNull] Options options)
        {
            var input = ReadInput(options);
            var startLength = input.Length;

            var tokens = Tokenizer.TryTokenize(input);
            if (!tokens.HasValue)
            {
                Console.Error.WriteLine($"{tokens.FormatErrorMessageFragment()}");
                Console.Error.WriteLine(tokens.ErrorPosition);
                return;
            }

            var astResult = Yolol.Grammar.Parser.TryParseProgram(tokens.Value);
            if (!astResult.HasValue)
            {
                Console.Error.WriteLine($"{astResult.FormatErrorMessageFragment()}");
                Console.Error.WriteLine(astResult.ErrorPosition);
                return;
            }

            var ast = astResult.Value;

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
                if (!options.DisableCompoundIncrementSubstitution && !options.DisableAstTransformPasses)
                    a = a.CompressCompoundIncrement();
                if (!options.DisableVariableNameSimplification && !options.DisableAstTransformPasses)
                    a = a.SimplifyVariableNames();
                if (!options.DisableDeadPostGotoElimination && !options.DisableAstTransformPasses)
                    a = a.DeadPostGotoElimination();
                if (!options.DisableEolGotoElimination && !options.DisableAstTransformPasses)
                    a = a.TrailingGotoNextLineElimination();
                if (!options.DisableTrailingConditionalGotoAnyLineCompression && !options.DisableAstTransformPasses)
                    a = a.TrailingConditionalGotoAnyLineCompression();
                if (!options.DisableCompressConditionalAssignment && !options.DisableAstTransformPasses)
                    a = a.CompressConditionalAssignment();
                if (!options.DisableConstantCompression && !options.DisableAstTransformPasses)
                    a = a.CompressConstants();

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

            Console.WriteLine(output);
        }
    }
}
