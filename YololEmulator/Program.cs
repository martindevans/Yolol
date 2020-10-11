using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using Yolol.Execution;
using YololEmulator.Network;
using YololEmulator.Network.Http;
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

            [Option('h', "host", HelpText = "Port to host a network on", Required = false)]
            public ushort? HostPort { get; set; }

            [Option('c', "client", HelpText = "IP/Port to connect on", Required = false)]
            public string? Client { get; set; }

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
                if (options.Client != null)
                    network = new HttpClientDeviceNetwork(options.Client);
                if (options.HostPort != null)
                    network = new HttpHostDeviceNetwork(options.HostPort.Value);

                var endvar = network.Get(options.EndProgramVar);
                var lines = 0;
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
                    lines++;

                    // Print machine state
                    Console.Title = $"Elapsed Game Time: {TimeSpan.FromMilliseconds(200 * lines).TotalSeconds.ToString(CultureInfo.CurrentCulture)}s";
                    Console.WriteLine("State:");
                    foreach (var (key, value) in st)
                    {
                        if (value.Value.Type == Yolol.Execution.Type.String)
                            Console.WriteLine($" | {key} = \"{value}\"");
                        else
                            Console.WriteLine($" | {key} = {value}");
                    }

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
                        // Pause until made to continue
                        Console.Write("Press F5 to continue");
                        while (Console.ReadKey(true).Key != ConsoleKey.F5) { }
                    }
                    Console.CursorLeft = 0;
                    Console.WriteLine(string.Join("", Enumerable.Repeat('=', Console.WindowWidth)));
                }

                Console.WriteLine($"{TimeSpan.FromMilliseconds(200 * lines).TotalSeconds.ToString(CultureInfo.CurrentCulture)}s");
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
            var result = Parser.ParseProgram(line);
            if (!result.IsOk)
            {
                Console.WriteLine();
                Console.Error.Write(result.Err.ToString());

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
                var c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Runtime Error: {ee.Message}");
                Console.ForegroundColor = c;
                return pc + 1;
            }
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