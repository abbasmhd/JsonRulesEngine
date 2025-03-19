using System.Collections.Generic;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Represents an event that is triggered when a rule's conditions are met
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets the type of the event
        /// </summary>
        public string Type { get; }
        
        /// <summary>
        /// Gets the parameters of the event
        /// </summary>
        public IDictionary<string, object> Params { get; }
        
        /// <summary>
        /// Initializes a new instance of the Event class
        /// </summary>
        /// <param name="type">The type of the event</param>
        /// <param name="params">The parameters of the event</param>
        public Event(string type, IDictionary<string, object> @params = null)
        {
            Type = type;
            Params = @params ?? new Dictionary<string, object>();
        }
    }
}
