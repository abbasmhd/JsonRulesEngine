using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Represents a condition in a rule
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// Gets the fact ID
        /// </summary>
        public string Fact { get; }
        
        /// <summary>
        /// Gets the operator name
        /// </summary>
        public string Operator { get; }
        
        /// <summary>
        /// Gets the value to compare against
        /// </summary>
        public object Value { get; }
        
        /// <summary>
        /// Gets the path to resolve within the fact object
        /// </summary>
        public string? Path { get; }
        
        /// <summary>
        /// Initializes a new instance of the Condition class
        /// </summary>
        /// <param name="fact">The fact ID</param>
        /// <param name="operator">The operator name</param>
        /// <param name="value">The value to compare against</param>
        /// <param name="path">The path to resolve within the fact object</param>
        public Condition(string fact, string @operator, object value, string? path = null)
        {
            Fact = fact ?? throw new ArgumentNullException(nameof(fact));
            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            Value = value;
            Path = path;
        }
    }
}
