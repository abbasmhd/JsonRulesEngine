namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Options for the Almanac
    /// </summary>
    public class AlmanacOptions
    {
        /// <summary>
        /// Gets or sets whether undefined facts are allowed
        /// </summary>
        public bool AllowUndefinedFacts { get; set; }
        
        /// <summary>
        /// Gets or sets the path resolver
        /// </summary>
        public Interfaces.IPathResolver PathResolver { get; set; }
    }
}
