# Operators Guide

This guide provides detailed information about all operators available in JsonRulesEngine, how they work, and examples of how to use them.

## Overview

Operators are used in rule conditions to compare fact values against condition values. Each operator performs a specific type of comparison and returns a boolean result indicating whether the condition is satisfied.

## Available Operators

JsonRulesEngine provides several categories of operators:

### Comparison Operators

These operators compare basic values such as numbers, strings, and booleans.

#### `equal`

Checks if two values are equal.

```csharp
new Condition("age", "equal", 30)
```

Behavior:
- Performs an equality check using `Equals` method
- If both values are null, returns `true`
- If only one value is null, returns `false`

Examples:
```csharp
"hello" equal "hello" => true
10 equal 10 => true
true equal true => true
"10" equal 10 => false (different types)
null equal null => true
"hello" equal null => false
```

#### `notEqual`

Checks if two values are not equal.

```csharp
new Condition("age", "notEqual", 30)
```

Behavior:
- Performs the opposite of the `equal` operator
- If both values are null, returns `false`
- If only one value is null, returns `true`

Examples:
```csharp
"hello" notEqual "world" => true
10 notEqual 20 => true
true notEqual false => true
"10" notEqual 10 => true (different types)
null notEqual null => false
"hello" notEqual null => true
```

#### `greaterThan`

Checks if a value is greater than another value.

```csharp
new Condition("age", "greaterThan", 30)
```

Behavior:
- Compares values using the `IComparable` interface
- If either value is null, returns `false`
- Both values must implement `IComparable`

Examples:
```csharp
40 greaterThan 30 => true
30 greaterThan 30 => false
20 greaterThan 30 => false
"b" greaterThan "a" => true
"a" greaterThan "b" => false
```

#### `greaterThanInclusive`

Checks if a value is greater than or equal to another value.

```csharp
new Condition("age", "greaterThanInclusive", 30)
```

Behavior:
- Similar to `greaterThan` but includes equality
- If either value is null, returns `false`
- Both values must implement `IComparable`

Examples:
```csharp
40 greaterThanInclusive 30 => true
30 greaterThanInclusive 30 => true
20 greaterThanInclusive 30 => false
```

#### `lessThan`

Checks if a value is less than another value.

```csharp
new Condition("age", "lessThan", 30)
```

Behavior:
- Compares values using the `IComparable` interface
- If either value is null, returns `false`
- Both values must implement `IComparable`

Examples:
```csharp
20 lessThan 30 => true
30 lessThan 30 => false
40 lessThan 30 => false
"a" lessThan "b" => true
"b" lessThan "a" => false
```

#### `lessThanInclusive`

Checks if a value is less than or equal to another value.

```csharp
new Condition("age", "lessThanInclusive", 30)
```

Behavior:
- Similar to `lessThan` but includes equality
- If either value is null, returns `false`
- Both values must implement `IComparable`

Examples:
```csharp
20 lessThanInclusive 30 => true
30 lessThanInclusive 30 => true
40 lessThanInclusive 30 => false
```

### Collection Operators

These operators work with collections like lists, arrays, and sets.

#### `in`

Checks if a value is contained in a collection.

```csharp
new Condition("status", "in", new[] { "active", "pending" })
```

Behavior:
- Checks if the collection contains the specified value
- If either value is null, returns `false`
- The collection must implement `IEnumerable<object>`

Examples:
```csharp
"active" in ["active", "pending", "completed"] => true
"canceled" in ["active", "pending", "completed"] => false
5 in [1, 2, 3, 4, 5] => true
null in ["active", "pending"] => false
"active" in null => false
```

#### `notIn`

Checks if a value is not contained in a collection.

```csharp
new Condition("status", "notIn", new[] { "deleted", "archived" })
```

Behavior:
- Performs the opposite of the `in` operator
- If the fact value is null or the collection is null, returns `true`

Examples:
```csharp
"active" notIn ["deleted", "archived"] => true
"deleted" notIn ["deleted", "archived"] => false
null notIn ["deleted", "archived"] => true
"active" notIn null => true
```

#### `contains`

Checks if a collection contains a specific value.

```csharp
new Condition("roles", "contains", "admin")
```

