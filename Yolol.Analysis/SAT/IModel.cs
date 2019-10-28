using JetBrains.Annotations;
using Microsoft.Z3;
using System;
using Yolol.Grammar;

namespace Yolol.Analysis.SAT
{
    public interface IModel
        : IDisposable
    {
        [CanBeNull] IModelVariable TryGetVariable(VariableName name);
    }
}
