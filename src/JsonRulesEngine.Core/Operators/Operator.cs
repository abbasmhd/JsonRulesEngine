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
        public abstract bool Evaluate(object? factValue, object? compareToValue);
    }
    
    /// <summary>
    /// Equal operator
    /// </summary>
    public class EqualOperator : Operator, IOperator<object, object>
    {
        /// <inheritdoc/>
        public override string Name => "equal";
        
        /// <inheritdoc/>
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null && compareToValue == null)
                return true;
                
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.Equals(compareToValue);
        }
        
        bool IOperator<object, object>.Evaluate(object? factValue, object? compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null && compareToValue == null)
                return false;
                
            if (factValue == null || compareToValue == null)
                return true;
                
            return !factValue.Equals(compareToValue);
        }
        
        bool IOperator<object, object>.Evaluate(object? factValue, object? compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (compareToValue is IEnumerable<object> collection)
                return collection.Contains(factValue);
                
            return false;
        }
        
        bool IOperator<object, IEnumerable<object>>.Evaluate(object? factValue, IEnumerable<object> compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return true;
                
            if (compareToValue is IEnumerable<object> collection)
                return !collection.Contains(factValue);
                
            return true;
        }
        
        bool IOperator<object, IEnumerable<object>>.Evaluate(object? factValue, IEnumerable<object> compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is IEnumerable<object> collection)
                return collection.Contains(compareToValue);
                
            return false;
        }
        
        bool IOperator<IEnumerable<object>, object>.Evaluate(IEnumerable<object> factValue, object? compareToValue)
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
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null)
                return true;
                
            if (compareToValue == null)
                return true;
                
            if (factValue is IEnumerable<object> collection)
                return !collection.Contains(compareToValue);
                
            return true;
        }
        
        bool IOperator<IEnumerable<object>, object>.Evaluate(IEnumerable<object> factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return true;
                
            return !factValue.Contains(compareToValue);
        }
    }
    
    /// <summary>
    /// Starts with operator (checks if a string starts with another string)
    /// </summary>
    public class StartsWithOperator : Operator, IOperator<string, string>
    {
        /// <inheritdoc/>
        public override string Name => "startsWith";
        
        /// <summary>
        /// Evaluates whether a string starts with the specified substring.
        /// Uses <see cref="String.StartsWith(string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="factValue">The fact value (string to check)</param>
        /// <param name="compareToValue">The substring to check for at the start of the string</param>
        /// <returns>
        /// True if <paramref name="factValue"/> starts with <paramref name="compareToValue"/>;
        /// False if either value is null or if the fact value doesn't start with the comparison value.
        /// Returns true if both strings are empty or if comparison value is empty.
        /// </returns>
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is string factString && compareToValue is string compareString)
                return factString.StartsWith(compareString, StringComparison.Ordinal);
                
            return false;
        }
        
        bool IOperator<string, string>.Evaluate(string factValue, string compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.StartsWith(compareToValue, StringComparison.Ordinal);
        }
    }
    
    /// <summary>
    /// Ends with operator (checks if a string ends with another string)
    /// </summary>
    public class EndsWithOperator : Operator, IOperator<string, string>
    {
        /// <inheritdoc/>
        public override string Name => "endsWith";
        
        /// <summary>
        /// Evaluates whether a string ends with the specified substring.
        /// Uses <see cref="String.EndsWith(string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="factValue">The fact value (string to check)</param>
        /// <param name="compareToValue">The substring to check for at the end of the string</param>
        /// <returns>
        /// True if <paramref name="factValue"/> ends with <paramref name="compareToValue"/>;
        /// False if either value is null or if the fact value doesn't end with the comparison value.
        /// Returns true if both strings are empty or if comparison value is empty.
        /// </returns>
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is string factString && compareToValue is string compareString)
                return factString.EndsWith(compareString, StringComparison.Ordinal);
                
            return false;
        }
        
        bool IOperator<string, string>.Evaluate(string factValue, string compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.EndsWith(compareToValue, StringComparison.Ordinal);
        }
    }
    
    /// <summary>
    /// String contains operator (checks if a string contains another string)
    /// </summary>
    public class StringContainsOperator : Operator, IOperator<string, string>
    {
        /// <inheritdoc/>
        public override string Name => "stringContains";
        
        /// <summary>
        /// Evaluates whether a string contains the specified substring.
        /// Uses <see cref="String.Contains(string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="factValue">The fact value (string to check)</param>
        /// <param name="compareToValue">The substring to check for within the string</param>
        /// <returns>
        /// True if <paramref name="factValue"/> contains <paramref name="compareToValue"/>;
        /// False if either value is null or if the fact value doesn't contain the comparison value.
        /// Returns true if both strings are empty or if comparison value is empty.
        /// </returns>
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is string factString && compareToValue is string compareString)
                return factString.Contains(compareString, StringComparison.Ordinal);
                
            return false;
        }
        
        bool IOperator<string, string>.Evaluate(string factValue, string compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            return factValue.Contains(compareToValue, StringComparison.Ordinal);
        }
    }
    
    /// <summary>
    /// Matches operator (checks if a string matches a regular expression)
    /// </summary>
    public class MatchesOperator : Operator, IOperator<string, string>
    {
        /// <inheritdoc/>
        public override string Name => "matches";
        
        /// <summary>
        /// Evaluates whether a string matches the specified regular expression pattern.
        /// Uses <see cref="System.Text.RegularExpressions.Regex.IsMatch(string, string)"/> to perform the matching.
        /// </summary>
        /// <param name="factValue">The fact value (string to check)</param>
        /// <param name="compareToValue">The regular expression pattern to match against</param>
        /// <returns>
        /// True if <paramref name="factValue"/> matches the pattern in <paramref name="compareToValue"/>;
        /// False if either value is null, if the fact value doesn't match the pattern, or if the pattern is invalid.
        /// Invalid regex patterns are handled gracefully (returns false instead of throwing an exception).
        /// </returns>
        public override bool Evaluate(object? factValue, object? compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            if (factValue is string factString && compareToValue is string pattern)
            {
                try 
                {
                    return System.Text.RegularExpressions.Regex.IsMatch(factString, pattern);
                }
                catch (System.Text.RegularExpressions.RegexParseException)
                {
                    return false;
                }
            }
                
            return false;
        }
        
        bool IOperator<string, string>.Evaluate(string factValue, string compareToValue)
        {
            if (factValue == null || compareToValue == null)
                return false;
                
            try 
            {
                return System.Text.RegularExpressions.Regex.IsMatch(factValue, compareToValue);
            }
            catch (System.Text.RegularExpressions.RegexParseException)
            {
                return false;
            }
        }
    }
}
