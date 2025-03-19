using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Interfaces
{
    /// <summary>
    /// Interface for the Engine which evaluates rules
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Adds a rule to the engine
        /// </summary>
        /// <param name="rule">The rule to add</param>
        void AddRule(Models.Rule rule);
        
        /// <summary>
        /// Updates an existing rule
        /// </summary>
        /// <param name="rule">The rule to update</param>
        void UpdateRule(Models.Rule rule);
        
        /// <summary>
        /// Removes a rule by instance
        /// </summary>
        /// <param name="rule">The rule to remove</param>
        /// <returns>True if the rule was removed, false otherwise</returns>
        bool RemoveRule(Models.Rule rule);
        
        /// <summary>
        /// Removes a rule by ID
        /// </summary>
        /// <param name="ruleId">The ID of the rule to remove</param>
        /// <returns>True if the rule was removed, false otherwise</returns>
        bool RemoveRule(string ruleId);
        
        /// <summary>
        /// Runs the engine with the provided facts
        /// </summary>
        /// <param name="facts">The facts to evaluate rules against</param>
        /// <param name="options">Optional run options</param>
        /// <returns>The engine result</returns>
        Task<Models.EngineResult> Run(IDictionary<string, object> facts = null, Models.RunOptions options = null);
        
        /// <summary>
        /// Stops the engine
        /// </summary>
        void Stop();
    }
}
