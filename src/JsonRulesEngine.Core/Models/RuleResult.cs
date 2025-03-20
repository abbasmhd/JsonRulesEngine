using System.Collections.Generic;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Represents the result of a rule evaluation
    /// </summary>
    public class RuleResult
    {
        /// <summary>
        /// Gets the rule that was evaluated
        /// </summary>
        public Rule Rule { get; }
        
        /// <summary>
        /// Gets whether the rule's conditions were met
        /// </summary>
        public bool Result { get; }
        
        /// <summary>
        /// Gets the event that was triggered (if any)
        /// </summary>
        public Event? Event { get; }
        
        /// <summary>
        /// Initializes a new instance of the RuleResult class
        /// </summary>
        /// <param name="rule">The rule that was evaluated</param>
        /// <param name="result">Whether the rule's conditions were met</param>
        /// <param name="event">The event that was triggered (if any)</param>
        public RuleResult(Rule rule, bool result, Event? @event = null)
        {
            Rule = rule;
            Result = result;
            Event = @event;
        }
    }
}
