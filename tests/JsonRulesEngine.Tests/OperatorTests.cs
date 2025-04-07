using System;
using System.Collections.Generic;
using JsonRulesEngine.Core.Operators;
using Xunit;

namespace JsonRulesEngine.Tests
{
    public class OperatorTests
    {
        [Fact]
        public void EqualOperator_WithEqualValues_ReturnsTrue()
        {
            // Arrange
            var op = new EqualOperator();
            
            // Act
            var result = op.Evaluate("test", "test");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void EqualOperator_WithDifferentValues_ReturnsFalse()
        {
            // Arrange
            var op = new EqualOperator();
            
            // Act
            var result = op.Evaluate("test", "different");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void EqualOperator_WithBothNull_ReturnsTrue()
        {
            // Arrange
            var op = new EqualOperator();
            
            // Act
            var result = op.Evaluate(null!, null!);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void EqualOperator_WithOneNull_ReturnsFalse()
        {
            // Arrange
            var op = new EqualOperator();
            
            // Act
            var result1 = op.Evaluate("test", null!);
            var result2 = op.Evaluate(null!, "test");
            
            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }
        
        [Fact]
        public void NotEqualOperator_WithEqualValues_ReturnsFalse()
        {
            // Arrange
            var op = new NotEqualOperator();
            
            // Act
            var result = op.Evaluate("test", "test");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void NotEqualOperator_WithDifferentValues_ReturnsTrue()
        {
            // Arrange
            var op = new NotEqualOperator();
            
            // Act
            var result = op.Evaluate("test", "different");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void GreaterThanOperator_WithGreaterValue_ReturnsTrue()
        {
            // Arrange
            var op = new GreaterThanOperator();
            
            // Act
            var result = op.Evaluate(10, 5);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void GreaterThanOperator_WithLesserValue_ReturnsFalse()
        {
            // Arrange
            var op = new GreaterThanOperator();
            
            // Act
            var result = op.Evaluate(5, 10);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void GreaterThanOperator_WithEqualValues_ReturnsFalse()
        {
            // Arrange
            var op = new GreaterThanOperator();
            
            // Act
            var result = op.Evaluate(10, 10);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void GreaterThanOrEqualOperator_WithGreaterValue_ReturnsTrue()
        {
            // Arrange
            var op = new GreaterThanOrEqualOperator();
            
            // Act
            var result = op.Evaluate(10, 5);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void GreaterThanOrEqualOperator_WithEqualValues_ReturnsTrue()
        {
            // Arrange
            var op = new GreaterThanOrEqualOperator();
            
            // Act
            var result = op.Evaluate(10, 10);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void GreaterThanOrEqualOperator_WithLesserValue_ReturnsFalse()
        {
            // Arrange
            var op = new GreaterThanOrEqualOperator();
            
            // Act
            var result = op.Evaluate(5, 10);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void LessThanOperator_WithLesserValue_ReturnsTrue()
        {
            // Arrange
            var op = new LessThanOperator();
            
            // Act
            var result = op.Evaluate(5, 10);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void LessThanOperator_WithGreaterValue_ReturnsFalse()
        {
            // Arrange
            var op = new LessThanOperator();
            
            // Act
            var result = op.Evaluate(10, 5);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void LessThanOperator_WithEqualValues_ReturnsFalse()
        {
            // Arrange
            var op = new LessThanOperator();
            
            // Act
            var result = op.Evaluate(10, 10);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void LessThanOrEqualOperator_WithLesserValue_ReturnsTrue()
        {
            // Arrange
            var op = new LessThanOrEqualOperator();
            
            // Act
            var result = op.Evaluate(5, 10);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void LessThanOrEqualOperator_WithEqualValues_ReturnsTrue()
        {
            // Arrange
            var op = new LessThanOrEqualOperator();
            
            // Act
            var result = op.Evaluate(10, 10);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void LessThanOrEqualOperator_WithGreaterValue_ReturnsFalse()
        {
            // Arrange
            var op = new LessThanOrEqualOperator();
            
            // Act
            var result = op.Evaluate(10, 5);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void InOperator_WithValueInCollection_ReturnsTrue()
        {
            // Arrange
            var op = new InOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate("test2", collection);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void InOperator_WithValueNotInCollection_ReturnsFalse()
        {
            // Arrange
            var op = new InOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate("test4", collection);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void NotInOperator_WithValueInCollection_ReturnsFalse()
        {
            // Arrange
            var op = new NotInOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate("test2", collection);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void NotInOperator_WithValueNotInCollection_ReturnsTrue()
        {
            // Arrange
            var op = new NotInOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate("test4", collection);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void ContainsOperator_WithCollectionContainingValue_ReturnsTrue()
        {
            // Arrange
            var op = new ContainsOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate(collection, "test2");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void ContainsOperator_WithCollectionNotContainingValue_ReturnsFalse()
        {
            // Arrange
            var op = new ContainsOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate(collection, "test4");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void DoesNotContainOperator_WithCollectionContainingValue_ReturnsFalse()
        {
            // Arrange
            var op = new DoesNotContainOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate(collection, "test2");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void DoesNotContainOperator_WithCollectionNotContainingValue_ReturnsTrue()
        {
            // Arrange
            var op = new DoesNotContainOperator();
            var collection = new List<object> { "test1", "test2", "test3" };
            
            // Act
            var result = op.Evaluate(collection, "test4");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void StartsWithOperator_WithStringStartingWithValue_ReturnsTrue()
        {
            // Arrange
            var op = new StartsWithOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "Hello");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void StartsWithOperator_WithStringNotStartingWithValue_ReturnsFalse()
        {
            // Arrange
            var op = new StartsWithOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "World");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void StartsWithOperator_WithNullValues_ReturnsFalse()
        {
            // Arrange
            var op = new StartsWithOperator();
            
            // Act
            var result1 = op.Evaluate(null, "Hello");
            var result2 = op.Evaluate("Hello", null);
            
            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }
        
        [Fact]
        public void StartsWithOperator_WithEmptyString_HandlesCorrectly()
        {
            // Arrange
            var op = new StartsWithOperator();
            
            // Act & Assert
            Assert.True(op.Evaluate("Hello", ""));
            Assert.True(op.Evaluate("", ""));
            Assert.False(op.Evaluate("", "Hello"));
        }
        
        [Fact]
        public void EndsWithOperator_WithStringEndingWithValue_ReturnsTrue()
        {
            // Arrange
            var op = new EndsWithOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "World");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void EndsWithOperator_WithStringNotEndingWithValue_ReturnsFalse()
        {
            // Arrange
            var op = new EndsWithOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "Hello");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void EndsWithOperator_WithNullValues_ReturnsFalse()
        {
            // Arrange
            var op = new EndsWithOperator();
            
            // Act
            var result1 = op.Evaluate(null, "World");
            var result2 = op.Evaluate("Hello World", null);
            
            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }
        
        [Fact]
        public void EndsWithOperator_WithEmptyString_HandlesCorrectly()
        {
            // Arrange
            var op = new EndsWithOperator();
            
            // Act & Assert
            Assert.True(op.Evaluate("Hello", ""));
            Assert.True(op.Evaluate("", ""));
            Assert.False(op.Evaluate("", "Hello"));
        }
        
        [Fact]
        public void StringContainsOperator_WithStringContainingValue_ReturnsTrue()
        {
            // Arrange
            var op = new StringContainsOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "o Wo");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void StringContainsOperator_WithStringNotContainingValue_ReturnsFalse()
        {
            // Arrange
            var op = new StringContainsOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "Goodbye");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void StringContainsOperator_WithNullValues_ReturnsFalse()
        {
            // Arrange
            var op = new StringContainsOperator();
            
            // Act
            var result1 = op.Evaluate(null, "World");
            var result2 = op.Evaluate("Hello World", null);
            
            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }
        
        [Fact]
        public void StringContainsOperator_WithEmptyString_HandlesCorrectly()
        {
            // Arrange
            var op = new StringContainsOperator();
            
            // Act & Assert
            Assert.True(op.Evaluate("Hello", ""));
            Assert.True(op.Evaluate("", ""));
            Assert.False(op.Evaluate("", "Hello"));
        }
        
        [Fact]
        public void MatchesOperator_WithStringMatchingPattern_ReturnsTrue()
        {
            // Arrange
            var op = new MatchesOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "Hello.*");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void MatchesOperator_WithStringNotMatchingPattern_ReturnsFalse()
        {
            // Arrange
            var op = new MatchesOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "^Goodbye.*");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void MatchesOperator_WithNullValues_ReturnsFalse()
        {
            // Arrange
            var op = new MatchesOperator();
            
            // Act
            var result1 = op.Evaluate(null, ".*");
            var result2 = op.Evaluate("Hello World", null);
            
            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }
        
        [Fact]
        public void MatchesOperator_WithInvalidRegex_ReturnsFalse()
        {
            // Arrange
            var op = new MatchesOperator();
            
            // Act
            var result = op.Evaluate("Hello World", "["); // Invalid regex pattern
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void MatchesOperator_WithComplexRegex_MatchesCorrectly()
        {
            // Arrange
            var op = new MatchesOperator();
            
            // Act & Assert
            Assert.True(op.Evaluate("abc123", "^[a-z]+\\d+$"));
            Assert.False(op.Evaluate("ABC123", "^[a-z]+\\d+$"));
            Assert.True(op.Evaluate("test@example.com", "^[\\w.-]+@[\\w.-]+\\.\\w+$"));
        }
    }
}
