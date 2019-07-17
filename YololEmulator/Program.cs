using System;
using System.IO;
using System.Linq;
using CommandLine;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Superpower.Model;
using Yolol.Execution;
using Yolol.Grammar;
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
            public string InputFile { get; set; }

            [Option('h', "host", HelpText = "Port to host a network on", Required = false)]
            public ushort? HostPort { get; set; }

            [Option('c', "client", HelpText = "IP/Port to connect on", Required = false)]
            public string Client { get; set; }
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

                IDeviceNetwork network = new ConsoleInputDeviceNetwork();
                if (options.Client != null)
                    network = new HttpClientDeviceNetwork(options.Client);
                if (options.HostPort != null)
                    network = new HttpHostDeviceNetwork(options.HostPort.Value);

                var st = new MachineState(network, new DefaultIntrinsics());
                var pc = 0;
                while (pc <= 20)
                {
                    // Read the next line to execute from the file
                    var line = ReadLine(options.InputFile, pc);
                    Console.WriteLine($"[{pc + 1:00}] {line}");

                    // Evaluate this line
                    pc = EvaluateLine(line, pc, st);

                    // Print machine state
                    Console.WriteLine("State:");
                    foreach (var (key, value) in st)
                        Console.WriteLine($" | {key} = {value}");
                    Console.WriteLine();

                    // Pause until made to continue
                    Console.Write("Press F5 to continue");
                    while (Console.ReadKey(true).Key != ConsoleKey.F5) { }
                    Console.CursorLeft = 0;
                    Console.WriteLine(string.Join("", Enumerable.Repeat('=', Console.WindowWidth)));
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

        [NotNull] private static string ReadLine([NotNull] string filepath, int lineNumber)
        {
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number is negative");

            return File.ReadLines(filepath).Skip(lineNumber).FirstOrDefault() ?? "";
        }

        private static int EvaluateLine(string line, int pc, MachineState state)
        {
            var tokens = Tokenizer.TryTokenize(line);
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

            var parsed = Parser.TryParseLine(tokens.Value);
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

        private static void Error([NotNull] Action act)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            act();
            Console.ForegroundColor = fg;
        }
    }
}