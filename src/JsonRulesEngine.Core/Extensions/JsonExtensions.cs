using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Core.Extensions
{
    /// <summary>
    /// Extension methods for JSON serialization and deserialization
    /// </summary>
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Serializes an object to JSON
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The JSON representation of the object</returns>
        public static string ToJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }

        /// <summary>
        /// Deserializes JSON to an object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="json">The JSON to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        /// <summary>
        /// Creates a rule from a JSON string
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>The created rule</returns>
        public static Rule CreateRuleFromJson(this string json)
        {
            return json.FromJson<Rule>();
        }

        /// <summary>
        /// Creates a condition from a JSON string
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>The created condition</returns>
        public static Condition CreateConditionFromJson(this string json)
        {
            return json.FromJson<Condition>();
        }
    }
}
