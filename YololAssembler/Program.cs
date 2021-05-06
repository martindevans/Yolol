using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CommandLine;
using YololAssembler.Grammar.Errors;

namespace YololAssembler
{
    internal class Program
    {
        private class Options
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [Option('i', "input", HelpText = "File to read code from", Required = true)]
            public string? InputFile { get; set; }

            [Option('o', "output", HelpText = "File to write YOLOL code to", Required = true)]
            public string? OutputFile { get; set; }

            [Option('w', "watch", HelpText = "If set, the assembler will automatically run every time the input file changes", Required = false)]
            public bool Watch { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            var input = options.InputFile!;
            var output = options.OutputFile!;

            ProcessFile(options.InputFile!, options.OutputFile!);

            if (options.Watch)
            {
                Console.WriteLine($"Watching for changes in `{Path.GetFullPath(input)}`");

                using var watcher = new FileSystemWatcher {
                    Path = Path.GetDirectoryName(Path.GetFullPath(input)),
                    NotifyFilter = NotifyFilters.LastWrite
                                 | NotifyFilters.Size,
                    Filter = Path.GetFileName(options.InputFile),
                };

                void OnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
                {
                    Thread.Sleep(100);
                    ProcessFile(input, output);
                }

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                Console.WriteLine("Waiting for file changes...");
                Console.WriteLine("Press 'q' to quit. Press any other key to recompile.");
                while (Console.ReadKey(true).Key != ConsoleKey.Q && File.Exists(options.InputFile))
                {
                    Thread.Sleep(100);
                    ProcessFile(input, output);
                }
            }
        }

        private static void ProcessFile(string inputPath, string outputPath)
        {
            var timer = new Stopwatch();
            timer.Start();

            // Keep retrying the operation 10 times, waiting slightly longer each time. This works around the
            // file lock still (sometimes) being held by the editer that just modified the file when the
            // notification about the change comes in.
            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(10 * i);
                try
                {
                    var parseResult = Grammar.Parser.ParseProgram(File.ReadAllText(inputPath));
                    if (!parseResult.IsOk)
                    {
                        Console.WriteLine("# Yasm Parse Error");
                        Console.WriteLine("------------------");
                        Console.WriteLine(parseResult.Err.ToString());
                        break;
                    }

                    var yolol = parseResult.Ok.Compile();
                    yolol += "\n\n";
                    yolol += "// <-------------- this line is 70 characters long ------------------>";

                    File.WriteAllText(outputPath, yolol);
                    break;
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                }
                catch (BaseCompileException e)
                {
                    Console.WriteLine("# YASM Compiler Error");
                    Console.WriteLine("---------------------");
                    Console.WriteLine(e);
                    Console.WriteLine();
                    break;
                }
            }

            Console.WriteLine($"Done ({timer.ElapsedMilliseconds}ms)");
        }
    }
}
