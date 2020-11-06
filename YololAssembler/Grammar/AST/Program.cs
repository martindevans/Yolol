using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using YololAssembler.Grammar.Errors;
using Result = Yolol.Grammar.Parser.Result<string, Yolol.Grammar.Parser.ParseError>;

namespace YololAssembler.Grammar.AST
{
    internal class Program
    {
        public IReadOnlyList<BaseStatement> Statements { get; }

        public Program(IEnumerable<BaseStatement> statements)
        {
            Statements = statements.ToArray();
        }

        public Result Compile(bool compress = true)
        {
            // Apply replacements to all blocks of `Other`
            var lines = Apply(Defines().ToArray(), Others().ToArray()).ToArray();

            // Replace line labels
            lines = Apply(Labels().ToArray(), lines.ToArray()).ToArray();

            // Early out if compression should not be applied
            var yolol = string.Join("\n", lines);
            if (!compress)
                return new Result(yolol);

            // Parse as yolol to apply compression
            var parsedYolol = Yolol.Grammar.Parser.ParseProgram(yolol);
            if (!parsedYolol.IsOk)
                return new Result(parsedYolol.Err);

            // remove unnecessary spaces from the program
            return new Result(Compress(parsedYolol.Ok));
        }

        private static string Compress(Yolol.Grammar.AST.Program yolol)
        {
            //todo: compress
            return yolol.ToString();
        }

        private static IEnumerable<string> Apply(IReadOnlyList<BaseDefine> defines, IReadOnlyList<string> blocks)
        {
            foreach (var block in blocks)
                yield return BaseDefine.Apply(block, defines);
        }

        private IEnumerable<BaseDefine> Labels()
        {
            return Statements
                   .OfType<LineLabel>()
                   .Select((s, i) => (s.Name, (i + 1).ToString()))
                   .Select(a => new FindAndReplace(a.Name, a.Item2));
        }

        private IEnumerable<BaseDefine> Defines()
        {
            // Find all things defined in this file
            var defines = Statements.OfType<BaseDefine>();

            // Resolve imports
            var imports = Statements.OfType<Import>().SelectMany(Resolve);

            return defines.Concat(imports);
        }

        private static IEnumerable<BaseDefine> Resolve(Import import)
        {
            string Fetch()
            {
                if (File.Exists(import.Path))
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
