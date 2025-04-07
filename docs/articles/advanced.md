# Advanced Usage Guide

This guide covers advanced features and techniques for getting the most out of JsonRulesEngine.

## Custom Operators

One of the most powerful features of JsonRulesEngine is the ability to create custom operators. This allows you to extend the engine with domain-specific logic.

### Creating a Custom Operator

To create a custom operator, follow these steps:

1. Create a class that extends `Operator`
2. Implement the `IOperator<TFactValue, TCompareValue>` interface
3. Override the `Name` property
4. Implement the `Evaluate` method

Here's an example of a custom operator that checks if a number is even:

```csharp
using System;
using JsonRulesEngine.Core.Interfaces;
using JsonRulesEngine.Core.Operators;

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

Key points:
- The `Name` property defines how the operator will be referenced in rules
- The generic implementation (`IOperator<int, object>`) provides type safety and performance benefits
- The non-generic `Evaluate` method handles dynamic invocation with proper type checking
- The generic `Evaluate` method handles the strongly-typed case

### Registering a Custom Operator

Once you've created a custom operator, you need to register it with the operator registry:

```csharp
// Create and register the operator
var registry = new OperatorRegistry();
registry.AddOperator(new IsEvenOperator());

// Create the engine with the custom registry
var engine = new Engine(operatorRegistry: registry);
```

### Using a Custom Operator in Rules

After registering your custom operator, you can use it in rules:

```csharp
var rule = new Rule(
    "even-number-rule",
    new TopLevelCondition("all", new[] {
        new Condition("number", "isEven", null)
    }),
    new Event("even-number-found")
);

engine.AddRule(rule);

var facts = new Dictionary<string, object>
{
    { "number", 42 }
};

var results = await engine.Run(facts);
// Even number rule will match
```

### Operator Decorators

For more complex operators, you can use the decorator pattern to add functionality to existing operators. Implement the `IOperatorDecorator<TFactValue, TCompareValue, TNextFactValue, TNextCompareValue>` interface to create a decorator.

```csharp
public class CaseInsensitiveDecorator : IOperatorDecorator<string, string, string, string>
{
    public bool Evaluate(string factValue, string compareToValue, 
        Func<string, string, bool> next)
    {
        if (factValue == null || compareToValue == null)
            return next(factValue, compareToValue);
            
        return next(factValue.ToLower(), compareToValue.ToLower());
    }
}
```

## Advanced Path Resolution

JsonRulesEngine supports path resolution within fact objects, allowing you to access nested properties.

### Complex Path Expressions

You can use JSON path expressions to access deeply nested properties:

```csharp
var rule = new Rule(
    "nested-property-rule",
    new TopLevelCondition("all", new[] {
        new Condition("user", "equal", "admin", "$.account.permissions.role")
    }),
    new Event("admin-user-found")
);

var facts = new Dictionary<string, object>
{
    { "user", new { 
        account = new { 
            permissions = new { 
                role = "admin" 
            } 
        } 
    }}
};
```

### Array Access

You can access array elements using the JSON path syntax:

```csharp
var rule = new Rule(
    "array-access-rule",
    new TopLevelCondition("all", new[] {
        new Condition("data", "equal", "important", "$.tags[0]")
    }),
    new Event("important-data-found")
);

var facts = new Dictionary<string, object>
{
    { "data", new { 
        tags = new[] { "important", "urgent", "review" } 
    }}
};
```

### Multiple Path Expressions

You can combine multiple path expressions in a single rule:

```csharp
var rule = new Rule(
    "complex-path-rule",
    new TopLevelCondition("all", new[] {
        new Condition("user", "equal", "admin", "$.account.role"),
        new Condition("user", "greaterThan", 1000, "$.account.permissions.level"),
        new Condition("user", "contains", "manage_users", "$.account.permissions.capabilities")
    }),
    new Event("admin-with-user-management")
);
```

## Complex Rule Scenarios

JsonRulesEngine can handle complex rule scenarios by combining multiple conditions, nesting boolean operators, and using path resolution.

### Nested Boolean Operators

You can nest boolean operators to create complex logic:

```csharp
var rule = new Rule(
    "complex-nested-rule",
    new TopLevelCondition("all", new[] {
        new Condition("user", "equal", "active", "$.status"),
        new TopLevelCondition("any", new[] {
            new TopLevelCondition("all", new[] {
                new Condition("user", "equal", "admin", "$.role"),
                new Condition("user", "greaterThanInclusive", 5, "$.level")
            }),
            new TopLevelCondition("all", new[] {
                new Condition("user", "equal", "manager", "$.role"),
                new Condition("user", "greaterThanInclusive", 8, "$.level")
            })
        })
    }),
    new Event("authorized-user")
);
```

This rule will match if:
- The user status is "active" AND
- EITHER:
  - The user role is "admin" AND level >= 5, OR
  - The user role is "manager" AND level >= 8

### Dynamic Fact Loading

You can use the fact system to load facts dynamically:

```csharp
// Create a dynamic fact loader
var userRepository = new UserRepository();
var userLoader = new Fact("user", async (almanac, factValue) => {
    var userId = await almanac.GetFact("userId");
    return await userRepository.GetUserById(userId);
});

