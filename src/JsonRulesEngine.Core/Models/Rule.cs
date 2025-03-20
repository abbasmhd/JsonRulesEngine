using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Represents a rule with conditions and events to trigger
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Gets the ID of the rule
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// Gets or sets the priority of the rule (lower numbers are evaluated first)
        /// </summary>
        public int Priority { get; set; } = 1;
        
        /// <summary>
        /// Gets the conditions of the rule
        /// </summary>
        public TopLevelCondition Conditions { get; }
        
        /// <summary>
        /// Gets the event to trigger when the rule's conditions are met
        /// </summary>
        public Event Event { get; }
        
        /// <summary>
        /// Initializes a new instance of the Rule class
        /// </summary>
        /// <param name="id">The ID of the rule</param>
        /// <param name="conditions">The conditions of the rule</param>
        /// <param name="event">The event to trigger when the rule's conditions are met</param>
        /// <param name="priority">The priority of the rule</param>
        public Rule(string id, TopLevelCondition conditions, Event @event, int priority = 1)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Priority = priority;
        }
        
        /// <summary>
        /// Serializes the rule to JSON
        /// </summary>
        /// <returns>The JSON representation of the rule</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        
        /// <summary>
        /// Deserializes a rule from JSON
        /// </summary>
        /// <param name="json">The JSON representation of the rule</param>
        /// <returns>The deserialized rule</returns>
        public static Rule FromJson(string json)
        {
            var rule = JsonSerializer.Deserialize<Rule>(json);
            return rule ?? throw new JsonException("Failed to deserialize rule from JSON");
        }
    }
}
