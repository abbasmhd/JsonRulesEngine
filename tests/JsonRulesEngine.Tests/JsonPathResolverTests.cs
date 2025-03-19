using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Models;
using Xunit;

namespace JsonRulesEngine.Tests
{
    public class JsonPathResolverTests
    {
        [Fact]
        public void ResolveValue_WithNullFact_ReturnsNull()
        {
            // Arrange
            var resolver = new JsonPathResolver();
            
            // Act
            var result = resolver.ResolveValue(null!, "$.property");
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void ResolveValue_WithEmptyPath_ReturnsFact()
        {
            // Arrange
            var resolver = new JsonPathResolver();
            var fact = new { Property = "value" };
            
            // Act
            var result = resolver.ResolveValue(fact, "");
            
            // Assert
            Assert.Equal(fact, result);
        }
        
        [Fact]
        public void ResolveValue_WithValidPath_ReturnsPropertyValue()
        {
            // Arrange
            var resolver = new JsonPathResolver();
            var fact = new { Property = "value" };
            
            // Act
            var result = resolver.ResolveValue(fact, "$.Property");
            
            // Assert
            Assert.Equal("value", result);
        }
        
        [Fact]
        public void ResolveValue_WithNestedPath_ReturnsNestedPropertyValue()
        {
            // Arrange
            var resolver = new JsonPathResolver();
            var fact = new { Nested = new { Property = "value" } };
            
            // Act
            var result = resolver.ResolveValue(fact, "$.Nested.Property");
            
            // Assert
            Assert.Equal("value", result);
        }
        
        [Fact]
        public void ResolveValue_WithInvalidPath_ReturnsNull()
        {
            // Arrange
            var resolver = new JsonPathResolver();
            var fact = new { Property = "value" };
            
            // Act
            var result = resolver.ResolveValue(fact, "$.InvalidProperty");
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void ResolveValue_WithDictionary_ReturnsValue()
        {
            // Arrange
            var resolver = new JsonPathResolver();
            var fact = new Dictionary<string, object>
            {
                { "Property", "value" }
            };
            
            // Act
            var result = resolver.ResolveValue(fact, "$.Property");
            
            // Assert
            Assert.Equal("value", result);
        }
        
        [Fact]
        public void ResolveValue_WithNestedDictionary_ReturnsNestedValue()
        {
            // Arrange
            var resolver = new JsonPathResolver();
            var fact = new Dictionary<string, object>
            {
                { "Nested", new Dictionary<string, object> { { "Property", "value" } } }
            };
            
            // Act
            var result = resolver.ResolveValue(fact, "$.Nested.Property");
            
            // Assert
            Assert.Equal("value", result);
        }
    }
}
