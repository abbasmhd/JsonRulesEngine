# Quick Start Guide

This guide will help you get started with JsonRulesEngine quickly.

## Installation

Install the JsonRulesEngine package from NuGet:

```bash
dotnet add package JsonRulesEngine
```

## Basic Usage

Here's a simple example of how to use JsonRulesEngine:

```csharp
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Create a rule
var rule = new Rule(
    "age-rule",
    new TopLevelCondition("all", new[] {
        new Condition("age", "greaterThan", 30)
    }),
    new Event("person-over-30")
);

// Create an engine
var engine = new Engine();
engine.AddRule(rule);

// Define facts
var facts = new Dictionary<string, object>
{
    { "age", 35 }
};

// Run the engine
var results = await engine.Run(facts);

// Check triggered events
foreach (var event in results.Events)
{
    Console.WriteLine($"Event triggered: {event.Type}");
}
```

## Creating Rules

Rules consist of conditions and events. When all conditions are met, the event is triggered.

### Condition Operators

JsonRulesEngine supports the following operators:

#### Comparison Operators
- `equal`: Checks if values are equal
- `notEqual`: Checks if values are not equal
- `greaterThan`: Checks if a value is greater than another
- `greaterThanInclusive`: Checks if a value is greater than or equal to another
- `lessThan`: Checks if a value is less than another
- `lessThanInclusive`: Checks if a value is less than or equal to another

#### Collection Operators
- `in`: Checks if a value is in a collection
- `notIn`: Checks if a value is not in a collection
- `contains`: Checks if a collection contains a value
- `doesNotContain`: Checks if a collection does not contain a value

#### String Operators
- `startsWith`: Checks if a string starts with another string
- `endsWith`: Checks if a string ends with another string
- `stringContains`: Checks if a string contains a substring
- `matches`: Checks if a string matches a regular expression pattern

### Boolean Operators

You can combine conditions using boolean operators:

- `all`: All conditions must be true (AND)
- `any`: At least one condition must be true (OR)

Example:

```csharp
var rule = new Rule(
    "complex-rule",
    new TopLevelCondition("any", new[] {
        new Condition("age", "greaterThan", 30),
        new Condition("name", "equal", "John")
    }),
    new Event("match-found")
);
```

## Working with Facts

Facts are the data that rules are evaluated against. They can be static values or dynamic functions.

### Static Facts

Static facts are simple key-value pairs:

```csharp
var facts = new Dictionary<string, object>
{
    { "age", 35 },
    { "name", "John" }
};
```

### Path Resolution

You can access nested properties in facts using JSON path:

```csharp
var rule = new Rule(
    "person-rule",
    new TopLevelCondition("all", new[] {
        new Condition("person", "equal", "John", "$.name")
    }),
    new Event("found-john")
);

var facts = new Dictionary<string, object>
{
    { "person", new { name = "John", age = 35 } }
};
```

## Using String Operators

String operators provide powerful text comparison capabilities. Here are examples of using the string operators:

### StartsWith Example

```csharp
var rule = new Rule(
    "email-rule",
    new TopLevelCondition("all", new[] {
        new Condition("email", "startsWith", "admin")
    }),
    new Event("admin-email-found")
);

var facts = new Dictionary<string, object>
{
    { "email", "admin@example.com" }
};
```

### EndsWith Example

```csharp
var rule = new Rule(
    "domain-rule",
    new TopLevelCondition("all", new[] {
        new Condition("email", "endsWith", "@company.com")
    }),
    new Event("company-email-found")
);

var facts = new Dictionary<string, object>
{
    { "email", "john@company.com" }
};
```

### StringContains Example

```csharp
var rule = new Rule(
    "keyword-rule",
    new TopLevelCondition("all", new[] {
        new Condition("message", "stringContains", "urgent")
    }),
    new Event("urgent-message-found")
);

var facts = new Dictionary<string, object>
{
    { "message", "Please review this urgent request" }
};
```

### Matches (Regex) Example

```csharp
var rule = new Rule(
    "zipcode-rule",
    new TopLevelCondition("all", new[] {
        new Condition("zipcode", "matches", "^\\d{5}(-\\d{4})?$")
    }),
    new Event("valid-zipcode")
);

var facts = new Dictionary<string, object>
{
    { "zipcode", "12345-6789" }
};
```

## Events

Events are triggered when rules match. They can include parameters that reference fact values:

```csharp
var eventParams = new Dictionary<string, object>
{
    { "personName", "{{name}}" },
    { "personAge", "{{age}}" }
};

var rule = new Rule(
    "param-rule",
    new TopLevelCondition("all", new[] { 
        new Condition("age", "greaterThan", 20) 
    }),
    new Event("person-matched", eventParams)
);
```

When the rule matches, `event.Params["personName"]` will contain the value of the "name" fact.

## Creating Custom Operators

You can create custom operators by extending the `Operator` class:

```csharp
public class IsEvenOperator : Operator, IOperator<int, object>
{
    public override string Name => "isEven";
    
    public override bool Evaluate(object? factValue, object? compareToValue)
    {
        if (factValue is int intValue)
            return intValue % 2 == 0;
            
        return false;
    }
    
    bool IOperator<int, object>.Evaluate(int factValue, object? compareToValue)
    {
        return factValue % 2 == 0;
    }
}
```

Register your custom operator:

```csharp
var registry = new OperatorRegistry();
registry.AddOperator(new IsEvenOperator());

var engine = new Engine(operatorRegistry: registry);
```

## Performance Tips

JsonRulesEngine provides several features for optimizing performance:

### Fact Caching

Facts can be cached to avoid redundant computation:

```csharp
// Enable fact caching with default settings
var engine = new Engine();  // Caching is enabled by default

// Create a fact with caching enabled
var fact = new Fact("expensiveCalculation", CalculateValue, 
    new FactOptions { Cache = true });

// Disable caching for volatile data
var volatileFact = new Fact("realTimeMetric", GetCurrentValue, 
    new FactOptions { Cache = false });
```

For more advanced caching options including expiration and cache size limits, see the [Advanced Usage Guide](advanced.md#advanced-fact-caching).

## Next Steps

For more detailed information, check out these resources:

- [Operators Guide](operators.md) - Detailed information about all operators
- [Advanced Usage](advanced.md) - Advanced techniques and patterns
- [Troubleshooting](troubleshooting.md) - Common issues and solutions
- [API Reference](../api/index.md) - Complete API documentation
