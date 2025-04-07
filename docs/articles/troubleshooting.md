# Troubleshooting Guide

This guide covers common issues you might encounter when using JsonRulesEngine, along with solutions, performance tips, and debugging techniques.

## Common Issues and Solutions

### Rules Not Matching As Expected

**Issue**: Rules aren't matching when you expect them to.

**Possible Causes and Solutions**:

1. **Incorrect Operator Selection**
   - **Symptom**: Rule fails to match despite the fact value appearing correct.
   - **Solution**: Double-check you're using the right operator. For example, using `equal` instead of `greaterThan`.

   ```csharp
   // Incorrect
   new Condition("age", "equal", 30)  // Only matches exactly 30
   
   // Correct
   new Condition("age", "greaterThanInclusive", 30)  // Matches 30 and above
   ```

2. **Type Mismatch**
   - **Symptom**: Comparisons fail despite values appearing correct.
   - **Solution**: Ensure fact values and comparison values have compatible types.

   ```csharp
   // Issue: String vs Int comparison
   facts["age"] = "30";  // String
   new Condition("age", "equal", 30)  // Int - Will not match
   
   // Solution: Use consistent types
   facts["age"] = 30;  // Int
   new Condition("age", "equal", 30)  // Int - Will match
   ```

3. **Case Sensitivity**
   - **Symptom**: String comparisons fail despite text appearing the same.
   - **Solution**: Remember string operators are case-sensitive by default.

   ```csharp
   // These will not match
   facts["status"] = "Active";
   new Condition("status", "equal", "active")
   
   // Solutions:
   // 1. Use consistent casing
   facts["status"] = "active";
   
   // 2. Or create a custom case-insensitive operator
   public class CaseInsensitiveEqualOperator : Operator, IOperator<string, string>
   {
       public override string Name => "caseInsensitiveEqual";
       
       public override bool Evaluate(object? factValue, object? compareToValue)
       {
           if (factValue == null && compareToValue == null)
               return true;
               
           if (factValue == null || compareToValue == null)
               return false;
               
           if (factValue is string s1 && compareToValue is string s2)
               return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
               
           return false;
       }
   }
   ```

4. **Path Resolution**
   - **Symptom**: Nested properties aren't being accessed correctly.
   - **Solution**: Verify your path expressions.

   ```csharp
   // Incorrect path
   new Condition("user", "equal", "admin", "$.account.permissions.roles")
   
   // Correct path (if the property is role, not roles)
   new Condition("user", "equal", "admin", "$.account.permissions.role")
   ```

5. **Boolean Operator Confusion**
   - **Symptom**: Complex conditions don't evaluate as expected.
   - **Solution**: Understand how `all` and `any` combine conditions.

   ```csharp
   // This requires BOTH conditions to be true
   new TopLevelCondition("all", new[] {
       new Condition("age", "greaterThan", 30),
       new Condition("status", "equal", "active")
   })
   
   // This requires EITHER condition to be true
   new TopLevelCondition("any", new[] {
       new Condition("age", "greaterThan", 30),
       new Condition("status", "equal", "active")
   })
   ```

### Rule Evaluation Exceptions

**Issue**: Exceptions are thrown during rule evaluation.

**Possible Causes and Solutions**:

1. **Invalid Path Expressions**
   - **Symptom**: `PathResolutionException` or `NullReferenceException`
   - **Solution**: Verify paths exist and handle nullable paths safely.

   ```csharp
   // Add null checks or use try-catch
   try
   {
       var results = await engine.Run(facts);
   }
   catch (PathResolutionException ex)
   {
       Console.WriteLine($"Path resolution failed: {ex.Message}");
   }
   ```

2. **Missing Fact Values**
   - **Symptom**: Rules fail with "Fact not found" errors.
   - **Solution**: Ensure all required facts are provided.

   ```csharp
   // Ensure all required facts are available
   if (!facts.ContainsKey("requiredFact"))
   {
       facts["requiredFact"] = defaultValue;
   }
   ```

