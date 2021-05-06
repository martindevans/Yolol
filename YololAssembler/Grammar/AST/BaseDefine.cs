using System.Collections.Generic;
using System.Text.RegularExpressions;
using YololAssembler.Grammar.Errors;

namespace YololAssembler.Grammar.AST
{
    internal abstract class BaseDefine
        : BaseStatement
    {
        public string Apply(string input)
        {
            var match = Regex.Match(input, FindRegex);
            if (!match.Success)
                return input;

            var r = Replace(match.Groups["body"].Value);
            r = Other.Trim(r);

            var v = input[..match.Index]
                    + r
                    + input[(0 + match.Index + match.Length)..];
            return v;
        }

        protected abstract string FindRegex { get; }

        protected abstract string Replace(string part);

        /// <summary>
        /// Apply a set of defines repeatedly until fixpoint
        /// </summary>
        /// <param name="block"></param>
        /// <param name="defines"></param>
        /// <returns></returns>
        internal static string Apply(string block, IReadOnlyList<BaseDefine> defines)
        {
            var original = block;
            var matches = 0;

            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var item in defines)
                {
                    var input = block;
                    block = item.Apply(input);

                    if (!block.Equals(input))
                    {
                        changed = true;
                        matches++;
                    }
                }

                if (matches >= 100)
                    throw new TooManySubstitutions(original, block, matches);
            }

            return block;
        }
    }
}
