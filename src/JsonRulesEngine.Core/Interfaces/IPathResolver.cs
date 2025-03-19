using System;

namespace JsonRulesEngine.Core.Interfaces
{
    /// <summary>
    /// Interface for resolving paths within fact objects
    /// </summary>
    public interface IPathResolver
    {
        /// <summary>
        /// Resolves a value from a fact object using the specified path
        /// </summary>
        /// <param name="fact">The fact object</param>
        /// <param name="path">The path to resolve</param>
        /// <returns>The resolved value</returns>
        object ResolveValue(object fact, string path);
    }
}