3. **Invalid Operator Names**
   - **Symptom**: `KeyNotFoundException` when evaluating conditions.
   - **Solution**: Verify all operators are registered and spelled correctly.

   ```csharp
   // Verify operators are registered
   var registry = new OperatorRegistry();
   var operators = registry.GetAllOperators();
   foreach (var op in operators)
   {
       Console.WriteLine(op.Name);
   }
   ```

### Serialization Issues

**Issue**: Problems with JSON serialization or deserialization of rules.

**Possible Causes and Solutions**:

1. **Complex Types**
   - **Symptom**: Some properties aren't serialized correctly.
   - **Solution**: Implement custom converters for complex types.

   ```csharp
   // Custom JsonConverter for rules
   public class RuleConverter : JsonConverter<Rule>
   {
       // Implementation...
   }
   
   // Use with serializer
   var options = new JsonSerializerOptions
   {
       Converters = { new RuleConverter() }
   };
   ```

2. **Circular References**
   - **Symptom**: `JsonException` about cycles in object graph.
   - **Solution**: Use reference handling options or break circular references.

   ```csharp
   var options = new JsonSerializerOptions
   {
       ReferenceHandler = ReferenceHandler.Preserve
   };
   ```

## Performance Optimization

### Slow Rule Evaluation

**Issue**: Rule evaluation is taking too long.

**Solutions**:

1. **Enable Fact Caching**
   
   Fact caching stores resolved fact values to avoid redundant resolution.

   ```csharp
   var engine = new Engine(enableFactCaching: true);
   ```

2. **Optimize Rule Order**
   
   Evaluate fast-failing or frequently matching rules first.

   ```csharp
   // Add high-impact rules with higher priority
   engine.AddRule(criticalRule, priority: 100);
   engine.AddRule(secondaryRule, priority: 50);
   ```

3. **Use Efficient Path Expressions**
   
   Complex path expressions can be slow. Simplify when possible.

   ```csharp
   // Less efficient - traverses deep object graph
   new Condition("data", "equal", "value", "$.level1.level2.level3.property")
   
   // More efficient - restructure facts if possible
   facts["flattened_property"] = data.level1.level2.level3.property;
   new Condition("flattened_property", "equal", "value")
   ```

4. **Avoid Unnecessary Type Conversions**
   
   Type conversions add overhead. Use consistent types when possible.

5. **Profile and Optimize Bottlenecks**
   
   Use profiling tools to identify and address performance bottlenecks.

   ```csharp
   // Simple timing
   var stopwatch = Stopwatch.StartNew();
   var results = await engine.Run(facts);
   stopwatch.Stop();
   Console.WriteLine($"Rule evaluation took {stopwatch.ElapsedMilliseconds}ms");
   ```

### Optimizing Fact Caching

**Issue**: Cache performance is suboptimal or causing issues.

**Solutions**:

1. **Fine-tune Cache Expiration**
   
   Set appropriate expiration times based on data volatility:

   ```csharp
   // Frequently changing data: short expiration
   var volatileFact = new Fact("marketPrice", GetPrice, 
       new FactOptions { Cache = true, CacheExpirationInSeconds = 30 });
       
   // Relatively stable data: longer expiration
   var stableFact = new Fact("userProfile", GetProfile, 
       new FactOptions { Cache = true, CacheExpirationInSeconds = 3600 });
   ```

2. **Limit Cache Size**
   
   Prevent excessive memory usage with cache size limits:

   ```csharp
   var options = new AlmanacOptions
   {
       EnableFactCaching = true,
       CacheMaxSize = 500,  // Adjust based on expected usage
       PathResolver = new JsonPathResolver()
   };
   ```

