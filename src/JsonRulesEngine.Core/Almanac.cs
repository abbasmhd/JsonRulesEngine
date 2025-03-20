using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonRulesEngine.Core.Interfaces;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Core
{
    /// <summary>
    /// Implementation of the Almanac which manages facts during rule evaluation
    /// </summary>
    public class Almanac : IAlmanac
    {
        private readonly Dictionary<string, Fact> _facts;
        private readonly Dictionary<string, object> _factValues;
        private readonly Dictionary<string, object> _runtimeFacts;
        private readonly AlmanacOptions _options;

        /// <summary>
        /// Initializes a new instance of the Almanac class
        /// </summary>
        /// <param name="facts">The facts to include in the almanac</param>
        /// <param name="options">The options for the almanac</param>
        public Almanac(IEnumerable<Fact>? facts = null, AlmanacOptions? options = null)
        {
            _facts = facts?.ToDictionary(f => f.Id) ?? new Dictionary<string, Fact>();
            _factValues = new Dictionary<string, object>();
            _runtimeFacts = new Dictionary<string, object>();
            _options = options ?? new AlmanacOptions
            {
                AllowUndefinedFacts = false,
                PathResolver = new JsonPathResolver()
            };
        }

        /// <summary>
        /// Gets the value of a fact
        /// </summary>
        /// <param name="factId">The ID of the fact</param>
        /// <param name="params">Optional parameters to pass to the fact</param>
        /// <returns>The fact value</returns>
        public async Task<object> FactValue(string factId, IDictionary<string, object>? @params = null)
        {
            // Check runtime facts first
            if (_runtimeFacts.TryGetValue(factId, out var runtimeValue))
                return runtimeValue;

            // Check if fact exists
            if (!_facts.TryGetValue(factId, out var fact))
            {
                if (_options.AllowUndefinedFacts)
                    return null!;

                throw new KeyNotFoundException($"Fact '{factId}' not found in almanac");
            }

            // Check cache if enabled
            string cacheKey = factId;
            if (@params != null && @params.Count > 0)
            {
                // Create a cache key that includes parameters
                var sortedParams = new SortedDictionary<string, object>(@params);
                cacheKey = $"{factId}:{string.Join(",", sortedParams.Select(p => $"{p.Key}={p.Value}"))}";
            }

            if (fact.Options.Cache && _factValues.TryGetValue(cacheKey, out var cachedValue))
                return cachedValue;

            // Evaluate fact
            var value = await fact.ValueCallback(@params ?? new Dictionary<string, object>(), this);

            // Cache if enabled
            if (fact.Options.Cache)
            {
                _factValues[cacheKey] = value;
            }

            return value;
        }

        /// <summary>
        /// Adds a runtime fact
        /// </summary>
        /// <param name="factId">The ID of the fact</param>
        /// <param name="value">The fact value</param>
        public void AddRuntimeFact(string factId, object value)
        {
            _runtimeFacts[factId] = value;
        }

        /// <summary>
        /// Adds a fact to the almanac
        /// </summary>
        /// <param name="fact">The fact to add</param>
        public void AddFact(Fact? fact)
        {
            if (fact == null)
            {
                throw new ArgumentNullException(nameof(fact));
            }

            _facts[fact.Id] = fact;
        }
    }
}
