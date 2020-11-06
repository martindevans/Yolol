using System.Collections.Generic;

namespace YololAssembler.Grammar.AST
{
    internal abstract class BaseDefine
        : BaseStatement
    {
        public abstract string Apply(string input);

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
