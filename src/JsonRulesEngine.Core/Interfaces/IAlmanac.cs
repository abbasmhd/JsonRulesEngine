using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Interfaces
{
    /// <summary>
    /// Interface for the Almanac which manages facts during rule evaluation
    /// </summary>
    public interface IAlmanac
    {
        /// <summary>
        /// Gets the value of a fact
        /// </summary>
        /// <param name="factId">The ID of the fact</param>
        /// <param name="params">Optional parameters to pass to the fact</param>
        /// <returns>The fact value</returns>
        Task<object> FactValue(string factId, IDictionary<string, object>? @params = null);
        
        /// <summary>
        /// Adds a runtime fact
        /// </summary>
        /// <param name="factId">The ID of the fact</param>
        /// <param name="value">The fact value</param>
        void AddRuntimeFact(string factId, object value);
        
        /// <summary>
        /// Clears the fact cache
        /// </summary>
        void ClearCache();
        
        /// <summary>
        /// Invalidates the cache entry for a specific fact
        /// </summary>
        /// <param name="factId">The ID of the fact to invalidate</param>
        void InvalidateCache(string factId);
    }
}
