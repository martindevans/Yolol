using System;
using System.IO;
using System.Linq;
using CommandLine;
using Superpower.Model;
using YololEmulator.Execution;
using Parser = YololEmulator.Grammar.Parser;

namespace YololEmulator
{
    internal class Program
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class Options
        {
            [Option('c', "code", HelpText = "File to read YOLOL code from", Required = true)]
            
            public string InputFile { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            try
            {
                if (!File.Exists(options.InputFile))
                {
                    Console.Error.WriteLine($"Input file `{options.InputFile}` does not exist");
                    return;
                }

                var st = new MachineState();
                var pc = 0;
                while (pc <= 20)
                {
                    // Read the next line to execute from the file
                    var line = ReadLine(options.InputFile, pc);
                    Console.WriteLine($"[{pc:00}] {line}");

                    // Evaluate this line
                    pc = EvaluateLine(line, pc, st);

                    // Print machine state
                    Console.WriteLine("State:");
                    st.Print("| ");
                    Console.WriteLine();

                    // Pause until made to continue
                    while (Console.ReadKey(true).Key != ConsoleKey.F5)
                        continue;
                }
            }
            catch (Exception e)
            {
                Error(() => {
                    Console.WriteLine();
                    Console.WriteLine("Fatal Exception:");
                    Console.Error.WriteLine(e);
                });
            }
        }

        private static string ReadLine(string filepath, int lineNumber)
        {
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number is negative");

            return File.ReadLines(filepath).Skip(lineNumber).FirstOrDefault() ?? "";
        }

        private static int EvaluateLine(string line, int pc, MachineState state)
        {
            var tokens = Parser.TryTokenize(line);
            if (!tokens.HasValue)
            {
                Error(() => {
                    ErrorSpan(tokens.Location);
                    Console.WriteLine();

                    Console.Error.WriteLine(tokens.FormatErrorMessageFragment());
                });

                Console.WriteLine("Press any key to try this line again");
                Console.ReadKey(true);
                return pc;
            }

            var parsed = Parser.TryParse(tokens.Value);
            if (!parsed.HasValue)
            {
                Error(() => {
                    ErrorSpan(tokens.Location);
                    Console.WriteLine();

                    Console.Error.WriteLine(parsed.FormatErrorMessageFragment());
                });

                Console.WriteLine("Press any key to try this line again");
                Console.ReadKey(true);
                return pc;
            }

            if (parsed.Remainder.Any())
            {
                Error(() => {
                    Console.WriteLine("Failed to parse entire line");
                    foreach (var token in parsed.Remainder)
                        Console.WriteLine($" - {token}");
                });

                Console.WriteLine("Press any key to try this line again");
                Console.ReadKey(true);
                return pc;
            }

            return parsed.Value.Evaluate(pc, state);
        }

        private static void ErrorSpan(TextSpan location)
        {
            for (var i = 0; i < location.Position.Column + 4; i++)
                Console.Write('-');
            for (var i = 0; i < location.Length; i++)
                Console.Error.Write('^');
        }

        private static void Error(Action act)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            act();
            Console.ForegroundColor = fg;
        }
    }
}