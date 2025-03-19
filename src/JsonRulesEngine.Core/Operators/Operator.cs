using System;
using System.Collections.Generic;
using System.Linq;
using JsonRulesEngine.Core.Interfaces;

namespace JsonRulesEngine.Core.Operators
{
    /// <summary>
    /// Base class for operators that compare fact values against condition values
    /// </summary>
    public abstract class Operator
    {
        /// <summary>
        /// Gets the name of the operator
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// Evaluates whether the fact value satisfies the condition based on the operator
        /// </summary>
        /// <param name="factValue">The fact value</param>
        /// <param name="compareToValue">The value to compare against</param>
        /// <returns>True if the condition is satisfied, false otherwise</returns>
        public abstract bool Evaluate(object factValue, object compareToValue);
    }
    
    /// <summary>
    /// Equal operator
    /// </summary>
    public class EqualOperator : Operator, IOperator<object, object>
    {
        /// <inheritdoc/>
        public override string Name => "equal";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null && compareToValue == null)
                return true;
                
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.Equals(compareToValue);
        }
        
        bool IOperator<object, object>.Evaluate(object factValue, object compareToValue)
        {
            return Evaluate(factValue, compareToValue);
        }
    }
    
    /// <summary>
    /// Not equal operator
    /// </summary>
    public class NotEqualOperator : Operator, IOperator<object, object>
    {
        /// <inheritdoc/>
        public override string Name => "notEqual";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null && compareToValue == null)
                return false;
                
            if (factValue == null || compareToValue == null)
                return true;
                
            return !factValue.Equals(compareToValue);
        }
        
        bool IOperator<object, object>.Evaluate(object factValue, object compareToValue)
        {
            return Evaluate(factValue, compareToValue);
        }
    }
    
    /// <summary>
    /// Greater than operator
    /// </summary>
    public class GreaterThanOperator : Operator, IOperator<IComparable, IComparable>
    {
        /// <inheritdoc/>
        public override string Name => "greaterThan";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is IComparable comparable && compareToValue is IComparable compareTo)
                return comparable.CompareTo(compareTo) > 0;
                
            return false;
        }
        
        bool IOperator<IComparable, IComparable>.Evaluate(IComparable factValue, IComparable compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.CompareTo(compareToValue) > 0;
        }
    }
    
    /// <summary>
    /// Greater than or equal operator
    /// </summary>
    public class GreaterThanOrEqualOperator : Operator, IOperator<IComparable, IComparable>
    {
        /// <inheritdoc/>
        public override string Name => "greaterThanInclusive";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is IComparable comparable && compareToValue is IComparable compareTo)
                return comparable.CompareTo(compareTo) >= 0;
                
            return false;
        }
        
        bool IOperator<IComparable, IComparable>.Evaluate(IComparable factValue, IComparable compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.CompareTo(compareToValue) >= 0;
        }
    }
    
    /// <summary>
    /// Less than operator
    /// </summary>
    public class LessThanOperator : Operator, IOperator<IComparable, IComparable>
    {
        /// <inheritdoc/>
        public override string Name => "lessThan";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is IComparable comparable && compareToValue is IComparable compareTo)
                return comparable.CompareTo(compareTo) < 0;
                
            return false;
        }
        
        bool IOperator<IComparable, IComparable>.Evaluate(IComparable factValue, IComparable compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.CompareTo(compareToValue) < 0;
        }
    }
    
    /// <summary>
    /// Less than or equal operator
    /// </summary>
    public class LessThanOrEqualOperator : Operator, IOperator<IComparable, IComparable>
    {
        /// <inheritdoc/>
        public override string Name => "lessThanInclusive";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is IComparable comparable && compareToValue is IComparable compareTo)
                return comparable.CompareTo(compareTo) <= 0;
                
            return false;
        }
        
        bool IOperator<IComparable, IComparable>.Evaluate(IComparable factValue, IComparable compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.CompareTo(compareToValue) <= 0;
        }
    }
    
    /// <summary>
    /// In operator (checks if a value is in a collection)
    /// </summary>
    public class InOperator : Operator, IOperator<object, IEnumerable<object>>
    {
        /// <inheritdoc/>
        public override string Name => "in";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (compareToValue is IEnumerable<object> collection)
                return collection.Contains(factValue);
                
            return false;
        }
        
        bool IOperator<object, IEnumerable<object>>.Evaluate(object factValue, IEnumerable<object> compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return compareToValue.Contains(factValue);
        }
    }
    
    /// <summary>
    /// Not in operator (checks if a value is not in a collection)
    /// </summary>
    public class NotInOperator : Operator, IOperator<object, IEnumerable<object>>
    {
        /// <inheritdoc/>
        public override string Name => "notIn";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return true;
                
            if (compareToValue is IEnumerable<object> collection)
                return !collection.Contains(factValue);
                
            return true;
        }
        
        bool IOperator<object, IEnumerable<object>>.Evaluate(object factValue, IEnumerable<object> compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return true;
                
            return !compareToValue.Contains(factValue);
        }
    }
    
    /// <summary>
    /// Contains operator (checks if a collection contains a value)
    /// </summary>
    public class ContainsOperator : Operator, IOperator<IEnumerable<object>, object>
    {
        /// <inheritdoc/>
        public override string Name => "contains";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is IEnumerable<object> collection)
                return collection.Contains(compareToValue);
                
            return false;
        }
        
        bool IOperator<IEnumerable<object>, object>.Evaluate(IEnumerable<object> factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.Contains(compareToValue);
        }
    }
    
    /// <summary>
    /// Does not contain operator (checks if a collection does not contain a value)
    /// </summary>
    public class DoesNotContainOperator : Operator, IOperator<IEnumerable<object>, object>
    {
        /// <inheritdoc/>
        public override string Name => "doesNotContain";
        
        /// <inheritdoc/>
        public override bool Evaluate(object factValue, object compareToValue)
        {
            if (factValue == null)
                return true;
                
            if (compareToValue == null)
                return true;
                
            if (factValue is IEnumerable<object> collection)
                return !collection.Contains(compareToValue);
                
            return true;
        }
        
        bool IOperator<IEnumerable<object>, object>.Evaluate(IEnumerable<object> factValue, object compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return true;
                
            return !factValue.Contains(compareToValue);
        }
    }
}
