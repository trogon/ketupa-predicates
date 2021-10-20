namespace Trogon.KetupaPredicates
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Predicate variable representation
    /// </summary>
    public class PredicateVariable
    {
        private readonly ExpressionParser parser = new ExpressionParser();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
#if NET5_0_OR_GREATER
        public object? GetValue(IDictionary<string, object> variables)
#else
        public object GetValue(IDictionary<string, object> variables)
#endif
        {
            if (!string.IsNullOrEmpty(Name) && variables.ContainsKey(Name))
            {
                if (Indices != null && variables[Name] is IList<object> arrayValue)
                {
                    if (Indices.Count > 0 && Indices[0] >= 0 && Indices[0] < arrayValue.Count)
                    {
                        return arrayValue[Indices[0]];
                    }
                }
                else
                {
                    return variables[Name];
                }
            }

            return null;
        }

        /// <summary>
        /// Prepares the predicate tree before it is ready to evaluate
        /// </summary>
        public void Prepare()
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
