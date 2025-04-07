using System;
using System.Collections.Generic;
using System.Linq;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Operators;
using Xunit;

namespace JsonRulesEngine.Tests
{
    public class OperatorRegistryTests
    {
        [Fact]
        public void Constructor_CreatesRegistryWithDefaultOperators()
        {
            // Arrange & Act
            var registry = new OperatorRegistry();
            
            // Assert
            Assert.NotNull(registry.GetOperator("equal"));
            Assert.NotNull(registry.GetOperator("notEqual"));
            Assert.NotNull(registry.GetOperator("greaterThan"));
            Assert.NotNull(registry.GetOperator("greaterThanInclusive"));
            Assert.NotNull(registry.GetOperator("lessThan"));
            Assert.NotNull(registry.GetOperator("lessThanInclusive"));
            Assert.NotNull(registry.GetOperator("in"));
            Assert.NotNull(registry.GetOperator("notIn"));
            Assert.NotNull(registry.GetOperator("contains"));
            Assert.NotNull(registry.GetOperator("doesNotContain"));
        }
        
        [Fact]
        public void GetOperator_WithValidName_ReturnsOperator()
        {
            // Arrange
            var registry = new OperatorRegistry();
            
            // Act
            var op = registry.GetOperator("equal");
            
            // Assert
            Assert.NotNull(op);
            Assert.IsType<EqualOperator>(op);
            Assert.Equal("equal", op.Name);
        }
        
        [Fact]
        public void GetOperator_WithInvalidName_ThrowsKeyNotFoundException()
        {
            // Arrange
            var registry = new OperatorRegistry();
            
            // Act & Assert
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => registry.GetOperator("invalidOperator"));
        }
        
        [Fact]
        public void GetOperator_WithNullName_ThrowsArgumentNullException()
        {
            // Arrange
            var registry = new OperatorRegistry();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => registry.GetOperator(null!));
        }
        
        [Fact]
        public void AddOperator_WithValidOperator_AddsToRegistry()
        {
            // Arrange
            var registry = new OperatorRegistry();
            var customOp = new CustomTestOperator();
            
            // Act
            registry.AddOperator(customOp);
            var retrievedOp = registry.GetOperator("customTest");
            
            // Assert
            Assert.NotNull(retrievedOp);
            Assert.Same(customOp, retrievedOp);
        }
        
        [Fact]
        public void AddOperator_WithNullOperator_ThrowsArgumentNullException()
        {
            // Arrange
            var registry = new OperatorRegistry();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => registry.AddOperator(null!));
        }
        
        [Fact]
        public void GetAllOperators_ReturnsAllRegisteredOperators()
        {
            // Arrange
            var registry = new OperatorRegistry();
            
            // Act
            var operators = registry.GetAllOperators();
            
            // Assert
            Assert.NotNull(operators);
            Assert.Equal(14, operators.Count()); // Default operators + string operators
        }
        
        [Fact]
        public void DefaultRegistry_ContainsAllStandardOperators()
        {
            // Arrange
            var registry = new OperatorRegistry();
            
            // Act
            var operators = registry.GetAllOperators().ToList();
            
            // Assert
            Assert.Contains(operators, op => op.Name == "equal");
            Assert.Contains(operators, op => op.Name == "notEqual");
            Assert.Contains(operators, op => op.Name == "greaterThan");
            Assert.Contains(operators, op => op.Name == "greaterThanInclusive");
            Assert.Contains(operators, op => op.Name == "lessThan");
            Assert.Contains(operators, op => op.Name == "lessThanInclusive");
            Assert.Contains(operators, op => op.Name == "in");
            Assert.Contains(operators, op => op.Name == "notIn");
            Assert.Contains(operators, op => op.Name == "contains");
            Assert.Contains(operators, op => op.Name == "doesNotContain");
            
            // New string operators
            Assert.Contains(operators, op => op.Name == "startsWith");
            Assert.Contains(operators, op => op.Name == "endsWith");
            Assert.Contains(operators, op => op.Name == "stringContains");
            Assert.Contains(operators, op => op.Name == "matches");
        }
        
        [Fact]
        public void StringOperators_AreRegisteredCorrectly()
        {
            // Arrange
            var registry = new OperatorRegistry();
            
            // Act & Assert
            Assert.IsType<StartsWithOperator>(registry.GetOperator("startsWith"));
            Assert.IsType<EndsWithOperator>(registry.GetOperator("endsWith"));
            Assert.IsType<StringContainsOperator>(registry.GetOperator("stringContains"));
            Assert.IsType<MatchesOperator>(registry.GetOperator("matches"));
        }
        
        // Custom operator for testing
        private class CustomTestOperator : Operator
        {
            public override string Name => "customTest";
            
            public override bool Evaluate(object? factValue, object? compareToValue)
            {
                return true;
            }
        }
    }
}
