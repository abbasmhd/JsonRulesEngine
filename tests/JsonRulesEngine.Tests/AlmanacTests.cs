using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Models;
using Xunit;

namespace JsonRulesEngine.Tests
{
    public class AlmanacTests
    {
        [Fact]
        public async Task FactValue_WithExistingFact_ReturnsCorrectValue()
        {
            // Arrange
            var fact = Fact.Create("testFact", "testValue");
            var almanac = new Almanac(new[] { fact });
            
            // Act
            var result = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("testValue", result);
        }
        
        [Fact]
        public async Task FactValue_WithNonExistingFactAndAllowUndefinedFacts_ReturnsNull()
        {
            // Arrange
            var options = new AlmanacOptions { AllowUndefinedFacts = true };
            var almanac = new Almanac(null, options);
            
            // Act
            var result = await almanac.FactValue("nonExistingFact");
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task FactValue_WithNonExistingFactAndNotAllowUndefinedFacts_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new AlmanacOptions { AllowUndefinedFacts = false };
            var almanac = new Almanac(null, options);
            
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => almanac.FactValue("nonExistingFact"));
        }
        
        [Fact]
        public async Task FactValue_WithRuntimeFact_ReturnsRuntimeValue()
        {
            // Arrange
            var fact = Fact.Create("testFact", "testValue");
            var almanac = new Almanac(new[] { fact });
            almanac.AddRuntimeFact("runtimeFact", "runtimeValue");
            
            // Act
            var result = await almanac.FactValue("runtimeFact");
            
            // Assert
            Assert.Equal("runtimeValue", result);
        }
        
        [Fact]
        public async Task FactValue_WithRuntimeFactOverridingExistingFact_ReturnsRuntimeValue()
        {
            // Arrange
            var fact = Fact.Create("testFact", "testValue");
            var almanac = new Almanac(new[] { fact });
            almanac.AddRuntimeFact("testFact", "runtimeValue");
            
            // Act
            var result = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("runtimeValue", result);
        }
        
        [Fact]
        public async Task FactValue_WithParameters_PassesParametersCorrectly()
        {
            // Arrange
            var fact = new Fact("testFact", (parameters, _) => 
            {
                return Task.FromResult<object>(parameters["param1"]);
            });
            
            var almanac = new Almanac(new[] { fact });
            var parameters = new Dictionary<string, object>
            {
                { "param1", "paramValue" }
            };
            
            // Act
            var result = await almanac.FactValue("testFact", parameters);
            
            // Assert
            Assert.Equal("paramValue", result);
        }
        
        [Fact]
        public async Task FactValue_WithCachedFact_ReturnsCachedValue()
        {
            // Arrange
            int callCount = 0;
            var fact = new Fact("testFact", (_, __) => 
            {
                callCount++;
                return Task.FromResult<object>($"testValue{callCount}");
            }, new FactOptions { Cache = true });
            
            var almanac = new Almanac(new[] { fact });
            
            // Act
            var result1 = await almanac.FactValue("testFact");
            var result2 = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("testValue1", result1);
            Assert.Equal("testValue1", result2); // Should be cached
            Assert.Equal(1, callCount); // Should only be called once
        }
        
        [Fact]
        public async Task FactValue_WithNonCachedFact_ReturnsNewValueEachTime()
        {
            // Arrange
            int callCount = 0;
            var fact = new Fact("testFact", (_, __) => 
            {
                callCount++;
                return Task.FromResult<object>($"testValue{callCount}");
            }, new FactOptions { Cache = false });
            
            var almanac = new Almanac(new[] { fact });
            
            // Act
            var result1 = await almanac.FactValue("testFact");
            var result2 = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("testValue1", result1);
            Assert.Equal("testValue2", result2); // Should not be cached
            Assert.Equal(2, callCount); // Should be called twice
        }
        
        [Fact]
        public void AddFact_WithNullFact_ThrowsArgumentNullException()
        {
            // Arrange
            var almanac = new Almanac();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => almanac.AddFact(null));
        }
        
        [Fact]
        public async Task AddFact_WithValidFact_MakesFactAvailable()
        {
            // Arrange
            var almanac = new Almanac();
            var fact = Fact.Create("testFact", "testValue");
            
            // Act
            almanac.AddFact(fact);
            var result = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("testValue", result);
        }
    }
}
