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
        private readonly Dictionary<string, CacheEntry> _factCache;
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
            _factCache = new Dictionary<string, CacheEntry>();
            _runtimeFacts = new Dictionary<string, object>();
            _options = options ?? new AlmanacOptions
            {
                AllowUndefinedFacts = false,
                EnableFactCaching = true,
                CacheMaxSize = 1000,
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

            // Create cache key
            string cacheKey = CreateCacheKey(factId, @params);

            // Check cache if enabled
            if (_options.EnableFactCaching && fact.Options.Cache && _factCache.TryGetValue(cacheKey, out var cachedEntry))
            {
                // Check if the cache entry has expired
                if (!cachedEntry.IsExpired())
                {
                    return cachedEntry.Value;
                }
                
                // Remove expired entry
                _factCache.Remove(cacheKey);
            }

            // Evaluate fact
            var value = await fact.ValueCallback(@params ?? new Dictionary<string, object>(), this);

            // Cache if enabled
            if (_options.EnableFactCaching && fact.Options.Cache)
            {
                // Check if we need to enforce cache size limit
                if (_options.CacheMaxSize > 0 && _factCache.Count >= _options.CacheMaxSize)
                {
                    // Remove oldest entry (simple LRU implementation)
                    var oldestKey = _factCache.OrderBy(kv => kv.Value.CreatedAt).First().Key;
                    _factCache.Remove(oldestKey);
                }
                
                // Add to cache
                _factCache[cacheKey] = new CacheEntry(value, fact.Options.CacheExpirationInSeconds);
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
        
        /// <summary>
        /// Clears the fact cache
        /// </summary>
        public void ClearCache()
        {
            _factCache.Clear();
        }
        
        /// <summary>
        /// Invalidates the cache entry for a specific fact
        /// </summary>
        /// <param name="factId">The ID of the fact to invalidate</param>
        public void InvalidateCache(string factId)
        {
            var keysToRemove = _factCache.Keys
                .Where(key => key.StartsWith($"{factId}:") || key == factId)
                .ToList();
                
            foreach (var key in keysToRemove)
            {
                _factCache.Remove(key);
            }
        }
        
        /// <summary>
        /// Creates a cache key for a fact and parameters
        /// </summary>
        /// <param name="factId">The fact ID</param>
        /// <param name="params">The parameters</param>
        /// <returns>The cache key</returns>
        private string CreateCacheKey(string factId, IDictionary<string, object>? @params)
        {
            if (@params == null || @params.Count == 0)
                return factId;
                
            // Create a cache key that includes parameters
            var sortedParams = new SortedDictionary<string, object>(@params);
            return $"{factId}:{string.Join(",", sortedParams.Select(p => $"{p.Key}={p.Value}"))}";
        }
    }
}
