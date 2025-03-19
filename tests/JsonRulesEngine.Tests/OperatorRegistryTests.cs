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
            Assert.Throws<KeyNotFoundException>(() => registry.GetOperator("invalidOperator"));
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
            Assert.Equal(10, operators.Count()); // Default operators
        }
        
        // Custom operator for testing
        private class CustomTestOperator : Operator
        {
            public override string Name => "customTest";
            
            public override bool Evaluate(object factValue, object compareToValue)
            {
                return true;
            }
        }
    }
}
