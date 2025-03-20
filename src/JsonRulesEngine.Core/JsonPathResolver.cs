using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRulesEngine.Core.Interfaces;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Core
{
    /// <summary>
    /// Implementation of the JsonPathResolver interface that resolves paths using JSON path syntax
    /// </summary>
    public class JsonPathResolver : IPathResolver
    {
        /// <summary>
        /// Resolves a value from a fact object using the specified JSON path
        /// </summary>
        /// <param name="fact">The fact object</param>
        /// <param name="path">The JSON path to resolve</param>
        /// <returns>The resolved value, or null if the path cannot be resolved</returns>
        public object? ResolveValue(object? fact, string path)
        {
            if (fact == null)
                return null;
                
            if (string.IsNullOrEmpty(path))
                return fact;
                
            // Simple implementation for basic path resolution
            // Format: $.property.subproperty
            if (path.StartsWith("$."))
            {
                path = path[2..];
                string[] parts = path.Split('.');
                
                object? current = fact;
                foreach (var part in parts)
                {
                    if (current == null)
                        return null;
                        
                    // Handle dictionary
                    if (current is IDictionary<string, object> dict)
                    {
                        if (dict.TryGetValue(part, out var value))
                            current = value;
                        else
                            return null;
                    }
                    // Handle regular object properties
                    else
                    {
                        var property = current.GetType().GetProperty(part);
                        if (property != null)
                            current = property.GetValue(current);
                        else
                            return null;
                    }
                }
                
                return current;
            }
            
            return null;
        }
    }
}
