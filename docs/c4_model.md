# C4 Model for JsonRulesEngine

This document provides a C4 model representation of the JsonRulesEngine architecture.

## Level 1: System Context Diagram

```
+-------------------+      +-------------------+
|                   |      |                   |
|  Client           |      |  External Data    |
|  Application      +----->+  Sources          |
|                   |      |                   |
+--------+----------+      +-------------------+
         |
         | Uses
         v
+-------------------+
|                   |
|  JsonRulesEngine  |
|                   |
+-------------------+
```

The JsonRulesEngine is used by client applications to evaluate business rules against data that may come from various sources, including external systems.

## Level 2: Container Diagram

```
+-------------------+
|                   |
|  Client           |
|  Application      |
|                   |
+--------+----------+
         |
         | Uses
         v
+-------------------+      +-------------------+
|                   |      |                   |
|  JsonRulesEngine  +----->+  External Data    |
|  NuGet Package    |      |  Sources          |
|                   |      |                   |
+-------------------+      +-------------------+
```

The JsonRulesEngine is packaged as a NuGet package that can be integrated into .NET applications.

## Level 3: Component Diagram

```
+-------------------+
|                   |
|  Client           |
|  Application      |
|                   |
+--------+----------+
         |
         | Uses
         v
+-----------------------------------------------------------+
|                                                           |
|  JsonRulesEngine NuGet Package                            |
|                                                           |
|  +---------------+      +---------------+      +--------+ |
|  |               |      |               |      |        | |
|  |  Engine       +----->+  Rule         +----->+ Event  | |
|  |               |      |               |      |        | |
|  +-------+-------+      +-------+-------+      +--------+ |
|          |                      |                         |
|          v                      v                         |
|  +---------------+      +---------------+                 |
|  |               |      |               |                 |
|  |  Almanac      +----->+  Condition    |                 |
|  |               |      |               |                 |
|  +-------+-------+      +---------------+                 |
|          |                                                |
|          v                                                |
|  +---------------+      +---------------+                 |
|  |               |      |               |                 |
|  |  Fact         +----->+  OperatorMap  |                 |
|  |               |      |               |                 |
|  +---------------+      +---------------+                 |
|                                                           |
+-----------------------------------------------------------+
```

## Level 4: Code Diagram

```
+------------------------------------------------------------------+
|                                                                  |
|  Engine                                                          |
|                                                                  |
|  - Rules: List<Rule>                                             |
|  - Facts: Dictionary<string, Fact>                               |
|  - OperatorMap: OperatorMap                                      |
|  - Conditions: Dictionary<string, TopLevelCondition>             |
|                                                                  |
|  + AddRule(rule: Rule): void                                     |
|  + UpdateRule(rule: Rule): void                                  |
|  + RemoveRule(rule: Rule): bool                                  |
|  + AddFact(id: string, definitionFunc: Func<...>): void          |
|  + RemoveFact(id: string): bool                                  |
|  + AddOperator(name: string, evaluateFunc: Func<...>): void      |
|  + Run(facts: Dictionary<string, object>): Task<EngineResult>    |
|                                                                  |
+------------------+-----------------------------------------------+
                   |
                   | Uses
                   v
+------------------------------------------------------------------+
|                                                                  |
|  Rule                                                            |
|                                                                  |
|  - Id: string                                                    |
|  - Priority: int                                                 |
|  - Conditions: TopLevelCondition                                 |
|  - Event: Event                                                  |
|                                                                  |
|  + ToJson(): string                                              |
|  + FromJson(json: string): Rule                                  |
|                                                                  |
+------------------+-----------------------------------------------+
                   |
                   | Contains
                   v
+------------------------------------------------------------------+
|                                                                  |
|  TopLevelCondition                                               |
|                                                                  |
|  - BooleanOperator: string ("all" or "any")                      |
|  - Conditions: List<Condition>                                   |
|                                                                  |
|  + Evaluate(almanac: Almanac, operatorMap: OperatorMap): Task<bool> |
|                                                                  |
+------------------+-----------------------------------------------+
                   |
                   | Contains
                   v
+------------------------------------------------------------------+
|                                                                  |
|  Condition                                                       |
|                                                                  |
|  - Fact: string                                                  |
|  - Operator: string                                              |
|  - Value: object                                                 |
|  - Path: string                                                  |
|                                                                  |
|  + Evaluate(almanac: Almanac, operatorMap: OperatorMap): Task<bool> |
|                                                                  |
+------------------------------------------------------------------+

+------------------------------------------------------------------+
|                                                                  |
|  Almanac                                                         |
|                                                                  |
|  - Facts: Dictionary<string, Fact>                               |
|  - FactCache: Dictionary<string, object>                         |
|  - PathResolver: IPathResolver                                   |
|                                                                  |
|  + FactValue(factId: string, params: Dictionary<string, object>): Task<object> |
|  + AddRuntimeFact(factId: string, value: object): void           |
|  + AddFact(id: string, valueCallback: Func<...>): void           |
|                                                                  |
+------------------+-----------------------------------------------+
                   |
                   | Uses
                   v
+------------------------------------------------------------------+
|                                                                  |
|  Fact                                                            |
|                                                                  |
|  - Id: string                                                    |
|  - ValueCallback: Func<Dictionary<string, object>, Almanac, Task<object>> |
|  - Options: FactOptions                                          |
|                                                                  |
+------------------------------------------------------------------+

+------------------------------------------------------------------+
|                                                                  |
|  OperatorMap                                                     |
|                                                                  |
|  - Operators: Dictionary<string, Func<object, object, bool>>     |
|  - OperatorDecorators: Dictionary<string, Func<...>>             |
|                                                                  |
|  + AddOperator(name: string, evaluateFunc: Func<...>): void      |
|  + RemoveOperator(name: string): bool                            |
|  + GetOperator(name: string): Func<object, object, bool>         |
|                                                                  |
+------------------------------------------------------------------+

+------------------------------------------------------------------+
|                                                                  |
|  Event                                                           |
|                                                                  |
|  - Type: string                                                  |
|  - Params: Dictionary<string, object>                            |
|                                                                  |
+------------------------------------------------------------------+
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