Behavior:
- Checks if the fact value (which should be a collection) contains the specified value
- If either value is null, returns `false`
- The fact value must implement `IEnumerable<object>`

Examples:
```csharp
["admin", "user", "editor"] contains "admin" => true
["admin", "user", "editor"] contains "guest" => false
[] contains "admin" => false
null contains "admin" => false
["admin", "user"] contains null => false
```

#### `doesNotContain`

Checks if a collection does not contain a specific value.

```csharp
new Condition("roles", "doesNotContain", "guest")
```

Behavior:
- Performs the opposite of the `contains` operator
- If the fact value is null or the comparison value is null, returns `true`

Examples:
```csharp
["admin", "user", "editor"] doesNotContain "guest" => true
["admin", "user", "editor"] doesNotContain "admin" => false
[] doesNotContain "guest" => true
null doesNotContain "guest" => true
["admin", "user"] doesNotContain null => true
```

### String Operators

These operators perform string-specific comparisons.

#### `startsWith`

Checks if a string starts with another string.

```csharp
new Condition("email", "startsWith", "admin")
```

Behavior:
- Uses `String.StartsWith` method with ordinal comparison
- If either value is null, returns `false`
- Case-sensitive by default

Examples:
```csharp
"admin@example.com" startsWith "admin" => true
"user@example.com" startsWith "admin" => false
"Admin@example.com" startsWith "admin" => false (case-sensitive)
"" startsWith "" => true
"hello" startsWith "" => true
"" startsWith "hello" => false
```

#### `endsWith`

Checks if a string ends with another string.

```csharp
new Condition("email", "endsWith", "@company.com")
```

Behavior:
- Uses `String.EndsWith` method with ordinal comparison
- If either value is null, returns `false`
- Case-sensitive by default

Examples:
```csharp
"john@company.com" endsWith "@company.com" => true
"john@example.com" endsWith "@company.com" => false
"john@Company.com" endsWith "@company.com" => false (case-sensitive)
"" endsWith "" => true
"hello" endsWith "" => true
"" endsWith "hello" => false
```

#### `stringContains`

Checks if a string contains a substring.

```csharp
new Condition("message", "stringContains", "urgent")
```

Behavior:
- Uses `String.Contains` method with ordinal comparison
- If either value is null, returns `false`
- Case-sensitive by default

Examples:
```csharp
"This is an urgent message" stringContains "urgent" => true
"Please review this" stringContains "urgent" => false
"This is an Urgent message" stringContains "urgent" => false (case-sensitive)
"" stringContains "" => true
"hello" stringContains "" => true
"" stringContains "hello" => false
```

#### `matches`

Checks if a string matches a regular expression pattern.

```csharp
new Condition("zipcode", "matches", "^\\d{5}(-\\d{4})?$")
```

Behavior:
- Uses `Regex.IsMatch` method to check if the string matches the pattern
- If either value is null, returns `false`
- Handles invalid regex patterns gracefully (returns `false` instead of throwing an exception)

Examples:
```csharp
"12345" matches "^\\d{5}(-\\d{4})?$" => true
"12345-6789" matches "^\\d{5}(-\\d{4})?$" => true
"1234" matches "^\\d{5}(-\\d{4})?$" => false
"abc123" matches "^\\d{5}(-\\d{4})?$" => false
"test@example.com" matches "^[\\w.-]+@[\\w.-]+\\.\\w+$" => true
```

## Creating Custom Operators

You can create custom operators by extending the `Operator` class and implementing the `IOperator<TFactValue, TCompareValue>` interface. See the [Advanced Usage Guide](advanced.md) for more information on creating custom operators.

## Best Practices

- Choose the most appropriate operator for your comparison to ensure accurate results
- Be aware of null handling for each operator
- For string comparisons, consider case sensitivity requirements
- For regex patterns, test them thoroughly to ensure they match as expected
- When working with collections, ensure they implement the correct interfaces

## Next Steps

- [Advanced Usage Guide](advanced.md) - Learn about advanced techniques including custom operators
- [Troubleshooting](troubleshooting.md) - Common issues and solutions
- [API Reference](../api/index.md) - Full API documentation 