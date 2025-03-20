using System.Collections.Generic;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Options for the Engine
    /// </summary>
    public class EngineOptions
    {
        /// <summary>
        /// Gets or sets whether undefined facts are allowed
        /// </summary>
        public bool AllowUndefinedFacts { get; set; }
        
        /// <summary>
        /// Gets or sets whether undefined conditions are allowed
        /// </summary>
        public bool AllowUndefinedConditions { get; set; }
        
        /// <summary>
        /// Gets or sets whether facts should be replaced in event parameters
        /// </summary>
        public bool ReplaceFactsInEventParams { get; set; }
        
        /// <summary>
        /// Gets or sets the path resolver
        /// </summary>
        public required Interfaces.IPathResolver PathResolver { get; set; }
    }
}