3. **Selectively Disable Caching**
   
   Disable caching for facts that don't benefit from it:

   ```csharp
   // Real-time data that shouldn't be cached
   var realtimeFact = new Fact("currentTimestamp", () => DateTime.UtcNow, 
       new FactOptions { Cache = false });
   ```

4. **Invalidate Cache Strategically**
   
   Invalidate specific cached facts when data changes:

   ```csharp
   // After updating user data
   userRepository.UpdateUser(user);
   
   // Invalidate cached user facts
   almanac.InvalidateCache("user");
   ```

5. **Monitor Cache Performance**
   
   Add metrics to track cache effectiveness:

   ```csharp
   // Simple cache monitoring
   public class MonitoredAlmanac : Almanac
   {
       private int _cacheHits = 0;
       private int _cacheMisses = 0;
       
       public override async Task<object> FactValue(string factId, IDictionary<string, object>? @params = null)
       {
           // Implementation that counts hits and misses
           // ...
       }
       
       public double HitRate => _cacheHits / (_cacheHits + _cacheMisses);
   }
   ```

### Memory Usage

**Issue**: High memory usage with large rule sets or complex facts.

**Solutions**:

1. **Dispose Properly**
   
   Implement proper disposal patterns for any resources.

2. **Limit Fact Size**
   
   Only include necessary facts. Large fact objects consume more memory.

3. **Stream Large Data**
   
   For large data sets, use streaming approaches rather than loading everything into memory.

## Debugging Techniques

### Logging Rule Evaluation

Add logging to understand rule evaluation:

```csharp
public class LoggingEngine : Engine
{
    private readonly ILogger _logger;
    
    public LoggingEngine(ILogger logger)
    {
        _logger = logger;
    }
    
    protected override async Task<bool> EvaluateCondition(Condition condition, Almanac almanac)
    {
        var factValue = await almanac.GetFact(condition.Fact, condition.Path);
        _logger.LogInformation(
            "Evaluating condition: Fact '{fact}', Operator '{op}', Value '{value}', Result: {result}",
            condition.Fact,
            condition.Operator,
            condition.Value,
            result);
        
        return result;
    }
}
```

### Debug Mode

Enable debug mode to get detailed information:

```csharp
var engine = new Engine(debug: true);
```

### Rule Tracing

Implement rule tracing to understand evaluation flow:

```csharp
public class TracingEngine : Engine
{
    public override async Task<EngineResult> Run(Dictionary<string, object> facts)
    {
        Console.WriteLine("Starting rule evaluation with facts:");
        foreach (var fact in facts)
        {
            Console.WriteLine($"- {fact.Key}: {fact.Value}");
        }
        
        var result = await base.Run(facts);
        
        Console.WriteLine("\nEvaluation results:");
        foreach (var ruleResult in result.Rules)
        {
            Console.WriteLine($"- Rule '{ruleResult.Rule.Name}': {(ruleResult.Result ? "Matched" : "Not matched")}");
        }
        
        return result;
    }
}
```

### Unit Testing Rules

Create unit tests for your rules to verify behavior:

```csharp
public class RuleTests
{
    [Fact]
    public async Task AdminUserRule_WithAdminRole_ShouldMatch()
    {
        // Arrange
        var engine = new Engine();
        var rule = new Rule(
            "admin-rule",
            new TopLevelCondition("all", new[] {
                new Condition("user", "equal", "admin", "$.role")
            }),
            new Event("admin-found")
        );
        engine.AddRule(rule);
        
        var facts = new Dictionary<string, object>
        {
            { "user", new { role = "admin" } }
        };
        
        // Act
        var results = await engine.Run(facts);
        
        // Assert
        Assert.True(results.Events.Any(e => e.Type == "admin-found"));
    }
}
```

## Next Steps

- [Quick Start](quickstart.md) - Basic usage guide
- [Operators Guide](operators.md) - Detailed information about all operators
- [Advanced Usage](advanced.md) - Advanced techniques and patterns
- [API Reference](../api/index.md) - Complete API documentation 