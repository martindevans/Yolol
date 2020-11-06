using System;
using System.IO;
using System.Linq;
using CommandLine;
using Yolol.Execution;
using YololEmulator.Network;
using Parser = Yolol.Grammar.Parser;

namespace YololEmulator
{
    internal class Program
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class Options
        {
            [Option('i', "input", HelpText = "File to read YOLOL code from", Required = true)]
            #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
            public string InputFile { get; set; }
            #pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

            [Option('m', "max_line", HelpText = "Set the max line number", Required = false, Default = (ushort)20)]
            public ushort MaxLineNumber { get; set; }

            [Option('a', "auto", HelpText = "Automaticaly run each line", Required = false)]
            public bool Auto { get; set; }

            [Option('d', "delay", HelpText = "Set how long to wait (in ms) between automatically running lines", Required = false, Default = (ushort)200)]
            public ushort Delay { get; set; }

            [Option('s', "save", HelpText = "Values written to externals will be saved and automatically returned as the value next time", Required = false, Default = false)]
            public bool SaveOutputs { get; set; }

            [Option('e', "end_program", HelpText = "Set the name of the variable which ends the program", Required = false, Default = "done")]
            public string EndProgramVar { get; set; } = "done";
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
                if (string.IsNullOrWhiteSpace(options.InputFile) || !File.Exists(options.InputFile))
                {
                    Console.Error.WriteLine($"Input file `{options.InputFile}` does not exist");
                    return;
                }

                IDeviceNetwork network = new ConsoleInputDeviceNetwork(options.SaveOutputs, options.EndProgramVar);

                var endvar = network.Get(options.EndProgramVar);
                var st = new MachineState(network, options.MaxLineNumber);
                var pc = 0;
                while (pc <= options.MaxLineNumber)
                {
                    if (endvar.Value.ToBool())
                        break;

                    // Read the next line to execute from the file
                    var line = ReadLine(options.InputFile, pc);
                    Console.WriteLine($"[{pc + 1:00}] {line}");

                    // Evaluate this line
                    pc = EvaluateLine(line, pc, st);

                    // Print machine state
                    DisplayState(st);
                    Console.WriteLine();

                    if (options.Auto)
                    {
                        // Delay execution of the next line
                        System.Threading.Thread.Sleep(options.Delay);

                        // Press F5 to halt execution
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.F5)
                            break;
                    }
                    else
                    {
                        // Pause until user presses F5
                        Console.Write("Press F5 to continue");
                        while (Console.ReadKey(true).Key != ConsoleKey.F5) { }
                        Console.CursorLeft = 0;
                        Console.WriteLine(new string(' ', Console.WindowWidth));
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                DrawLine("Fatal Exception:", ConsoleColor.Red);
                DrawLine(e.ToString(), ConsoleColor.Red);
            }
        }

        private static void DisplayState(MachineState state)
        {
            Console.WriteLine("State:");
            foreach (var (key, value) in state.OrderBy(a => a.Key))
            {
                if (value.Value.Type == Yolol.Execution.Type.String)
                    Console.WriteLine($" | {key} = \"{value}\"");
                else
                    Console.WriteLine($" | {key} = {value}");
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
            var result = Parser.ParseProgram(line);
            if (!result.IsOk)
            {
                Console.WriteLine();

                DrawLine(result.Err.ToString(), ConsoleColor.Red);
                Console.WriteLine();
                Console.WriteLine("Press any key to try this line again");
                Console.ReadKey(true);
                return pc;
            }

            try
            {
                return result.Ok.Lines[0].Evaluate(pc, state);
            }
            catch (ExecutionException ee)
            {
                DrawLine($"Runtime Error: {ee.Message}", ConsoleColor.DarkYellow);
                return pc + 1;
            }
        }

        private static void DrawLine(string message, ConsoleColor color)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = fg;
        }
    }
}