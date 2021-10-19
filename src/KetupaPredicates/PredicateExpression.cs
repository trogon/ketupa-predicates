namespace Trogon.KetupaPredicates
{
    using System;

    /// <summary>
    /// Predicate expression representation
    /// </summary>
    public class PredicateExpression
    {
        private readonly ExpressionParser parser = new ExpressionParser();
        private readonly string expression;

        public PredicateExpression(string expression)
        {
            this.expression = parser.TrimExpression(expression);
        }

        public bool IsPrepared { get; private set; }
        public string? Operation { get; private set; }
        public IReadOnlyList<string>? Arguments { get; private set; }

        /// <summary>
        /// Parses the expression before it is ready to evaluate
        /// </summary>
        public void Prepare()
        {
            var arguments = new List<string>();
            var startIndex = 0;

            Operation = parser.GetFirstArgument(expression);
            startIndex += Operation.Length + 1;

            string nextArgument;
            while ((nextArgument = parser.GetNextArgument(expression, startIndex)) != string.Empty)
            {
                arguments.Add(nextArgument.Trim());
                startIndex += nextArgument.Length + 1;
            }

            Arguments = arguments;

            IsPrepared = true;
        }

        /// <summary>
        /// Evaluates the predicate
        /// </summary>
        /// <returns>True if predicate is logicaly true, otherwise False</returns>
        public bool Evaluate()
        {
            if (!IsPrepared)
            {
                Prepare();
            }

            if (Operation == "=")
            {
                if (Arguments?.Count == 2)
                {
                    return Arguments?[0] == Arguments?[1];
                }
            }

            return false;
        }

        public bool Evaluate(IDictionary<string, string> variables)
        {
            if (!IsPrepared)
            {
                Prepare();
            }

            if (Operation == "=")
            {
                if (Arguments?.Count == 2)
                {
                    return GetAgrumentValue(0, variables) == GetAgrumentValue(1, variables);
                }
            }

            return false;
        }

        public string? GetAgrumentValue(int index, IDictionary<string, string> variables)
        {
            var arg = Arguments?[index];
            if (arg != null && parser.IsVariable(arg) && variables.ContainsKey(arg))
            {
                return variables[arg];
            }
            else
            {
                return arg;
            }
        }
    }
}
