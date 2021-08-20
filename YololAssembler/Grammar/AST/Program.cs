using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using YololAssembler.Grammar.Errors;

namespace YololAssembler.Grammar.AST
{
    internal class Program
    {
        public IReadOnlyList<BaseStatement> Statements { get; }

        public Program(IEnumerable<BaseStatement> statements)
        {
            Statements = statements.Where(a => a is not Comment).ToArray();
        }

        public string Compile()
        {
            // Apply replacements to all blocks of `Other`
            var lines = Apply(Defines(), Others().ToArray()).ToArray();

            // Replace line labels
            lines = Apply(Labels(), lines.ToArray()).ToArray();

            // Replace implicit line labels
            lines = ApplyImplicitLabels(lines).ToArray();

            // Remove all of the {} characters now that substitutions are done
            lines = lines.Select(l => l.Replace("{", "").Replace("}", "")).ToArray();

            // Run all `eval` replacements
            lines = Apply(new[] { new EvalReplacement() }, lines).ToArray();

            // Return compiled program
            return string.Join("\n", lines);
        }

        private static IEnumerable<string> ApplyImplicitLabels(IEnumerable<string> lines)
        {
            var lineNum = 1;
            foreach (var line in lines)
            {
                yield return line.Replace("@", lineNum.ToString());
                lineNum++;
            }
        }

        private static IEnumerable<string> Apply(IReadOnlyList<BaseDefine> defines, IReadOnlyList<string> blocks)
        {
            foreach (var block in blocks)
                yield return BaseDefine.Apply(block, defines);
        }

        private IReadOnlyList<BaseDefine> Labels()
        {
            return Statements
                   .OfType<LineLabel>()
                   .Select((s, i) => (s.Name, (i + 1).ToString()))
                   .Select(a => new FindAndReplace(a.Name, a.Item2))
                   .ToArray();
        }

        private IReadOnlyList<BaseDefine> Defines()
        {
            // Find all things defined in this file
            var defines = Statements.OfType<BaseDefine>();

            // Resolve imports
            var imports = Statements.OfType<Import>().SelectMany(Resolve);

            return defines.Concat(imports).ToArray();
        }

        private static IReadOnlyList<BaseDefine> Resolve(Import import)
        {
            string Fetch()
            {
                var path = Path.Combine(
                    Environment.CurrentDirectory,
                    import.Path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                );

                if (File.Exists(path))
                    return File.ReadAllText(import.Path);

                if (Uri.TryCreate(import.Path, UriKind.Absolute, out var uri))
                    using (var client = new WebClient())
                        return client.DownloadString(uri);

                throw new CannotResolveImport(import.Path);
            }

            var imported = Fetch();

            var parsed = Parser.ParseProgram(imported);
            if (!parsed.IsOk)
                throw new CannotParseImport(import.Path, parsed.Err);

            return parsed.Ok.Defines();
        }

        private IEnumerable<string> Others()
        {
            var run = new List<string>();
            foreach (var stmt in Statements.SkipWhile(a => !(a is LineLabel)))
            {
                if (stmt is Other other)
                {
                    var content = Other.Trim(other.Content);
                    run.Add(content);
                }
                else if (run.Count > 0)
                {
                    yield return string.Join("", run);
                    run.Clear();
                }
            }

            yield return string.Join("", run);
        }
    }
}
