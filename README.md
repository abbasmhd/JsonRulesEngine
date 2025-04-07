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
- Performance optimization with fact caching

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

### Using Fact Caching for Performance

```csharp
// Enable fact caching at the engine level
var options = new EngineOptions
{
    EnableFactCaching = true,
    AlmanacOptions = new AlmanacOptions
    {
        CacheMaxSize = 100  // Limit cache to 100 entries
    }
};
var engine = new Engine(options);

// Create a fact with caching enabled
var fact = new Fact<int>("userAge", () => 
{
    // Expensive operation that retrieves user age
    return 35;
}, new FactOptions
{
    Cache = true, 
    CacheExpirationInSeconds = 60  // Cache for 1 minute
});

// Add the fact to the almanac
var almanac = new Almanac();
almanac.AddFact(fact);

// Clear cache when needed
almanac.ClearCache();

// Or invalidate specific fact
almanac.InvalidateCache("userAge");
```

## Performance Benchmarks

The following benchmarks demonstrate the performance improvements achieved with fact caching:

| Method                      | Mean        | Error    | StdDev   | Ratio | RatioSD |
|---------------------------- |------------:|---------:|---------:|------:|--------:|
| RunWithoutCaching           |    31.78 ms | 0.617 ms | 0.758 ms |  1.00 |    0.03 |
| RunWithCaching              |    15.86 ms | 0.314 ms | 0.430 ms |  0.50 |    0.02 |
| RunWithCachingMultipleRules |    15.93 ms | 0.310 ms | 0.444 ms |  0.50 |    0.02 |
| RunWithCacheExpiration      | 1,132.84 ms | 1.774 ms | 1.482 ms | 35.67 |    0.85 |

// * Hints *
Outliers
  FactCachingBenchmarks.RunWithCaching: Default              -> 2 outliers were removed, 3 outliers were detected (14.52 ms, 17.00 ms, 17.00 ms)
  FactCachingBenchmarks.RunWithCachingMultipleRules: Default -> 1 outlier  was  removed (17.03 ms)
  FactCachingBenchmarks.RunWithCacheExpiration: Default      -> 2 outliers were removed (1.14 s, 1.15 s)

// * Legends *
  Mean    : Arithmetic mean of all measurements
  Error   : Half of 99.9% confidence interval
  StdDev  : Standard deviation of all measurements
  Ratio   : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD : Standard deviation of the ratio distribution ([Current]/[Baseline])
  1 ms    : 1 Millisecond (0.001 sec)

These benchmarks were performed with rules that reference the same fact multiple times, showing how caching can significantly reduce redundant fact resolution.

These benchmark results clearly demonstrate the performance benefits of the caching implementation:

**RunWithoutCaching (baseline)**: 31.78 ms
- This is our baseline with no caching

**RunWithCaching**: 15.86 ms (50% of baseline)
- With caching enabled, performance is twice as fast
- This confirms that caching prevents redundant fact resolution

**RunWithCachingMultipleRules**: 15.93 ms (50% of baseline)
- Maintains the same performance benefit across multiple rule evaluations
- Shows that cached values persist effectively between rule runs

**RunWithCacheExpiration**: 1,132.84 ms
- This is much higher because of the Thread.Sleep(1100) we included to wait for cache expiration
- Confirms that expired cache entries are properly invalidated after the expiration period

The 50% performance improvement is significant and demonstrates that our caching implementation effectively reduces the overhead of repeated fact resolution.

## License

MIT
