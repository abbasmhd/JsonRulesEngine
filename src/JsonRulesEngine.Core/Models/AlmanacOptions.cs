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
        /// Gets or sets whether fact caching is enabled globally.
        /// When set to true, facts with their Cache property set to true will be cached.
        /// When set to false, no facts will be cached regardless of their individual Cache property.
        /// This setting overrides individual fact cache settings.
        /// </summary>
        public bool EnableFactCaching { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the maximum number of entries in the fact cache.
        /// When the limit is reached, the oldest cache entries will be evicted using a simple LRU (Least Recently Used) approach.
        /// Set to 0 for unlimited cache size (not recommended for production environments with large fact sets).
        /// Default is 1000.
        /// </summary>
        public int CacheMaxSize { get; set; } = 1000;
        
        /// <summary>
        /// Gets or sets the path resolver
        /// </summary>
        public required Interfaces.IPathResolver PathResolver { get; set; }
    }
}
