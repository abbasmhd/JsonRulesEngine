using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using JsonRulesEngine.Core;
using JsonRulesEngine.Core.Interfaces;
using JsonRulesEngine.Core.Models;

namespace JsonRulesEngine.Benchmarks
{
    public class FactCachingBenchmarks
    {
        private Engine _engineWithoutCache = null!;
        private Engine _engineWithCache = null!;
        private Rule _rule = null!;
        private Dictionary<string, object> _facts = null!;
        private int _callCount;

        [GlobalSetup]
        public void Setup()
        {
            // Reset call counter
            _callCount = 0;

            // Create engines with and without caching
            _engineWithoutCache = new Engine(null, new EngineOptions
            {
                EnableFactCaching = false,
                PathResolver = new JsonPathResolver()
            });

            _engineWithCache = new Engine(null, new EngineOptions
            {
                EnableFactCaching = true,
                PathResolver = new JsonPathResolver(),
                AllowUndefinedFacts = false,
                AllowUndefinedConditions = false
            });

            // Create a rule that uses the same fact multiple times
            _rule = new Rule(
                "repeat-fact-rule",
                new TopLevelCondition("all", new[] {
                    new Condition("expensive-fact", "equal", true),
                    new Condition("another-condition", "equal", true),
                    new Condition("expensive-fact", "equal", true) // Intentionally repeated
                }),
                new Event("rule-matched")
            );

            // Add the rule to both engines
            _engineWithoutCache.AddRule(_rule);
            _engineWithCache.AddRule(_rule);

            // Create facts including an "expensive" fact that counts how many times it's called
            _facts = new Dictionary<string, object>();
        }

        private bool ExpensiveOperation()
        {
            // Increment call counter to track how many times this is executed
            _callCount++;
            
            // Simulate an expensive operation with a small delay
            System.Threading.Thread.Sleep(10);
            return true;
        }

        [Benchmark(Baseline = true)]
        public async Task RunWithoutCaching()
        {
            // Reset counter
            _callCount = 0;

            // Add a dynamic fact that will be resolved during rule evaluation
            var almanac = new Almanac(null, new AlmanacOptions 
            { 
                EnableFactCaching = false,
                PathResolver = new JsonPathResolver()
            });
            
            almanac.AddFact(new Fact("expensive-fact", (_, __) => Task.FromResult<object>(ExpensiveOperation())));
            almanac.AddFact(new Fact("another-condition", (_, __) => Task.FromResult<object>(true)));

            // Run the engine
            var runOptions = new RunOptions { Almanac = almanac };
            await _engineWithoutCache.Run(null, runOptions);
        }

        [Benchmark]
        public async Task RunWithCaching()
        {
            // Reset counter
            _callCount = 0;

            // Add a dynamic fact with caching enabled
            var almanac = new Almanac(null, new AlmanacOptions 
            { 
                EnableFactCaching = true,
                PathResolver = new JsonPathResolver()
            });
            
            almanac.AddFact(new Fact("expensive-fact", (_, __) => Task.FromResult<object>(ExpensiveOperation()), 
                new FactOptions { Cache = true }));
            almanac.AddFact(new Fact("another-condition", (_, __) => Task.FromResult<object>(true)));

            // Run the engine
            var runOptions = new RunOptions { Almanac = almanac };
            await _engineWithCache.Run(null, runOptions);
        }

        [Benchmark]
        public async Task RunWithCachingMultipleRules()
        {
            // Reset counter
            _callCount = 0;

            // Add a dynamic fact with caching enabled
            var almanac = new Almanac(null, new AlmanacOptions 
            { 
                EnableFactCaching = true,
                PathResolver = new JsonPathResolver()
            });
            
            almanac.AddFact(new Fact("expensive-fact", (_, __) => Task.FromResult<object>(ExpensiveOperation()), 
                new FactOptions { Cache = true }));
            almanac.AddFact(new Fact("another-condition", (_, __) => Task.FromResult<object>(true)));

            // Run the engine multiple times to demonstrate cache reuse
            var runOptions = new RunOptions { Almanac = almanac };
            await _engineWithCache.Run(null, runOptions);
            await _engineWithCache.Run(null, runOptions);
            await _engineWithCache.Run(null, runOptions);
        }

        [Benchmark]
        public async Task RunWithCacheExpiration()
        {
            // Reset counter
            _callCount = 0;

            // Add a dynamic fact with caching enabled and a short expiration
            var almanac = new Almanac(null, new AlmanacOptions 
            { 
                EnableFactCaching = true,
                PathResolver = new JsonPathResolver()
            });
            
            almanac.AddFact(new Fact("expensive-fact", (_, __) => Task.FromResult<object>(ExpensiveOperation()), 
                new FactOptions { 
                    Cache = true,
                    CacheExpirationInSeconds = 1 // Short expiration
                }));
            almanac.AddFact(new Fact("another-condition", (_, __) => Task.FromResult<object>(true)));

            // Run the engine
            var runOptions = new RunOptions { Almanac = almanac };
            await _engineWithCache.Run(null, runOptions);
            
            // Wait for cache to expire
            System.Threading.Thread.Sleep(1100);
            
            // Run again after expiration
            await _engineWithCache.Run(null, runOptions);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FactCachingBenchmarks>();
        }
    }
}
