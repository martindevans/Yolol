﻿using System;
using System.Collections.Generic;
using System.Linq;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
{
    public class StatementList
        : BaseStatement, IEquatable<StatementList>
    {
        public override bool CanRuntimeError => Statements.Any(s => s.CanRuntimeError);

        public IReadOnlyList<BaseStatement> Statements { get; }

        public StatementList()
            : this(Array.Empty<BaseStatement>())
        {
            
        }

        public StatementList(params BaseStatement[] stmts)
            : this((IEnumerable<BaseStatement>)stmts)
        {
            
        }

        public StatementList(IEnumerable<BaseStatement> statements)
        {
            Statements = statements.Where(a => a is not EmptyStatement).ToArray();
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            foreach (var statement in Statements)
            {
                var r = statement.Evaluate(state);
                switch (r.Type)
                {
                    case ExecutionResultType.Goto:
                        return r;

                    case ExecutionResultType.None:
                        continue;

                    //ncrunch: no coverage start
                    default:
                        throw new ExecutionException($"Unknown ExecutionResult `{r.Type}`");
                    //ncrunch: no coverage end

                }
            }

            return new ExecutionResult();
        }

        public bool Equals(StatementList? other)
        {
            return other != null
                && other.Statements.Count == Statements.Count
                && other.Statements.Zip(Statements, (a, b) => a.Equals(b)).All(a => a);
        }

        public override bool Equals(BaseStatement? other)
        {
            return other is StatementList sl
                && sl.Equals(this);
        }

        public override string ToString()
        {
            return string.Join(" ", Statements.Select(s => s.ToString()));
        }

        public override int GetHashCode()
        {
            return Statements.GetHashCode();
        }
    }
}
