using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Core
{
    /// <summary>
    /// Factory class for creating rules engine components
    /// </summary>
    public static class RulesEngineFactory
    {
        /// <summary>
        /// Creates a new engine with the specified rules and options
        /// </summary>
        /// <param name="rules">The rules to add to the engine</param>
        /// <param name="options">The options for the engine</param>
        /// <returns>A new engine instance</returns>
        public static Engine CreateEngine(IEnumerable<Rule>? rules = null, EngineOptions? options = null)
        {
            return new Engine(rules, options);
        }
        
        /// <summary>
        /// Creates a new rule with the specified parameters
        /// </summary>
        /// <param name="id">The ID of the rule</param>
        /// <param name="conditions">The conditions of the rule</param>
        /// <param name="event">The event to trigger when the rule's conditions are met</param>
        /// <param name="priority">The priority of the rule</param>
        /// <returns>A new rule instance</returns>
        public static Rule CreateRule(string id, TopLevelCondition conditions, Event @event, int priority = 1)
        {
            return new Rule(id, conditions, @event, priority);
        }
        
        /// <summary>
        /// Creates a new top-level condition with the specified parameters
        /// </summary>
        /// <param name="booleanOperator">The boolean operator ("all" or "any")</param>
        /// <param name="conditions">The conditions</param>
        /// <returns>A new top-level condition instance</returns>
        public static TopLevelCondition CreateTopLevelCondition(string booleanOperator, IEnumerable<Condition> conditions)
        {
            return new TopLevelCondition(booleanOperator, conditions);
        }
        
        /// <summary>
        /// Creates a new condition with the specified parameters
        /// </summary>
        /// <param name="fact">The fact ID</param>
        /// <param name="operator">The operator name</param>
        /// <param name="value">The value to compare against</param>
        /// <param name="path">The path to resolve within the fact object</param>
        /// <returns>A new condition instance</returns>
        public static Condition CreateCondition(string fact, string @operator, object value, string? path = null)
        {
            return new Condition(fact, @operator, value, path);
        }
        
        /// <summary>
        /// Creates a new event with the specified parameters
        /// </summary>
        /// <param name="type">The type of the event</param>
        /// <param name="params">The parameters of the event</param>
        /// <returns>A new event instance</returns>
        public static Event CreateEvent(string type, IDictionary<string, object>? @params = null)
        {
            return new Event(type, @params);
        }
        
        /// <summary>
        /// Creates a new fact with a static value
        /// </summary>
        /// <param name="id">The ID of the fact</param>
        /// <param name="value">The static value</param>
        /// <param name="options">The options for the fact</param>
        /// <returns>A new fact instance</returns>
        public static Fact CreateFact(string id, object value, FactOptions? options = null)
        {
            return Fact.Create(id, value, options);
        }
        
        /// <summary>
        /// Creates a new fact with a dynamic value callback
        /// </summary>
        /// <param name="id">The ID of the fact</param>
        /// <param name="valueCallback">The value callback function</param>
        /// <param name="options">The options for the fact</param>
        /// <returns>A new fact instance</returns>
        public static Fact CreateDynamicFact(string id, Func<IDictionary<string, object>, Interfaces.IAlmanac, Task<object>> valueCallback, FactOptions? options = null)
        {
            return new Fact(id, valueCallback, options);
        }
    }
}
