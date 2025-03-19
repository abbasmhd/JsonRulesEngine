using System;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Interfaces
{
    /// <summary>
    /// Interface for operators that compare fact values against condition values
    /// </summary>
    /// <typeparam name="TFactValue">The type of the fact value</typeparam>
    /// <typeparam name="TCompareValue">The type of the comparison value</typeparam>
    public interface IOperator<TFactValue, TCompareValue>
    {
        /// <summary>
        /// Evaluates whether the fact value satisfies the condition based on the operator
        /// </summary>
        /// <param name="factValue">The fact value</param>
        /// <param name="compareToValue">The value to compare against</param>
        /// <returns>True if the condition is satisfied, false otherwise</returns>
        bool Evaluate(TFactValue factValue, TCompareValue compareToValue);
    }
}
