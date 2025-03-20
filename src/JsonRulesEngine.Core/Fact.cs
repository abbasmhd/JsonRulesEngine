using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRulesEngine.Core.Interfaces;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Core
{
    /// <summary>
    /// Implementation of the Fact class that represents a data point in the rules engine
    /// </summary>
    public class Fact
    {
        /// <summary>
        /// Gets the ID of the fact
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// Gets the value callback function
        /// </summary>
        public Func<IDictionary<string, object>, IAlmanac, Task<object>> ValueCallback { get; }
        
        /// <summary>
        /// Gets the options for the fact
        /// </summary>
        public FactOptions Options { get; }
        
        /// <summary>
        /// Initializes a new instance of the Fact class
        /// </summary>
        /// <param name="id">The ID of the fact</param>
        /// <param name="valueCallback">The value callback function</param>
        /// <param name="options">The options for the fact</param>
        public Fact(string id, Func<IDictionary<string, object>, IAlmanac, Task<object>> valueCallback, FactOptions? options = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ValueCallback = valueCallback ?? throw new ArgumentNullException(nameof(valueCallback));
            Options = options ?? new FactOptions();
        }
        
        /// <summary>
        /// Creates a fact with a static value
        /// </summary>
        /// <param name="id">The ID of the fact</param>
        /// <param name="value">The static value</param>
        /// <param name="options">The options for the fact</param>
        /// <returns>A new fact with a static value</returns>
        public static Fact Create(string id, object value, FactOptions? options = null)
        {
            return new Fact(id, (_, __) => Task.FromResult(value), options);
        }
    }
}
