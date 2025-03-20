using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Models;
using Xunit;

namespace JsonRulesEngine.Tests
{
    public class EngineTests
    {
        [Fact]
        public void Constructor_WithNoRules_CreatesEmptyEngine()
        {
            // Arrange & Act
            var engine = new Engine();
            
            // Assert
            Assert.NotNull(engine);
        }
        
        [Fact]
        public void AddRule_WithValidRule_AddsRuleToEngine()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            
            // Act
            engine.AddRule(rule);
            
            // Assert - We'll test this by running the engine
            // and checking if the rule is evaluated
        }
        
        [Fact]
        public void AddRule_WithNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            var engine = new Engine();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => engine.AddRule(null!));
        }
        
        [Fact]
        public void UpdateRule_WithExistingRule_UpdatesRule()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            engine.AddRule(rule);
            
            var updatedRule = new Rule(
                rule.Id,
                new TopLevelCondition("all", new[] { new Condition("age", "greaterThan", 30) }),
                new Event("updatedEvent"),
                2
            );
            
            // Act
            engine.UpdateRule(updatedRule);
            
            // Assert - We'll test this by running the engine
            // and checking if the updated rule is evaluated
        }
        
        [Fact]
        public void UpdateRule_WithNonExistingRule_ThrowsKeyNotFoundException()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => engine.UpdateRule(rule));
        }
        
        [Fact]
        public void RemoveRule_WithExistingRuleInstance_RemovesRule()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            engine.AddRule(rule);
            
            // Act
            var result = engine.RemoveRule(rule);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void RemoveRule_WithNonExistingRuleInstance_ReturnsFalse()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            
            // Act
            var result = engine.RemoveRule(rule);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void RemoveRule_WithExistingRuleId_RemovesRule()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            engine.AddRule(rule);
            
            // Act
            var result = engine.RemoveRule(rule.Id);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void RemoveRule_WithNonExistingRuleId_ReturnsFalse()
        {
            // Arrange
            var engine = new Engine();
            
            // Act
            var result = engine.RemoveRule("nonExistingRuleId");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task Run_WithMatchingFacts_TriggersEvent()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            engine.AddRule(rule);
            
            var facts = new Dictionary<string, object>
            {
                { "age", 25 }
            };
            
            // Act
            var result = await engine.Run(facts);
            
            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Events);
            Assert.Equal("personMatched", result.Events.First().Type);
            Assert.Single(result.Results);
            Assert.True(result.Results.First().Result);
            Assert.Empty(result.FailureResults);
        }
        
        [Fact]
        public async Task Run_WithNonMatchingFacts_DoesNotTriggerEvent()
        {
            // Arrange
            var engine = new Engine();
            var rule = CreateSampleRule();
            engine.AddRule(rule);
            
            var facts = new Dictionary<string, object>
            {
                { "age", 15 }
            };
            
            // Act
            var result = await engine.Run(facts);
            
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Events);
            Assert.Empty(result.Results);
            Assert.Single(result.FailureResults);
            Assert.False(result.FailureResults.First().Result);
        }
        
        [Fact]
        public async Task Run_WithMultipleRules_EvaluatesAllRules()
        {
            // Arrange
            var engine = new Engine();
            
            var rule1 = CreateSampleRule();
            var rule2 = new Rule(
                "rule2",
                new TopLevelCondition("all", new[] { new Condition("age", "lessThan", 30) }),
                new Event("youngPerson"),
                2
            );
            
            engine.AddRule(rule1);
            engine.AddRule(rule2);
            
            var facts = new Dictionary<string, object>
            {
                { "age", 25 }
            };
            
            // Act
            var result = await engine.Run(facts);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Events.Count());
            Assert.Contains(result.Events, e => e.Type == "personMatched");
            Assert.Contains(result.Events, e => e.Type == "youngPerson");
            Assert.Equal(2, result.Results.Count());
            Assert.Empty(result.FailureResults);
        }
        
        [Fact]
        public async Task Run_WithRuleUsingAnyOperator_EvaluatesCorrectly()
        {
            // Arrange
            var engine = new Engine();
            var rule = new Rule(
                "anyRule",
                new TopLevelCondition("any", new[] 
                { 
                    new Condition("age", "greaterThan", 30),
                    new Condition("name", "equal", "John")
                }),
                new Event("anyMatched"),
                1
            );
            
            engine.AddRule(rule);
            
            var facts = new Dictionary<string, object>
            {
                { "age", 25 },
                { "name", "John" }
            };
            
            // Act
            var result = await engine.Run(facts);
            
            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Events);
            Assert.Equal("anyMatched", result.Events.First().Type);
        }
        
        [Fact]
        public async Task Run_WithFactPath_ResolvesPathCorrectly()
        {
            // Arrange
            var engine = new Engine();
            var rule = new Rule(
                "pathRule",
                new TopLevelCondition("all", new[] 
                { 
                    new Condition("person", "equal", "John", "$.name")
                }),
                new Event("pathMatched"),
                1
            );
            
            engine.AddRule(rule);
            
            var facts = new Dictionary<string, object>
            {
                { "person", new { name = "John", age = 25 } }
            };
            
            // Act
            var result = await engine.Run(facts);
            
            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Events);
            Assert.Equal("pathMatched", result.Events.First().Type);
        }
        
        [Fact]
        public async Task Run_WithEventParamReplacement_ReplacesFactsInParams()
        {
            // Arrange
            var engine = new Engine(null, new EngineOptions
            {
                AllowUndefinedFacts = false,
                AllowUndefinedConditions = false,
                ReplaceFactsInEventParams = true,
                PathResolver = new JsonPathResolver()
            });

            var rule = new Rule(
                "testRule",
                new TopLevelCondition("all", new[] { new Condition("age", "greaterThan", 20) }),
                new Event("personMatched", new Dictionary<string, object>
                {
                    { "age", "$.age" }
                }),
                1
            );

            engine.AddRule(rule);

            var facts = new Dictionary<string, object>
            {
                { "age", 25 }
            };

            // Act
            var result = await engine.Run(facts);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Events);
            Assert.Equal(25, result.Events.First().Params["age"]);
        }
        
        private Rule CreateSampleRule()
        {
            return new Rule(
                "rule1",
                new TopLevelCondition("all", new[] { new Condition("age", "greaterThan", 20) }),
                new Event("personMatched"),
                1
            );
        }
    }
}
