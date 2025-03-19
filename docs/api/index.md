# API Documentation

Welcome to the API documentation for JsonRulesEngine. This section provides detailed information about the classes, interfaces, and methods available in the library.

## Namespaces

- **JsonRulesEngine.Core**: Contains the main engine and core components
- **JsonRulesEngine.Core.Models**: Contains model classes for rules, conditions, events, etc.
- **JsonRulesEngine.Core.Interfaces**: Contains interfaces for the core components
- **JsonRulesEngine.Core.Operators**: Contains operator implementations
- **JsonRulesEngine.Core.Extensions**: Contains extension methods

## Key Components

### Engine

The `Engine` class is the main entry point for the rules engine. It manages rules and evaluates them against facts.

### Rule

The `Rule` class represents a rule with conditions and an event to trigger when the conditions are met.

### Almanac

The `Almanac` class manages facts during rule evaluation.

### Fact

The `Fact` class represents a data point that can be static or dynamically loaded.

### Condition

The `Condition` class represents a condition in a rule.

### Event

The `Event` class represents an event that is triggered when a rule's conditions are met.

### Operators

The library includes various operators for comparing fact values against condition values.

## Getting Started

For a quick introduction to using the library, see the [Quick Start Guide](../articles/quickstart.md).
