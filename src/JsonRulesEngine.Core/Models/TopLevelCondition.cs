using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Represents a top-level condition that can contain nested conditions with boolean operators
    /// </summary>
    public class TopLevelCondition
    {
        /// <summary>
        /// Gets the boolean operator ("all" or "any")
        /// </summary>
        public string BooleanOperator { get; }
        
        /// <summary>
        /// Gets the conditions
        /// </summary>
        public IEnumerable<Condition> Conditions { get; }
        
        /// <summary>
        /// Initializes a new instance of the TopLevelCondition class
        /// </summary>
        /// <param name="booleanOperator">The boolean operator ("all" or "any")</param>
        /// <param name="conditions">The conditions</param>
        public TopLevelCondition(string booleanOperator, IEnumerable<Condition> conditions)
        {
            if (booleanOperator != "all" && booleanOperator != "any")
                throw new ArgumentException("Boolean operator must be 'all' or 'any'", nameof(booleanOperator));
                
            BooleanOperator = booleanOperator;
            Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
        }
    }
}
