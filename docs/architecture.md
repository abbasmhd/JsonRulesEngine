# JsonRulesEngine C# Architecture

This document outlines the architecture for the C# port of the json-rules-engine JavaScript library.

## Overview

JsonRulesEngine is a rules engine that allows defining rules in JSON format, with support for boolean operators (ALL/ANY), fact evaluation, and event triggering when rules match.

## Core Components

### Engine

The main class that manages rules, facts, operators, and executes rule evaluation.

```csharp
public class Engine
{
    // Constructor with optional rules and options
    public Engine(IEnumerable<Rule> rules = null, EngineOptions options = null);
    
    // Add a rule to the engine
    public void AddRule(Rule rule);
    
    // Update an existing rule
    public void UpdateRule(Rule rule);
    
    // Remove a rule by instance or ID
    public bool RemoveRule(Rule rule);
    public bool RemoveRule(string ruleId);
    
    // Add a fact definition
    public void AddFact(string id, Func<IDictionary<string, object>, Almanac, Task<object>> definitionFunc, FactOptions options = null);
    
    // Remove a fact by ID
    public bool RemoveFact(string id);
    
    // Get a fact by ID
    public Fact GetFact(string id);
    
    // Add a custom operator
    public void AddOperator(string operatorName, Func<object, object, bool> evaluateFunc);
    
    // Remove an operator
    public bool RemoveOperator(string operatorName);
    
    // Add an operator decorator
    public void AddOperatorDecorator(string decoratorName, Func<object, object, Func<object, object, bool>, bool> evaluateFunc);
    
    // Remove an operator decorator
    public bool RemoveOperatorDecorator(string decoratorName);
    
    // Set a named condition that can be referenced by rules
    public void SetCondition(string name, TopLevelCondition condition);
    
    // Remove a named condition
    public bool RemoveCondition(string name);
    
    // Run the engine with the provided facts
    public Task<EngineResult> Run(IDictionary<string, object> facts = null, RunOptions options = null);
    
    // Stop the engine
    public void Stop();
}
```

### Rule

Represents a rule with conditions and events to trigger.

```csharp
public class Rule
{
    // Properties
    public string Id { get; }
    public int Priority { get; set; }
    public TopLevelCondition Conditions { get; }
    public Event Event { get; }
    
    // Constructor
    public Rule(RuleProperties properties);
    
    // Serialize the rule to JSON
    public string ToJson();
    
    // Create a rule from JSON
    public static Rule FromJson(string json);
}
```

### Almanac

Manages facts and their values during rule evaluation.

```csharp
public class Almanac
{
    // Constructor
    public Almanac(AlmanacOptions options = null);
    
    // Get the value of a fact
    public Task<object> FactValue(string factId, IDictionary<string, object> params = null);
    
    // Add a fact at runtime
    public void AddRuntimeFact(string factId, object value);
    
    // Add a fact definition
    public void AddFact<T>(string id, Func<IDictionary<string, object>, Almanac, Task<T>> definitionFunc, FactOptions options = null);
}
```

### Fact

Represents data points that can be static or dynamically loaded.

```csharp
public class Fact
{
    // Properties
    public string Id { get; }
    public Func<IDictionary<string, object>, Almanac, Task<object>> ValueCallback { get; }
    public FactOptions Options { get; }
    
    // Constructor
    public Fact(string id, Func<IDictionary<string, object>, Almanac, Task<object>> valueCallback, FactOptions options = null);
}
```

### Condition

Represents a condition in a rule.

```csharp
public class Condition
{
    // Properties
    public string Fact { get; }
    public string Operator { get; }
    public object Value { get; }
    public string Path { get; }
    
    // Constructor
    public Condition(ConditionProperties properties);
    
    // Evaluate the condition
    public Task<bool> Evaluate(Almanac almanac, OperatorMap operatorMap);
}
```

### TopLevelCondition

Represents a top-level condition that can contain nested conditions with boolean operators.

```csharp
public class TopLevelCondition
{
    // Properties
    public string BooleanOperator { get; } // "all" or "any"
    public IEnumerable<Condition> Conditions { get; }
    
    // Constructor
    public TopLevelCondition(string booleanOperator, IEnumerable<Condition> conditions);
    
    // Evaluate the top-level condition
    public Task<bool> Evaluate(Almanac almanac, OperatorMap operatorMap);
}
```

### Event

Represents an event that is triggered when a rule's conditions are met.

```csharp
public class Event
{
    // Properties
    public string Type { get; }
    public IDictionary<string, object> Params { get; }
    
    // Constructor
    public Event(string type, IDictionary<string, object> params = null);
}
```

### OperatorMap

Manages the available operators and operator decorators.

```csharp
public class OperatorMap
{
    // Add an operator
    public void AddOperator(string operatorName, Func<object, object, bool> evaluateFunc);
    
    // Remove an operator
    public bool RemoveOperator(string operatorName);
    
    // Add an operator decorator
    public void AddOperatorDecorator(string decoratorName, Func<object, object, Func<object, object, bool>, bool> evaluateFunc);
    
    // Remove an operator decorator
    public bool RemoveOperatorDecorator(string decoratorName);
    
    // Get an operator
    public Func<object, object, bool> GetOperator(string operatorName);
}
```

## Options Classes

### EngineOptions

```csharp
public class EngineOptions
{
    public bool AllowUndefinedFacts { get; set; }
    public bool AllowUndefinedConditions { get; set; }
    public bool ReplaceFactsInEventParams { get; set; }
    public IPathResolver PathResolver { get; set; }
}
```

### FactOptions

```csharp
public class FactOptions
{
    public bool Cache { get; set; }
    public int Priority { get; set; }
}
```

### RunOptions

```csharp
public class RunOptions
{
    public Almanac Almanac { get; set; }
}
```

### EngineResult

```csharp
public class EngineResult
{
    public IEnumerable<Event> Events { get; }
    public IEnumerable<Event> FailureEvents { get; }
    public Almanac Almanac { get; }
    public IEnumerable<RuleResult> Results { get; }
    public IEnumerable<RuleResult> FailureResults { get; }
}
```

## Interfaces

### IPathResolver

```csharp
public interface IPathResolver
{
    object ResolveValue(object fact, string path);
}
```

## Default Implementations

### JsonPathResolver

```csharp
public class JsonPathResolver : IPathResolver
{
    public object ResolveValue(object fact, string path);
}
```

## Extension Points

The architecture allows for several extension points:

1. Custom operators via `Engine.AddOperator()`
2. Custom operator decorators via `Engine.AddOperatorDecorator()`
3. Custom path resolvers via the `IPathResolver` interface
4. Dynamic fact loading via fact definition functions

## Serialization

The library will support serialization and deserialization of rules to/from JSON using System.Text.Json.

## Async Support

The C# port will use Task-based asynchronous programming model to maintain the asynchronous nature of the original JavaScript library.
