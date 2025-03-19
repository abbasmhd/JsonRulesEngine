# JsonRulesEngine Documentation

## Introduction

JsonRulesEngine is a C# port of the [json-rules-engine](https://github.com/cachecontrol/json-rules-engine) library for evaluating JSON rules against facts. It provides a powerful, lightweight rules engine for .NET applications.

## Getting Started

To get started with JsonRulesEngine, install the NuGet package:

```bash
dotnet add package JsonRulesEngine
```

Then, you can create rules, define facts, and run the engine to evaluate the rules against the facts.

## Features

- Define rules with conditions in JSON format
- Support for boolean operators (ALL/ANY)
- Dynamic fact loading
- Path resolution within fact objects
- Event triggering when rules match
- Extensible operator system

## Examples

Check out the [Quick Start](articles/quickstart.md) guide for examples of how to use JsonRulesEngine.

## API Documentation

For detailed API documentation, see the [API Reference](api/index.md).
