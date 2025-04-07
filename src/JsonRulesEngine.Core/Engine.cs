using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonRulesEngine.Core.Interfaces;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Core
{
    /// <summary>
    /// Implementation of the Engine which evaluates rules
    /// </summary>
    public class Engine : IEngine
    {
        private readonly List<Rule> _rules;
        private readonly OperatorRegistry _operatorRegistry;
        private readonly EngineOptions _options;
        private bool _running;

        /// <summary>
        /// Initializes a new instance of the Engine class
        /// </summary>
        /// <param name="rules">The rules to add to the engine</param>
        /// <param name="options">The options for the engine</param>
        public Engine(IEnumerable<Rule>? rules = null, EngineOptions? options = null)
        {
            _rules = rules?.ToList() ?? new List<Rule>();
            _operatorRegistry = new OperatorRegistry();
            _options = options ?? new EngineOptions
            {
                AllowUndefinedFacts = false,
                AllowUndefinedConditions = false,
                ReplaceFactsInEventParams = true,
                PathResolver = new JsonPathResolver()
            };
            _running = false;
        }

        /// <summary>
        /// Adds a rule to the engine
        /// </summary>
        /// <param name="rule">The rule to add</param>
        public void AddRule(Rule? rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _rules.Add(rule);
        }

        /// <summary>
        /// Updates an existing rule
        /// </summary>
        /// <param name="rule">The rule to update</param>
        public void UpdateRule(Rule? rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            var existingRule = _rules.FirstOrDefault(r => r.Id == rule.Id);
            if (existingRule != null)
            {
                _rules.Remove(existingRule);
                _rules.Add(rule);
            }
            else
            {
                throw new KeyNotFoundException($"Rule '{rule.Id}' not found in engine");
            }
        }

        /// <summary>
        /// Removes a rule by instance
        /// </summary>
        /// <param name="rule">The rule to remove</param>
        /// <returns>True if the rule was removed, false otherwise</returns>
        public bool RemoveRule(Rule? rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            return _rules.Remove(rule);
        }

        /// <summary>
        /// Removes a rule by ID
        /// </summary>
        /// <param name="ruleId">The ID of the rule to remove</param>
        /// <returns>True if the rule was removed, false otherwise</returns>
        public bool RemoveRule(string? ruleId)
        {
            if (string.IsNullOrEmpty(ruleId))
                throw new ArgumentNullException(nameof(ruleId));

            var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
            if (rule != null)
                return _rules.Remove(rule);

            return false;
        }

        /// <summary>
        /// Runs the engine with the provided facts
        /// </summary>
        /// <param name="facts">The facts to evaluate rules against</param>
        /// <param name="options">Optional run options</param>
        /// <returns>The engine result</returns>
        public async Task<EngineResult> Run(IDictionary<string, object>? facts = null, RunOptions? options = null)
        {
            if (_running)
                throw new InvalidOperationException("Engine is already running");

            _running = true;

            try
            {
                // Create almanac
                var almanac = options?.Almanac ?? new Almanac(null, new AlmanacOptions
                {
                    AllowUndefinedFacts = _options.AllowUndefinedFacts,
                    EnableFactCaching = _options.EnableFactCaching,
                    PathResolver = _options.PathResolver
                });

                // Add facts to almanac
                if (facts != null)
                {
                    foreach (var fact in facts)
                    {
                        almanac.AddRuntimeFact(fact.Key, fact.Value);
                    }
                }

                // Sort rules by priority
                var sortedRules = _rules.OrderBy(r => r.Priority).ToList();

                var events = new List<Event>();
                var failureEvents = new List<Event>();
                var results = new List<RuleResult>();
                var failureResults = new List<RuleResult>();

                // Evaluate each rule
                foreach (var rule in sortedRules)
                {
                    if (!_running)
                        break;

                    var result = await EvaluateRule(rule, almanac);

                    if (result.Result && result.Event != null)
                    {
                        events.Add(result.Event);
                        results.Add(result);
                    }
                    else
                    {
                        failureResults.Add(result);
                    }
                }

                return new EngineResult(events, failureEvents, almanac, results, failureResults);
            }
            finally
            {
                _running = false;
            }
        }

        /// <summary>
        /// Stops the engine
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// Evaluates a rule against the almanac
        /// </summary>
        /// <param name="rule">The rule to evaluate</param>
        /// <param name="almanac">The almanac to use</param>
        /// <returns>The rule result</returns>
        private async Task<RuleResult> EvaluateRule(Rule rule, IAlmanac almanac)
        {
            var result = await EvaluateConditions(rule.Conditions, almanac);

            if (result)
            {
                // Create a copy of the event
                var eventCopy = new Event(rule.Event.Type, new Dictionary<string, object>(rule.Event.Params));

                // Replace facts in event params if enabled
                if (_options.ReplaceFactsInEventParams)
                {
                    await ReplaceFactsInEventParams(eventCopy, almanac);
                }

                return new RuleResult(rule, true, eventCopy);
            }

            return new RuleResult(rule, false);
        }

        /// <summary>
        /// Evaluates a top-level condition against the almanac
        /// </summary>
        /// <param name="topLevelCondition">The top-level condition to evaluate</param>
        /// <param name="almanac">The almanac to use</param>
        /// <returns>True if the condition is satisfied, false otherwise</returns>
        private async Task<bool> EvaluateConditions(TopLevelCondition topLevelCondition, IAlmanac almanac)
        {
            var results = new List<bool>();

            foreach (var condition in topLevelCondition.Conditions)
            {
                var result = await EvaluateCondition(condition, almanac);
                results.Add(result);

                // Short-circuit evaluation
                if (topLevelCondition.BooleanOperator == "all" && !result)
                    return false;

                if (topLevelCondition.BooleanOperator == "any" && result)
                    return true;
            }

            // If we get here, we need to check the final result
            if (topLevelCondition.BooleanOperator == "all")
                return results.All(r => r);

            if (topLevelCondition.BooleanOperator == "any")
                return results.Any(r => r);

            throw new InvalidOperationException($"Unknown boolean operator: {topLevelCondition.BooleanOperator}");
        }

        /// <summary>
        /// Evaluates a condition against the almanac
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <param name="almanac">The almanac to use</param>
        /// <returns>True if the condition is satisfied, false otherwise</returns>
        private async Task<bool> EvaluateCondition(Condition condition, IAlmanac almanac)
        {
            // Get fact value
            var factValue = await almanac.FactValue(condition.Fact);

            // Resolve path if specified
            if (!string.IsNullOrEmpty(condition.Path) && factValue != null)
            {
                factValue = _options.PathResolver.ResolveValue(factValue, condition.Path);
            }

            // Get operator
            var op = _operatorRegistry.GetOperator(condition.Operator);

            // Evaluate condition
            return op.Evaluate(factValue, condition.Value);
        }

        /// <summary>
        /// Replaces fact references in event parameters with their actual values
        /// </summary>
        /// <param name="event">The event to process</param>
        /// <param name="almanac">The almanac to use</param>
        private async Task ReplaceFactsInEventParams(Event @event, IAlmanac almanac)
        {
            foreach (var param in @event.Params.ToList())
            {
                if (param.Value is string strValue)
                {
                    if (strValue.StartsWith("{{") && strValue.EndsWith("}}"))
                    {
                        // Extract fact ID
                        var factId = strValue.Substring(2, strValue.Length - 4).Trim();

                        // Get fact value
                        var factValue = await almanac.FactValue(factId);

                        // Replace parameter value
                        @event.Params[param.Key] = factValue;
                    }
                    else if (strValue.StartsWith("$."))
                    {
                        // Extract fact path
                        var factPath = strValue.Substring(2);

                        // Get fact value
                        var factValue = await almanac.FactValue(factPath);

                        // Replace parameter value
                        @event.Params[param.Key] = factValue;
                    }
                }
            }
        }
    }
}
