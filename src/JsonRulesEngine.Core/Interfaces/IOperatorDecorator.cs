using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Interfaces
{
    /// <summary>
    /// Interface for operator decorators that enhance operator functionality
    /// </summary>
    /// <typeparam name="TFactValue">The type of the fact value</typeparam>
    /// <typeparam name="TCompareValue">The type of the comparison value</typeparam>
    /// <typeparam name="TNextFactValue">The type of the next fact value</typeparam>
    /// <typeparam name="TNextCompareValue">The type of the next comparison value</typeparam>
    public interface IOperatorDecorator<TFactValue, TCompareValue, TNextFactValue, TNextCompareValue>
    {
        /// <summary>
        /// Evaluates the operator with additional decorator functionality
        /// </summary>
        /// <param name="factValue">The fact value</param>
        /// <param name="compareToValue">The value to compare against</param>
        /// <param name="next">The next operator evaluator in the chain</param>
        /// <returns>True if the condition is satisfied, false otherwise</returns>
        bool Evaluate(TFactValue factValue, TCompareValue compareToValue, 
            Func<TNextFactValue, TNextCompareValue, bool> next);
    }
}
