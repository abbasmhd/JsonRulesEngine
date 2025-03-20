using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Models;
using Xunit;

namespace JsonRulesEngine.Tests
{
    public class FactTests
    {
        [Fact]
        public async Task ValueCallback_ReturnsCorrectValue()
        {
            // Arrange
            var fact = new Fact("testFact", (_, _) => Task.FromResult<object>("testValue"));
            
            // Act
            var result = await fact.ValueCallback(new Dictionary<string, object>(), null!);
            
            // Assert
            Assert.Equal("testValue", result);
        }
        
        [Fact]
        public async Task ValueCallback_WithParameters_PassesParametersCorrectly()
        {
            // Arrange
            var fact = new Fact("testFact", (parameters, _) => 
            {
                return Task.FromResult<object>(parameters["param1"]);
            });
            
            var parameters = new Dictionary<string, object>
            {
                { "param1", "paramValue" }
            };
            
            // Act
            var result = await fact.ValueCallback(parameters, null!);
            
            // Assert
            Assert.Equal("paramValue", result);
        }
        
        [Fact]
        public void Create_WithStaticValue_CreatesFactWithCorrectValue()
        {
            // Arrange & Act
            var fact = Fact.Create("testFact", "testValue");
            
            // Assert
            Assert.Equal("testFact", fact.Id);
            Assert.NotNull(fact.ValueCallback);
        }
        
        [Fact]
        public async Task Create_WithStaticValue_ValueCallbackReturnsCorrectValue()
        {
            // Arrange
            var fact = Fact.Create("testFact", "testValue");
            
            // Act
            var result = await fact.ValueCallback(new Dictionary<string, object>(), null!);
            
            // Assert
            Assert.Equal("testValue", result);
        }
        
        [Fact]
        public void Constructor_WithNullId_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Fact(null!, (_, _) => Task.FromResult<object>("testValue")));
        }
        
        [Fact]
        public void Constructor_WithNullValueCallback_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Fact("testFact", null!));
        }
        
        [Fact]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            var options = new FactOptions { Cache = false, Priority = 2 };
            
            // Act
            var fact = new Fact("testFact", (_, _) => Task.FromResult<object>("testValue"), options);
            
            // Assert
            Assert.Equal("testFact", fact.Id);
            Assert.NotNull(fact.ValueCallback);
            Assert.Equal(options, fact.Options);
            Assert.False(fact.Options.Cache);
            Assert.Equal(2, fact.Options.Priority);
        }
        
        [Fact]
        public void Constructor_WithNullOptions_CreatesDefaultOptions()
        {
            // Arrange & Act
            var fact = new Fact("testFact", (_, _) => Task.FromResult<object>("testValue"));
            
            // Assert
            Assert.NotNull(fact.Options);
            Assert.True(fact.Options.Cache);
            Assert.Equal(1, fact.Options.Priority);
        }
    }
}
