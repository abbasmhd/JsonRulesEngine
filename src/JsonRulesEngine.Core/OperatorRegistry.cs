using System;
using System.Collections.Generic;
using System.Linq;
using JsonRulesEngine.Core.Interfaces;
using JsonRulesEngine.Core.Operators;

namespace JsonRulesEngine.Core
{
    /// <summary>
    /// Registry for operators used in rule conditions
    /// </summary>
    public class OperatorRegistry
    {
        private readonly Dictionary<string, Operator> _operators;
        
        /// <summary>
        /// Initializes a new instance of the OperatorRegistry class with default operators
        /// </summary>
        public OperatorRegistry()
        {
            _operators = new Dictionary<string, Operator>
            {
                { "equal", new EqualOperator() },
                { "notEqual", new NotEqualOperator() },
                { "greaterThan", new GreaterThanOperator() },
                { "greaterThanInclusive", new GreaterThanOrEqualOperator() },
                { "lessThan", new LessThanOperator() },
                { "lessThanInclusive", new LessThanOrEqualOperator() },
                { "in", new InOperator() },
                { "notIn", new NotInOperator() },
                { "contains", new ContainsOperator() },
                { "doesNotContain", new DoesNotContainOperator() }
            };
        }
        
        /// <summary>
        /// Gets an operator by name
        /// </summary>
        /// <param name="name">The name of the operator</param>
        /// <returns>The operator</returns>
        public Operator GetOperator(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
                
            if (_operators.TryGetValue(name, out var op))
                return op;
                
            throw new KeyNotFoundException($"Operator '{name}' not found in registry");
        }
        
        /// <summary>
        /// Adds an operator to the registry
        /// </summary>
        /// <param name="op">The operator to add</param>
        public void AddOperator(Operator op)
        {
            if (op == null)
                throw new ArgumentNullException(nameof(op));
                
            _operators[op.Name] = op;
        }
        
        /// <summary>
        /// Gets all registered operators
        /// </summary>
        /// <returns>All registered operators</returns>
        public IEnumerable<Operator> GetAllOperators()
        {
            return _operators.Values;
        }
    }
}
