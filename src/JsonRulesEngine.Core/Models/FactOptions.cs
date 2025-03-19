namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Options for facts
    /// </summary>
    public class FactOptions
    {
        /// <summary>
        /// Gets or sets whether the fact value should be cached
        /// </summary>
        public bool Cache { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the priority of the fact (lower numbers are evaluated first)
        /// </summary>
        public int Priority { get; set; } = 1;
    }
}
