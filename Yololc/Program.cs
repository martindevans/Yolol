using System;
using System.IO;
using CommandLine;
using JetBrains.Annotations;
using Yolol.Grammar;
using Yolol.Analysis.Reduction;

namespace Yololc
{
    public class Options
    {
        [Option('i', "input", HelpText = "File to read YOLOL code from", Required = true)]
        public string InputFile { get; set; }


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

        private static void Run([NotNull] Options options)
        {
            if (!File.Exists(options.InputFile))
            {
                Console.Error.WriteLine($"Input file `{options.InputFile}` does not exist");
                return;
            }

            var input = File.ReadAllText(options.InputFile);

            var tokens = Tokenizer.TryTokenize(input);
            if (!tokens.HasValue)
            {
                Console.Error.WriteLine($"{tokens.FormatErrorMessageFragment()}");
                return;
            }

            var astResult = Yolol.Grammar.Parser.TryParseProgram(tokens.Value);
            if (!astResult.HasValue)
            {
                Console.Error.WriteLine($"{astResult.FormatErrorMessageFragment()}");
                return;
            }

            var ast = astResult.Value;

            if (!options.DisableConstantFolding)
                ast = ast.FoldConstants();
            if (!options.DisableConstantHoisting)
                ast = ast.HoistConstants();
            if (options.DisableCompoundIncrementSubstitution)
                ast = ast.CompressCompoundIncrement();
            if (!options.DisableVariableNameSimplification)
                ast = ast.SimplifyVariableNames();
            if (!options.DisableDeadPostGotoElimination)
                ast = ast.DeadPostGotoElimination();
            if (!options.DisableEolGotoElimination)
                ast = ast.TrailingGotoNextLineElimination();
            if (!options.DisableTrailingConditionalGotoAnyLineCompression)
                ast = ast.TrailingConditionalGotoAnyLineCompression();
            if (!options.DisableCompressConditionalAssignment)
                ast = ast.CompressConditionalAssignment();
            if (!options.DisableConstantCompression)
                ast = ast.CompressConstants();

            var output = ast.ToString();
            Console.Write(output);
        }
    }
}
