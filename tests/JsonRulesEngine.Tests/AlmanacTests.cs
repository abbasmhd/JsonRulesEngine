using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Models;
using Xunit;
using System.Linq;

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
            var options = new AlmanacOptions { 
                AllowUndefinedFacts = true,
                PathResolver = new JsonPathResolver()
            };
            var almanac = new Almanac([], options);
            
            // Act
            var result = await almanac.FactValue("nonExistingFact");
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task FactValue_WithNonExistingFactAndNotAllowUndefinedFacts_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new AlmanacOptions { 
                AllowUndefinedFacts = false,
                PathResolver = new JsonPathResolver()
            };
            var almanac = new Almanac([], options);
            
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
        public async Task FactValue_WithCacheDisabledGlobally_ReturnsNewValueEachTime()
        {
            // Arrange
            int callCount = 0;
            var fact = new Fact("testFact", (_, __) => 
            {
                callCount++;
                return Task.FromResult<object>($"testValue{callCount}");
            }, new FactOptions { Cache = true });
            
            var options = new AlmanacOptions 
            { 
                EnableFactCaching = false,
                PathResolver = new JsonPathResolver()
            };
            var almanac = new Almanac(new[] { fact }, options);
            
            // Act
            var result1 = await almanac.FactValue("testFact");
            var result2 = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("testValue1", result1);
            Assert.Equal("testValue2", result2); // Should not be cached
            Assert.Equal(2, callCount); // Should be called twice
        }
        
        [Fact]
        public async Task FactValue_WithExpiredCache_ReturnsFreshValue()
        {
            // Arrange
            int callCount = 0;
            var fact = new Fact("testFact", (_, __) => 
            {
                callCount++;
                return Task.FromResult<object>($"testValue{callCount}");
            }, new FactOptions { Cache = true, CacheExpirationInSeconds = 1 });
            
            var almanac = new Almanac(new[] { fact });
            
            // Act
            var result1 = await almanac.FactValue("testFact");
            
            // Wait for cache to expire
            await Task.Delay(1100); // Wait just over 1 second
            
            var result2 = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("testValue1", result1);
            Assert.Equal("testValue2", result2); // Should get fresh value after expiration
            Assert.Equal(2, callCount); // Should be called twice
        }
        
        [Fact]
        public async Task ClearCache_RemovesAllCachedValues()
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
            almanac.ClearCache();
            var result2 = await almanac.FactValue("testFact");
            
            // Assert
            Assert.Equal("testValue1", result1);
            Assert.Equal("testValue2", result2); // Should get fresh value after cache clear
            Assert.Equal(2, callCount); // Should be called twice
        }
        
        [Fact]
        public async Task InvalidateCache_RemovesSpecificFactFromCache()
        {
            // Arrange
            int fact1CallCount = 0;
            int fact2CallCount = 0;
            
            var fact1 = new Fact("fact1", (_, __) => 
            {
                fact1CallCount++;
                return Task.FromResult<object>($"value1.{fact1CallCount}");
            }, new FactOptions { Cache = true });
            
            var fact2 = new Fact("fact2", (_, __) => 
            {
                fact2CallCount++;
                return Task.FromResult<object>($"value2.{fact2CallCount}");
            }, new FactOptions { Cache = true });
            
            var almanac = new Almanac(new[] { fact1, fact2 });
            
            // Act - First access to cache both facts
            var result1a = await almanac.FactValue("fact1");
            var result2a = await almanac.FactValue("fact2");
            
            // Invalidate just fact1
            almanac.InvalidateCache("fact1");
            
            // Act - Access again
            var result1b = await almanac.FactValue("fact1");
            var result2b = await almanac.FactValue("fact2");
            
            // Assert
            Assert.Equal("value1.1", result1a);
            Assert.Equal("value2.1", result2a);
            
            // fact1 should be recalculated, fact2 should use cache
            Assert.Equal("value1.2", result1b);
            Assert.Equal("value2.1", result2b);
            
            Assert.Equal(2, fact1CallCount);
            Assert.Equal(1, fact2CallCount);
        }
        
        [Fact]
        public async Task FactValue_WithCacheSizeLimit_EvictsOldestEntries()
        {
            // Arrange
            var callCounts = new Dictionary<string, int>();
            
            // Create a function to generate facts
            Fact CreateFact(string id)
            {
                callCounts[id] = 0;
                return new Fact(id, (_, __) =>
                {
                    callCounts[id]++;
                    return Task.FromResult<object>($"{id}.{callCounts[id]}");
                }, new FactOptions { Cache = true });
            }
            
            // Create 5 facts
            var facts = Enumerable.Range(1, 5)
                .Select(i => CreateFact($"fact{i}"))
                .ToList();
            
            // Set cache size limit to 3
            var options = new AlmanacOptions
            {
                EnableFactCaching = true,
                CacheMaxSize = 3,
                PathResolver = new JsonPathResolver()
            };
            
            var almanac = new Almanac(facts, options);
            
            // Act - First access to cache the first 3 facts
            for (int i = 1; i <= 3; i++)
            {
                await almanac.FactValue($"fact{i}");
            }
            
            // Access facts 4 and 5 to push out facts 1 and 2 from cache
            await almanac.FactValue("fact4");
            await almanac.FactValue("fact5");
            
            // Now access all facts again
            var results = new Dictionary<string, object>();
            for (int i = 1; i <= 5; i++)
            {
                results[$"fact{i}"] = await almanac.FactValue($"fact{i}");
            }
            
            // Assert
            // With our implementation of the LRU cache, all facts are called twice
            // The eviction happens when the cache is full and a new entry needs to be added
            Assert.Equal(2, callCounts["fact1"]);
            Assert.Equal(2, callCounts["fact2"]);
            Assert.Equal(2, callCounts["fact3"]);
            Assert.Equal(2, callCounts["fact4"]);
            Assert.Equal(2, callCounts["fact5"]);
            
            // Results should reflect the latest calls
            Assert.Equal("fact1.2", results["fact1"]);
            Assert.Equal("fact2.2", results["fact2"]);
            Assert.Equal("fact3.2", results["fact3"]);
            Assert.Equal("fact4.2", results["fact4"]);
            Assert.Equal("fact5.2", results["fact5"]);
        }
        
        [Fact]
        public void AddFact_WithNullFact_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Almanac().AddFact(null!));
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
        
        [Fact]
        public async Task FactValue_WithParameterizedFactAndCaching_CachesPerParameterSet()
        {
            // Arrange
            int callCount = 0;
            var fact = new Fact("paramFact", (parameters, _) => 
            {
                callCount++;
                var userId = parameters.ContainsKey("userId") ? parameters["userId"] : "default";
                return Task.FromResult<object>($"Result for {userId}, call {callCount}");
            }, new FactOptions { Cache = true });
            
            var almanac = new Almanac(new[] { fact });
            
            var params1 = new Dictionary<string, object> { { "userId", "user1" } };
            var params2 = new Dictionary<string, object> { { "userId", "user2" } };
            
            // Act - First calls with different parameters
            var result1a = await almanac.FactValue("paramFact", params1);
            var result2a = await almanac.FactValue("paramFact", params2);
            
            // Access again with the same parameters
            var result1b = await almanac.FactValue("paramFact", params1);
            var result2b = await almanac.FactValue("paramFact", params2);
            
            // Try with a third set of parameters
            var params3 = new Dictionary<string, object> { { "userId", "user3" } };
            var result3 = await almanac.FactValue("paramFact", params3);
            
            // Assert
            Assert.Equal("Result for user1, call 1", result1a);
            Assert.Equal("Result for user2, call 2", result2a);
            
            // Same parameters should use cached values
            Assert.Equal("Result for user1, call 1", result1b);
            Assert.Equal("Result for user2, call 2", result2b);
            
            // New parameters should cause a new evaluation
            Assert.Equal("Result for user3, call 3", result3);
            
            // Total call count should be 3 (once per unique parameter set)
            Assert.Equal(3, callCount);
        }
        
        [Fact]
        public async Task FactValue_WithParameterizedFactAndCacheInvalidation_RefreshesSpecificParameter()
        {
            // Arrange
            int callCount = 0;
            var fact = new Fact("paramFact", (parameters, _) => 
            {
                callCount++;
                var userId = parameters.ContainsKey("userId") ? parameters["userId"] : "default";
                return Task.FromResult<object>($"Result for {userId}, call {callCount}");
            }, new FactOptions { Cache = true });
            
            var almanac = new Almanac(new[] { fact });
            
            var params1 = new Dictionary<string, object> { { "userId", "user1" } };
            var params2 = new Dictionary<string, object> { { "userId", "user2" } };
            
            // Act - Cache both parameter sets
            var result1a = await almanac.FactValue("paramFact", params1);
            var result2a = await almanac.FactValue("paramFact", params2);
            
            // Invalidate the entire cache for paramFact
            almanac.InvalidateCache("paramFact");
            
            // Access again - should recalculate both
            var result1b = await almanac.FactValue("paramFact", params1);
            var result2b = await almanac.FactValue("paramFact", params2);
            
            // Assert
            Assert.Equal("Result for user1, call 1", result1a);
            Assert.Equal("Result for user2, call 2", result2a);
            
            // After invalidation, should get new values
            Assert.Equal("Result for user1, call 3", result1b);
            Assert.Equal("Result for user2, call 4", result2b);
            
            // Total call count should be 4 (twice per unique parameter set)
            Assert.Equal(4, callCount);
        }
    }
}
