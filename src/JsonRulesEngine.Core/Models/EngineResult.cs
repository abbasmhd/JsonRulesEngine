using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Represents the result of running the engine
    /// </summary>
    public class EngineResult
    {
        /// <summary>
        /// Gets the events that were triggered
        /// </summary>
        public IEnumerable<Event> Events { get; }
        
        /// <summary>
        /// Gets the events that failed to trigger
        /// </summary>
        public IEnumerable<Event> FailureEvents { get; }
        
        /// <summary>
        /// Gets the almanac used during rule evaluation
        /// </summary>
        public Interfaces.IAlmanac Almanac { get; }
        
        /// <summary>
        /// Gets the results of rule evaluations
        /// </summary>
        public IEnumerable<RuleResult> Results { get; }
        
        /// <summary>
        /// Gets the results of failed rule evaluations
        /// </summary>
        public IEnumerable<RuleResult> FailureResults { get; }
        
        /// <summary>
        /// Initializes a new instance of the EngineResult class
        /// </summary>
        /// <param name="events">The events that were triggered</param>
        /// <param name="failureEvents">The events that failed to trigger</param>
        /// <param name="almanac">The almanac used during rule evaluation</param>
        /// <param name="results">The results of rule evaluations</param>
        /// <param name="failureResults">The results of failed rule evaluations</param>
        public EngineResult(
            IEnumerable<Event> events,
            IEnumerable<Event> failureEvents,
            Interfaces.IAlmanac almanac,
            IEnumerable<RuleResult> results,
            IEnumerable<RuleResult> failureResults)
        {
            Events = events;
            FailureEvents = failureEvents;
            Almanac = almanac;
            Results = results;
            FailureResults = failureResults;
        }
    }
}
