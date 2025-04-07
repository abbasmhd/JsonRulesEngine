using System;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Represents a cached fact value entry with expiration support.
    /// This internal class is used by the Almanac to store cached fact values along with their creation timestamp 
    /// and expiration settings. It enables time-based cache expiration and implements part of the enhanced
    /// caching mechanism to improve performance by avoiding redundant fact resolution.
    /// </summary>
    internal class CacheEntry
    {
        /// <summary>
        /// Gets the cached value
        /// </summary>
        public object Value { get; }
        
        /// <summary>
        /// Gets the timestamp when the entry was created
        /// </summary>
        public DateTime CreatedAt { get; }
        
        /// <summary>
        /// Gets the expiration time in seconds (0 = no expiration)
        /// </summary>
        public int ExpirationInSeconds { get; }
        
        /// <summary>
        /// Initializes a new instance of the CacheEntry class
        /// </summary>
        /// <param name="value">The value to cache</param>
        /// <param name="expirationInSeconds">The expiration time in seconds (0 = no expiration)</param>
        public CacheEntry(object value, int expirationInSeconds)
        {
            Value = value;
            CreatedAt = DateTime.UtcNow;
            ExpirationInSeconds = expirationInSeconds;
        }
        
        /// <summary>
        /// Checks if the cache entry has expired
        /// </summary>
        /// <returns>True if the entry has expired, false otherwise</returns>
        public bool IsExpired()
        {
            if (ExpirationInSeconds <= 0)
                return false;
                
            var expirationTime = CreatedAt.AddSeconds(ExpirationInSeconds);
            return DateTime.UtcNow >= expirationTime;
        }
    }
} 