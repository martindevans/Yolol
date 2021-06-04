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
        // ReSharper disable once ClassNeverInstantiated.Local
        private class Options
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [Option('i', "input", HelpText = "File/Folder to read code from", Required = true)]
            public string InputPath { get; set; } = null!;

            [Option('w', "watch", HelpText = "If set, the assembler will automatically run every time the input file/folder changes", Required = false)]
            public bool Watch { get; set; }

            [Option('m', "minimal", HelpText = "When set, console output will be less verbose", Required = false)]
            public bool Minimal { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            var input = options.InputPath!;

            if (File.Exists(input))
                ProcessFile(options.InputPath!, Path.GetFileNameWithoutExtension(options.InputPath) + ".yolol", options.Minimal);
            else if (Directory.Exists(input))
            {
                var children = Directory.EnumerateFiles(options.InputPath, "*.yasm", new EnumerationOptions { RecurseSubdirectories = true });
                foreach (var child in children)
                {
                    ProcessFile(child, Path.GetFileNameWithoutExtension(child) + ".yolol", options.Minimal);
                }
            }

            if (options.Watch)
            {
                using var watcher = CreateWatcher(options.InputPath);

                void OnChanged(object sender, FileSystemEventArgs args)
                {
                    // Delay for a while to try and prevent the text editor and the compiler from
                    // both accessing the file at the same time.
                    Thread.Sleep(100);

                    var i = new FileInfo(args.FullPath);
                    var o = Path.Combine(i.Directory!.FullName, Path.GetFileNameWithoutExtension(i.FullName) + ".yolol");

                    ProcessFile(i.FullName, o, options.Minimal);
                }

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                Console.WriteLine("Waiting for file changes...");
                Console.WriteLine("Press 'q' to quit.");
                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                    Thread.Sleep(100);
            }
        }

        private static FileSystemWatcher CreateWatcher(string path)
        {
            if (File.Exists(path))
            {
                Console.WriteLine($"Watching for file changes to `{Path.GetFullPath(path)}`");
                return new FileSystemWatcher {
                    Path = Path.GetDirectoryName(Path.GetFullPath(path))!,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    IncludeSubdirectories = false,
                    Filter = Path.GetFileName(path),
                };
            }
            else
            {
                Console.WriteLine($"Watching for directory changes in `{Path.GetFullPath(path)}`");
                return new FileSystemWatcher {
                    Path = Path.GetDirectoryName(Path.GetFullPath(path))!,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    IncludeSubdirectories = true,
                    Filter = Path.GetFileName("*.yasm"),
                };
            }
        }

        private static void ProcessFile(string inputPath, string outputPath, bool minimalOutput)
        {
            Console.WriteLine($"Compiling {inputPath}");

            var timer = new Stopwatch();
            timer.Start();

            // Keep retrying the operation 10 times, waiting slightly longer each time. This works around the
            // file lock still (sometimes) being held by the editer that just modified the file when the
            // notification about the change comes in.
            for (var i = 1; i <= 10; i++)
            {
                Thread.Sleep(20 * i);
                try
                {
                    var parseResult = Grammar.Parser.ParseProgram(File.ReadAllText(inputPath));
                    if (!parseResult.IsOk)
                    {
                        if (minimalOutput) {
                            Console.WriteLine($"{inputPath} @ {parseResult.Err.Cursor.Line}:{parseResult.Err.Cursor.Column-1} : {parseResult.Err.Message}");
                        } else {
                            Console.WriteLine($"# Yasm Parse Error");
                            Console.WriteLine("------------------");
                            Console.WriteLine(parseResult.Err.ToString());
                        }
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
                    if (minimalOutput) {
                        Console.WriteLine($"{inputPath} @ 0:0 : {e.Message}");
                    } else {
                        Console.WriteLine($"# YASM Compiler Error");
                        Console.WriteLine("---------------------");
                        Console.WriteLine(e);
                        Console.WriteLine();
                    }
                    break;
                }
            }

            Console.WriteLine($"Done {inputPath}=>{outputPath} ({timer.ElapsedMilliseconds}ms)");
        }
    }
}
