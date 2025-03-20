using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonRulesEngine.Core.Models
{
    /// <summary>
    /// Options for running the engine
    /// </summary>
    public class RunOptions
    {
        /// <summary>
        /// Gets or sets the almanac to use during rule evaluation
        /// </summary>
        public Interfaces.IAlmanac? Almanac { get; set; }
    }
}
