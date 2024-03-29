﻿namespace Trogon.KetupaPredicates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Predicate variable representation
    /// </summary>
    public class PredicateVariable : IPredicateElement
    {
        private readonly string expression;

        /// <summary>
        /// Constructs <see cref="PredicateVariable"/>
        /// </summary>
        /// <param name="expression">Text representation of variable</param>
        public PredicateVariable(string expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Is prepared for evaluation
        /// </summary>
        public bool IsPrepared { get; private set; }
#if NET5_0_OR_GREATER
        /// <summary>
        /// Variable name
        /// </summary>
        public string? Name { get; private set; }
        /// <summary>
        /// Array variable index
        /// </summary>
        public IReadOnlyList<int>? Indices { get; private set; }
#else
        /// <summary>
        /// Variable name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Array variable index
        /// </summary>
        public IReadOnlyList<int> Indices { get; private set; }
#endif

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public object? GetValue(IDictionary<string, object> variables)
#else
        public object GetValue(IDictionary<string, object> variables)
#endif
        {
            if (!string.IsNullOrEmpty(Name) && variables.ContainsKey(Name))
            {
                object boxedVariable = variables[Name];
                if (Indices != null)
                {
                    foreach (var index in Indices)
                    {
                        if (index >= 0 && boxedVariable is IList<object> arrayValue && index < arrayValue.Count)
                        {
                            boxedVariable = arrayValue[index];
                        }
                        else if (index >= 0 && boxedVariable is IConvertible convertable && index < 64)
                        {
                            boxedVariable = (convertable.ToInt64(CultureInfo.InvariantCulture) >> index) & 1;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                return boxedVariable;
            }

            return null;
        }

        /// <summary>
        /// Prepares the predicate tree before it is ready to evaluate
        /// </summary>
        public void Prepare(ExpressionParser parser)
        {
            var arguments = new List<int>();

            Name = parser.GetVariableName(expression);

            var startIndex = Name.Length + 1;
            string nextIndex;
            while ((nextIndex = parser.GetNextIndex(expression, startIndex)) != string.Empty)
            {
                var trimmedArgument = nextIndex.Trim();
                if (int.TryParse(trimmedArgument, out int index))
                {
                    arguments.Add(index);
                }
                else
                {
                    throw new InvalidCastException($"[Variable {Name}]: Index {trimmedArgument} is not a number.");
                }
                startIndex += nextIndex.Length + 2;
            }

            if (arguments.Count > 0)
            {
                Indices = arguments;
            }

            IsPrepared = true;
        }
    }
}
