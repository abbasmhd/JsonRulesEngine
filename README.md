# JsonRulesEngine

A C# port of the [json-rules-engine](https://github.com/cachecontrol/json-rules-engine) library for evaluating JSON rules against facts.

## Overview

JsonRulesEngine is a powerful, lightweight rules engine for .NET applications. It allows you to define rules in JSON format and evaluate them against facts to trigger events when conditions are met.

## Features

- Define rules with conditions in JSON format
- Support for boolean operators (ALL/ANY)
- Dynamic fact loading
- Path resolution within fact objects
- Event triggering when rules match
- Extensible operator system

## Installation

```bash
dotnet add package JsonRulesEngine
```

## Quick Start

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

## Advanced Usage

### Using Fact Path Resolution

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

### Using Multiple Conditions with ANY Operator

```csharp
var rule = new Rule(
    "any-rule",
    new TopLevelCondition("any", new[] {
        new Condition("age", "greaterThan", 30),
        new Condition("name", "equal", "John")
    }),
    new Event("match-found")
);
```

### Using Event Parameters with Fact Replacement

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

// When the rule matches, event.Params["personName"] will contain the value of the "name" fact
```

## License

MIT