// Register the fact loader with the engine
engine.AddFactLoader(userLoader);

// Run with just the userId
var facts = new Dictionary<string, object>
{
    { "userId", 123 }
};

var results = await engine.Run(facts);
```

### Rule Priorities

You can control the order in which rules are evaluated using priorities:

```csharp
var rule1 = new Rule(
    "high-priority-rule",
    new TopLevelCondition("all", new[] {
        new Condition("value", "greaterThan", 100)
    }),
    new Event("high-value"),
    priority: 10  // Higher priority
);

var rule2 = new Rule(
    "low-priority-rule",
    new TopLevelCondition("all", new[] {
        new Condition("value", "greaterThan", 50)
    }),
    new Event("medium-value"),
    priority: 5  // Lower priority
);

engine.AddRule(rule1);
engine.AddRule(rule2);
```

Rules with higher priority values are evaluated first.

## Error Handling

Proper error handling is essential for robust rule evaluation.

### Handling Rule Evaluation Errors

Use try-catch blocks to handle rule evaluation errors:

```csharp
try
{
    var results = await engine.Run(facts);
    // Process results
}
catch (RuleEvaluationException ex)
{
    // Handle rule evaluation errors
    Console.WriteLine($"Rule evaluation failed: {ex.Message}");
}
catch (Exception ex)
{
    // Handle other errors
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### Validation Before Evaluation

Validate rules before adding them to the engine:

```csharp
bool IsRuleValid(Rule rule)
{
    if (string.IsNullOrEmpty(rule.Name))
        return false;
        
    if (rule.Event == null)
        return false;
        
    if (rule.Conditions == null)
        return false;
        
    // Additional validation logic
    
    return true;
}

// Usage
if (IsRuleValid(rule))
{
    engine.AddRule(rule);
}
else
{
    Console.WriteLine("Invalid rule configuration");
}
```

### Debugging Rules

Enable debug mode to get detailed information about rule evaluation:

```csharp
var engine = new Engine(debug: true);
```

In debug mode, the engine will log detailed information about each step of the rule evaluation process.

### Custom Error Handling

Implement custom error handling for specific scenarios:

```csharp
public class SafeEngine : Engine
{
    public override async Task<EngineResult> Run(Dictionary<string, object> facts)
    {
        try
        {
            return await base.Run(facts);
        }
        catch (Exception ex)
        {
            // Log the error
            Logger.LogError(ex, "Rule evaluation failed");
            
            // Return a safe default result
            return new EngineResult
            {
                Events = new List<Event>(),
                Rules = _rules.Select(r => new RuleResult
                {
                    Rule = r,
                    Result = false,
                    Error = ex.Message
                }).ToList()
            };
        }
    }
}
```

## Performance Optimization

For optimal performance, consider these techniques:

### Fact Caching

Enable fact caching to avoid redundant fact resolution:

```csharp
var engine = new Engine(enableFactCaching: true);
```

### Optimized Rule Order

Organize rules to minimize unnecessary evaluations:
- Put specific rules before general rules
- Put frequently matching rules before rarely matching rules
- Group related rules together

### Rule Indexing

For large rule sets, consider implementing a rule indexing system to quickly filter applicable rules:

```csharp
public class IndexedEngine : Engine
{
    private Dictionary<string, List<Rule>> _ruleIndex = new Dictionary<string, List<Rule>>();
    
    public override void AddRule(Rule rule)
    {
        base.AddRule(rule);
        
        // Index the rule by fact references
        foreach (var fact in GetReferencedFacts(rule))
        {
            if (!_ruleIndex.ContainsKey(fact))
                _ruleIndex[fact] = new List<Rule>();
                
            _ruleIndex[fact].Add(rule);
        }
    }
    
    protected override IEnumerable<Rule> GetApplicableRules(Dictionary<string, object> facts)
    {
        // Get rules that reference available facts
        var applicableRules = new HashSet<Rule>();
        
        foreach (var fact in facts.Keys)
        {
            if (_ruleIndex.ContainsKey(fact))
                applicableRules.UnionWith(_ruleIndex[fact]);
        }
        
        return applicableRules.OrderByDescending(r => r.Priority);
    }
    
    private IEnumerable<string> GetReferencedFacts(Rule rule)
    {
        // Extract fact references from the rule
        // Implementation depends on rule structure
        // ...
    }
}
```

## Advanced Fact Caching

JsonRulesEngine includes a sophisticated fact caching system to improve performance when evaluating rules with expensive fact computations. This section explains how to configure and use the advanced caching features.

### Configuring Global Cache Settings

You can control caching behavior at the engine level:

```csharp
var engineOptions = new EngineOptions
{
    EnableFactCaching = true,  // Enable or disable caching globally
    PathResolver = new JsonPathResolver()
};
var engine = new Engine(options: engineOptions);
```

You can also configure the almanac directly with more detailed settings:

```csharp
var almanacOptions = new AlmanacOptions
{
    EnableFactCaching = true,  // Enable or disable caching globally
    CacheMaxSize = 1000,       // Maximum number of cached entries
    PathResolver = new JsonPathResolver()
};
var almanac = new Almanac(facts, almanacOptions);
```

### Per-Fact Cache Configuration

Individual facts can have their own cache settings:

```csharp
// Create a fact with caching enabled and 60 second expiration
var factOptions = new FactOptions
{
    Cache = true,                  // Enable caching for this fact
    CacheExpirationInSeconds = 60  // Expire cache entries after 60 seconds
};

var fact = new Fact("userStatus", async (params, almanac) => {
    // Expensive operation to fetch user status
    return await userService.GetUserStatusAsync(params["userId"]);
}, factOptions);
```

### Time-Based Cache Expiration

You can set cache entries to expire after a specific time:

```csharp
// Cache fact values for 5 minutes
var fact = new Fact("weather", GetWeatherData, new FactOptions 
{ 
    Cache = true, 
    CacheExpirationInSeconds = 300 // 5 minutes
});
```

This is useful for:
- Frequently changing data
- External API calls
- Resource-intensive calculations

### Cache Size Limits

To prevent memory issues, you can limit the cache size:

```csharp
var options = new AlmanacOptions
{
    EnableFactCaching = true,
    CacheMaxSize = 500,  // Store a maximum of 500 cache entries
    PathResolver = new JsonPathResolver()
};
```

When the cache reaches its limit, the oldest entries are automatically removed (LRU - Least Recently Used).

### Clearing and Invalidating Cache

You can programmatically clear the cache:

```csharp
// Clear the entire cache
almanac.ClearCache();

// Invalidate a specific fact
almanac.InvalidateCache("userStatus");
```

This is useful when:
- Facts become stale
- External data changes
- You need to force recalculation

### Caching with Parameters

Facts that accept parameters are cached using a composite key:

```csharp
// This fact will be cached separately for each userId
var userFact = new Fact("user", async (params, almanac) => {
    return await userRepository.GetUserById(params["userId"]);
}, new FactOptions { Cache = true });

// Each call with a different userId gets its own cache entry
await almanac.FactValue("user", new Dictionary<string, object> { ["userId"] = 123 });
await almanac.FactValue("user", new Dictionary<string, object> { ["userId"] = 456 });
```

### Best Practices

- **Enable caching for expensive operations**: API calls, database queries, complex calculations
- **Disable caching for volatile data**: Real-time metrics, rapidly changing values
- **Set appropriate expiration times**: Match cache lifetime to data freshness requirements
- **Set appropriate cache size limits**: Consider memory constraints and typical rule evaluations
- **Monitor cache performance**: Track hit rates and eviction frequency in production

## Next Steps

- [Operators Guide](operators.md) - Detailed information about all operators
- [Troubleshooting](troubleshooting.md) - Common issues and solutions
- [API Reference](../api/index.md) - Full API documentation 