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

- `equal`: Checks if values are equal
- `notEqual`: Checks if values are not equal
- `greaterThan`: Checks if a value is greater than another
- `greaterThanInclusive`: Checks if a value is greater than or equal to another
- `lessThan`: Checks if a value is less than another
- `lessThanInclusive`: Checks if a value is less than or equal to another
- `in`: Checks if a value is in a collection
- `notIn`: Checks if a value is not in a collection
- `contains`: Checks if a collection contains a value
- `doesNotContain`: Checks if a collection does not contain a value

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

## Next Steps

For more advanced usage, check out the [API Reference](../api/index.md).
