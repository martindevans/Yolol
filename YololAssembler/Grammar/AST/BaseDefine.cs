using System.Collections.Generic;
using System.Text.RegularExpressions;

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

            var v = input.Substring(0, match.Index)
                    + r
                    + input.Substring(0 + match.Index + match.Length);
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
        internal static string Apply(string block, IEnumerable<BaseDefine> defines)
        {
            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var item in defines)
                {
                    var input = block;
                    block = item.Apply(input);
                    changed |= !block.Equals(input);
                }
            }

            return block;
        }
    }
}
