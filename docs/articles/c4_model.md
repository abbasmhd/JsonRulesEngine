# C4 Model for JsonRulesEngine

This document describes the architecture of the JsonRulesEngine library using the C4 model.

## Context Diagram

```
┌─────────────────────┐
│                     │
│  Client Application │
│                     │
└──────────┬──────────┘
           │
           │ Uses
           ▼
┌─────────────────────┐
│                     │
│   JsonRulesEngine   │
│                     │
└─────────────────────┘
```

The JsonRulesEngine library is used by client applications to evaluate business rules against facts and trigger events when rules match.

## Container Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│                        JsonRulesEngine                          │
│                                                                 │
│  ┌───────────────┐    ┌───────────────┐    ┌───────────────┐   │
│  │               │    │               │    │               │   │
│  │     Engine    │───▶│    Almanac    │───▶│     Facts     │   │
│  │               │    │               │    │               │   │
│  └───────┬───────┘    └───────────────┘    └───────────────┘   │
│          │                                                      │
│          │                                                      │
│          ▼                                                      │
│  ┌───────────────┐    ┌───────────────┐    ┌───────────────┐   │
│  │               │    │               │    │               │   │
│  │     Rules     │───▶│   Conditions  │───▶│   Operators   │   │
│  │               │    │               │    │               │   │
│  └───────┬───────┘    └───────────────┘    └───────────────┘   │
│          │                                                      │
│          │                                                      │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │               │                                              │
│  │    Events     │                                              │
│  │               │                                              │
│  └───────────────┘                                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

The JsonRulesEngine library consists of several components that work together to evaluate rules against facts and trigger events.

## Component Diagram

```
┌───────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                                   │
│                                           Engine                                                  │
│                                                                                                   │
│  ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────────────┐   │
│  │                 │   │                 │   │                 │   │                         │   │
│  │  Add/Remove     │   │  Update         │   │  Run            │   │  Stop                   │   │
│  │  Rules          │   │  Rules          │   │  Evaluation     │   │  Evaluation             │   │
│  │                 │   │                 │   │                 │   │                         │   │
│  └─────────────────┘   └─────────────────┘   └────────┬────────┘   └─────────────────────────┘   │
│                                                       │                                           │
│                                                       ▼                                           │
│                                         ┌─────────────────────────┐                               │
│                                         │                         │                               │
│                                         │  Evaluate Conditions    │                               │
│                                         │                         │                               │
│                                         └─────────────────────────┘                               │
│                                                                                                   │
└───────────────────────────────────────────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                                   │
│                                           Almanac                                                 │
│                                                                                                   │
│  ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐                                  │
│  │                 │   │                 │   │                 │                                  │
│  │  Retrieve       │   │  Add Runtime    │   │  Add            │                                  │
│  │  Fact Values    │   │  Facts          │   │  Facts          │                                  │
│  │                 │   │                 │   │                 │                                  │
│  └─────────────────┘   └─────────────────┘   └─────────────────┘                                  │
│                                                                                                   │
└───────────────────────────────────────────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                                   │
│                                        Operators                                                  │
│                                                                                                   │
│  ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────────────┐   │
│  │                 │   │                 │   │                 │   │                         │   │
│  │  Comparison     │   │  Logical        │   │  Collection     │   │  Custom                 │   │
│  │  Operators      │   │  Operators      │   │  Operators      │   │  Operators              │   │
│  │                 │   │                 │   │                 │   │                         │   │
│  └─────────────────┘   └─────────────────┘   └─────────────────┘   └─────────────────────────┘   │
│                                                                                                   │
└───────────────────────────────────────────────────────────────────────────────────────────────────┘
```

## Code Diagram

The JsonRulesEngine library is organized into the following namespaces:

- **JsonRulesEngine.Core**: Contains the main engine and core components
- **JsonRulesEngine.Core.Models**: Contains model classes for rules, conditions, events, etc.
- **JsonRulesEngine.Core.Interfaces**: Contains interfaces for the core components
- **JsonRulesEngine.Core.Operators**: Contains operator implementations
- **JsonRulesEngine.Core.Extensions**: Contains extension methods

Key classes and their relationships:

```
IEngine <|-- Engine
IAlmanac <|-- Almanac
IPathResolver <|-- JsonPathResolver
IOperator <|-- Operator <|-- EqualOperator, NotEqualOperator, etc.

Engine *-- Rule
Rule *-- TopLevelCondition
TopLevelCondition *-- Condition
Engine *-- Almanac
Almanac *-- Fact
Engine *-- OperatorRegistry
OperatorRegistry *-- Operator
```

This architecture follows the design of the original JavaScript library while adapting it to C# conventions and best practices.
