using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Samples
{
    /// <summary>
    /// This sample demonstrates various real-world scenarios for using fact caching
    /// in the JsonRulesEngine to improve performance and handle common use cases.
    /// </summary>
    public class CachingSample
    {
        public static async Task RunSample()
        {
            Console.WriteLine("=== JsonRulesEngine Caching Sample ===");
            
            // Scenario 1: Database lookup caching
            await DatabaseLookupScenario();
            
            // Scenario 2: External API call caching
            await ExternalApiScenario();
            
            // Scenario 3: Computationally expensive calculations
            await ExpensiveCalculationScenario();
            
            // Scenario 4: Cache invalidation example
            await CacheInvalidationScenario();
        }
        
        /// <summary>
        /// Scenario 1: Caching database lookups
        /// 
        /// This scenario demonstrates how to cache database lookups
        /// to prevent multiple redundant database queries.
        /// </summary>
        public static async Task DatabaseLookupScenario()
        {
            Console.WriteLine("\n--- Database Lookup Caching Scenario ---");
            
            // Create engine with caching enabled
            var engine = new Engine(new EngineOptions { 
                EnableFactCaching = true 
            });
            
            // Create rule that needs user data multiple times
            var rule = new Rule(
                "user-eligibility",
                new TopLevelCondition("all", new[] {
                    new Condition("user", "equal", "PREMIUM", "$.accountType"),
                    new Condition("user", "greaterThan", 100, "$.purchaseCount"),
                    new Condition("user", "equal", true, "$.verifiedEmail")
                }),
                new Event("user-eligible-for-discount")
            );
            
            engine.AddRule(rule);
            
            // Simulate a database lookup that would be expensive to repeat
            int dbQueryCount = 0;
            
            Func<object> userLookup = () => {
                dbQueryCount++;
                Console.WriteLine($"  Database query executed (count: {dbQueryCount})");
                
                // Simulate DB query delay
                System.Threading.Thread.Sleep(100);
                
                return new {
                    accountType = "PREMIUM",
                    purchaseCount = 150,
                    verifiedEmail = true
                };
            };
            
            // Create almanac with cached fact
            var almanac = new Almanac();
            almanac.AddFact(new Fact<object>("user", userLookup, 
                new FactOptions { Cache = true }));
            
            // Run the rules - notice that the DB is only queried once even though the rule 
            // references the user fact three times
            var result = await engine.Run(almanac);
            
            Console.WriteLine($"  Rule evaluation result: {(result.Events.Count > 0 ? "User is eligible" : "User is not eligible")}");
            Console.WriteLine($"  Database queries executed: {dbQueryCount}");
        }
        
        /// <summary>
        /// Scenario 2: Caching external API calls
        /// 
        /// This scenario demonstrates caching data from external APIs
        /// with a time-based expiration to ensure data freshness.
        /// </summary>
        public static async Task ExternalApiScenario()
        {
            Console.WriteLine("\n--- External API Call Caching Scenario ---");
            
            // Create engine with caching enabled
            var engine = new Engine(new EngineOptions { 
                EnableFactCaching = true 
            });
            
            // Create rule that uses weather data
            var rule = new Rule(
                "weather-alert",
                new TopLevelCondition("any", new[] {
                    new Condition("weather", "greaterThan", 90, "$.temperature"),
                    new Condition("weather", "lessThan", 32, "$.temperature")
                }),
                new Event("extreme-weather-alert")
            );
            
            engine.AddRule(rule);
            
            // Simulate an external API call with a counter
            int apiCallCount = 0;
            
            Func<object> weatherApiCall = () => {
                apiCallCount++;
                Console.WriteLine($"  Weather API called (count: {apiCallCount})");
                
                // Simulate API call delay
                System.Threading.Thread.Sleep(200);
                
                // Return simulated weather data
                return new {
                    temperature = 95,
                    conditions = "Sunny",
                    humidity = 65
                };
            };
            
            // Create almanac with the weather fact that has a short expiration time
            var almanac = new Almanac();
            almanac.AddFact(new Fact<object>("weather", weatherApiCall, 
                new FactOptions { 
                    Cache = true,
                    CacheExpirationInSeconds = 30 // Cache for 30 seconds
                }));
            
            // First rule evaluation - should call the API
            Console.WriteLine("\n  First rule evaluation:");
            var result1 = await engine.Run(almanac);
            Console.WriteLine($"  Weather alert triggered: {result1.Events.Count > 0}");
            
            // Second rule evaluation - should use cached data
            Console.WriteLine("\n  Second rule evaluation (should use cache):");
            var result2 = await engine.Run(almanac);
            Console.WriteLine($"  Weather alert triggered: {result2.Events.Count > 0}");
            
            Console.WriteLine($"\n  Total API calls: {apiCallCount} (should be 1)");
        }
        
        /// <summary>
        /// Scenario 3: Caching computationally expensive calculations
        /// 
        /// This scenario demonstrates caching results of complex calculations
        /// that would be expensive to repeat.
        /// </summary>
        public static async Task ExpensiveCalculationScenario()
        {
            Console.WriteLine("\n--- Expensive Calculation Caching Scenario ---");
            
            // Create engine with caching enabled
            var engine = new Engine(new EngineOptions { 
                EnableFactCaching = true 
            });
            
            // Rule that uses the result of an expensive calculation multiple times
            var rule = new Rule(
                "risk-assessment",
                new TopLevelCondition("all", new[] {
                    new Condition("riskScore", "greaterThan", 70),
                    new Condition("customerStatus", "equal", "ACTIVE")
                }),
                new Event("high-risk-customer")
            );
            
            engine.AddRule(rule);
            
            // Simulate an expensive risk calculation
            int calculationCount = 0;
            
            Func<int> calculateRiskScore = () => {
                calculationCount++;
                Console.WriteLine($"  Performing complex risk calculation (count: {calculationCount})");
                
                // Simulate complex calculation
                System.Threading.Thread.Sleep(300);
                
                // Return calculated risk score
                return 75;
            };
            
            // Create almanac with cached risk calculation and simple customer status
            var almanac = new Almanac();
            almanac.AddFact(new Fact<int>("riskScore", calculateRiskScore, 
                new FactOptions { Cache = true }));
            almanac.AddFact(new Fact<string>("customerStatus", () => "ACTIVE"));
            
            // Run multiple rule evaluations in a loop
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"\n  Rule evaluation #{i+1}:");
                var result = await engine.Run(almanac);
                Console.WriteLine($"  High risk customer detected: {result.Events.Count > 0}");
            }
            
            Console.WriteLine($"\n  Total risk calculations performed: {calculationCount} (should be 1)");
        }
        
        /// <summary>
        /// Scenario 4: Cache invalidation for data updates
        /// 
        /// This scenario demonstrates how to invalidate cache entries
        /// when the underlying data changes.
        /// </summary>
        public static async Task CacheInvalidationScenario()
        {
            Console.WriteLine("\n--- Cache Invalidation Scenario ---");
            
            // Create engine with caching enabled
            var engine = new Engine(new EngineOptions { 
                EnableFactCaching = true 
            });
            
            // Create a rule that checks inventory status
            var rule = new Rule(
                "inventory-check",
                new TopLevelCondition("all", new[] {
                    new Condition("inventory", "greaterThan", 10, "$.quantity")
                }),
                new Event("inventory-sufficient")
            );
            
            engine.AddRule(rule);
            
            // Initial inventory level
            int inventory = 20;
            
            // Simulate inventory lookup
            int inventoryLookupCount = 0;
            
            Func<object> getInventory = () => {
                inventoryLookupCount++;
                Console.WriteLine($"  Inventory lookup performed (count: {inventoryLookupCount})");
                
                // Simulate lookup delay
                System.Threading.Thread.Sleep(50);
                
                return new { 
                    productId = "P12345",
                    quantity = inventory,
                    location = "Warehouse A"
                };
            };
            
            // Create almanac with cached inventory fact
            var almanac = new Almanac();
            almanac.AddFact(new Fact<object>("inventory", getInventory, 
                new FactOptions { Cache = true }));
            
            // First evaluation - should perform lookup
            Console.WriteLine("\n  First evaluation with initial inventory:");
            var result1 = await engine.Run(almanac);
            Console.WriteLine($"  Inventory sufficient: {result1.Events.Count > 0}");
            
            // Second evaluation - should use cache
            Console.WriteLine("\n  Second evaluation (should use cache):");
            var result2 = await engine.Run(almanac);
            Console.WriteLine($"  Inventory sufficient: {result2.Events.Count > 0}");
            
            // Simulate inventory change - decrease quantity below threshold
            inventory = 5;
            Console.WriteLine("\n  Inventory updated to 5 units");
            
            // Third evaluation - still using cached value (would give wrong result)
            Console.WriteLine("\n  Third evaluation with updated inventory (using stale cache):");
            var result3 = await engine.Run(almanac);
            Console.WriteLine($"  Inventory sufficient: {result3.Events.Count > 0} (incorrect, using stale data)");
            
            // Invalidate the cache for the inventory fact
            Console.WriteLine("\n  Invalidating cache for inventory fact");
            almanac.InvalidateCache("inventory");
            
            // Fourth evaluation - should perform a new lookup
            Console.WriteLine("\n  Fourth evaluation after cache invalidation:");
            var result4 = await engine.Run(almanac);
            Console.WriteLine($"  Inventory sufficient: {result4.Events.Count > 0} (correct)");
            
            Console.WriteLine($"\n  Total inventory lookups: {inventoryLookupCount} (should be 2)");
        }
    }
} 