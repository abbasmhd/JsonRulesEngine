# C4 Model for JsonRulesEngine

This document provides a C4 model representation of the JsonRulesEngine architecture.

## Level 1: System Context Diagram

```mermaid
graph TB
    Client["Client Application"]
    JsonRulesEngine["JsonRulesEngine"]
    ExternalData["External Data Sources"]

    Client -->|Uses| JsonRulesEngine
    Client -->|Uses| ExternalData
```

The JsonRulesEngine is used by client applications to evaluate business rules against data that may come from various sources, including external systems.

## Level 2: Container Diagram

```mermaid
graph TB
    Client["Client Application"]
    JsonRulesEngine["JsonRulesEngine<br/>NuGet Package"]
    ExternalData["External Data Sources"]

    Client -->|Uses| JsonRulesEngine
    JsonRulesEngine -->|Reads| ExternalData
```

The JsonRulesEngine is packaged as a NuGet package that can be integrated into .NET applications.

## Level 3: Component Diagram

```mermaid
graph TB
    subgraph JsonRulesEngine["JsonRulesEngine NuGet Package"]
        Engine["Engine"]
        Rule["Rule"]
        Event["Event"]
        Almanac["Almanac"]
        Condition["Condition"]
        Fact["Fact"]
        OperatorMap["OperatorMap"]

        Engine -->|Uses| Rule
        Rule -->|Triggers| Event
        Engine -->|Uses| Almanac
        Rule -->|Contains| Condition
        Almanac -->|Manages| Fact
        Fact -->|Uses| OperatorMap
    end

    Client["Client Application"]
    Client -->|Uses| Engine
```

## Level 4: Code Diagram

```mermaid
classDiagram
    class Engine {
        +List~Rule~ Rules
        +Dictionary~string, Fact~ Facts
        +OperatorMap OperatorMap
        +Dictionary~string, TopLevelCondition~ Conditions
        +AddRule(rule: Rule) void
        +UpdateRule(rule: Rule) void
        +RemoveRule(rule: Rule) bool
        +AddFact(id: string, definitionFunc: Func) void
        +RemoveFact(id: string) bool
        +AddOperator(name: string, evaluateFunc: Func) void
        +Run(facts: Dictionary~string, object~) Task~EngineResult~
    }

    class Rule {
        +string Id
        +int Priority
        +TopLevelCondition Conditions
        +Event Event
        +ToJson() string
        +FromJson(json: string) Rule
    }

    class TopLevelCondition {
        +string BooleanOperator
        +List~Condition~ Conditions
        +Evaluate(almanac: Almanac, operatorMap: OperatorMap) Task~bool~
    }

    class Condition {
        +string Fact
        +string Operator
        +object Value
        +string Path
        +Evaluate(almanac: Almanac, operatorMap: OperatorMap) Task~bool~
    }

    class Almanac {
        +Dictionary~string, Fact~ Facts
        +Dictionary~string, object~ FactCache
        +IPathResolver PathResolver
        +FactValue(factId: string, params: Dictionary) Task~object~
        +AddRuntimeFact(factId: string, value: object) void
        +AddFact(id: string, valueCallback: Func) void
    }

    class Fact {
        +string Id
        +Func ValueCallback
        +FactOptions Options
    }

    class OperatorMap {
        +Dictionary~string, Func~ Operators
        +Dictionary~string, Func~ OperatorDecorators
        +AddOperator(name: string, evaluateFunc: Func) void
        +RemoveOperator(name: string) bool
        +GetOperator(name: string) Func
    }

    class Event {
        +string Type
        +Dictionary~string, object~ Params
    }

    Engine --> Rule
    Rule --> TopLevelCondition
    TopLevelCondition --> Condition
    Engine --> Almanac
    Almanac --> Fact
    Engine --> OperatorMap
    Rule --> Event
```

## Relationships

1. **Engine** manages multiple **Rules**, **Facts**, and **Operators**
2. **Rule** contains a **TopLevelCondition** and an **Event**
3. **TopLevelCondition** contains multiple **Conditions**
4. **Condition** references a **Fact** and an **Operator**
5. **Almanac** manages **Facts** and their values
6. **OperatorMap** manages **Operators** and **OperatorDecorators**

## Behavioral Aspects

1. Client applications create an **Engine** instance
2. Rules are added to the **Engine** via `AddRule()`
3. Facts are defined via `AddFact()`
4. When `Run()` is called:
   - The **Engine** creates an **Almanac** instance
   - Each **Rule** is evaluated based on its **Conditions**
   - **Facts** are retrieved or computed as needed
   - **Operators** compare fact values against condition values
   - If a rule's conditions are met, its **Event** is triggered
   - The **Engine** returns an **EngineResult** with all triggered events
