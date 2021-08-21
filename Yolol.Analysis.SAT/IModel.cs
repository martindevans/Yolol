using Microsoft.Z3;
using System;
using Yolol.Grammar;

namespace Yolol.Analysis.SAT
{
    public interface IModel
        : IDisposable
    {
        Solver Solver { get; }

        /// <summary>
        /// Try to get the model variable for the given variable name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IModelVariable? TryGetVariable(VariableName name);

        /// <summary>
        /// Try to get the model variable for the final goto in the program
        /// </summary>
        /// <returns></returns>
        IModelVariable? TryGetGotoVariable();

        /// <summary>
        /// Try to get the model variable for the final conditional in the program
        /// </summary>
        /// <returns></returns>
        IModelVariable? TryGetConditionalVariable();

        /// <summary>
        /// Check the solver status
        /// </summary>
        /// <returns></returns>
        Status Check();
    }
}
