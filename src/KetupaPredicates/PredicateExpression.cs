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
        private Dictionary<int, PredicateExpression> preparedArguments = new Dictionary<int, PredicateExpression>();

        public PredicateExpression(string expression)
        {
            this.expression = parser.TrimExpression(expression);
        }

        public bool IsPrepared { get; private set; }
        public string? Operation { get; private set; }
        public IReadOnlyList<string>? Arguments { get; private set; }

        /// <summary>
        /// Prepares the predicate tree before it is ready to evaluate
        /// </summary>
        public void Prepare()
        {
            PrepareArguments();
            PrepareTree(this, parser);

            IsPrepared = true;
        }

        /// <summary>
        /// Prepares the predicate before it is ready to evaluate
        /// </summary>
        /// <remarks>Method sutable for simple predicates</remarks>
        public void PrepareArguments()
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
        /// Prepares the predicate arguments
        /// </summary>
        /// <param name="rootPredicate">Tree root predicate</param>
        /// <param name="parser"><see cref="ExpressionParser"/> instance</param>
        public static void PrepareTree(PredicateExpression rootPredicate, ExpressionParser parser)
        {
            var prepareQueue = new Queue<PredicateExpression>();
            prepareQueue.Enqueue(rootPredicate);

            while (prepareQueue.Count > 0)
            {
                var predicate = prepareQueue.Dequeue();

                if (predicate.Arguments != null)
                {
                    for (int argumentIndex = 0; argumentIndex < predicate.Arguments.Count; argumentIndex++)
                    {
                        var argument = predicate.Arguments[argumentIndex];
                        if (parser.IsExpression(argument))
                        {
                            var preparedArgument = new PredicateExpression(argument);
                            preparedArgument.PrepareArguments();
                            predicate.preparedArguments.Add(argumentIndex, preparedArgument);
                            prepareQueue.Enqueue(preparedArgument);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates the predicate
        /// </summary>
        /// <returns>True if predicate is logicaly true, otherwise False</returns>
        public bool Evaluate()
        {
            var variables = new Dictionary<string, string>();
            return Evaluate(variables);
        }

        /// <summary>
        /// Evaluates the predicate using provided variables
        /// </summary>
        /// <param name="variables">Dictionary with variables</param>
        /// <returns>True if predicate is logicaly true, otherwise False</returns>
        public bool Evaluate(IDictionary<string, string> variables)
        {
            if (!IsPrepared)
            {
                Prepare();
            }

            if (Arguments?.Count == 1)
            {
                return Operation switch
                {
                    "NOT" => !string.Equals(GetAgrumentValue(0, variables), $"{true}", StringComparison.InvariantCultureIgnoreCase),
                    _ => false,
                };
            }

            if (Arguments?.Count == 2)
            {
                return Operation switch
                {
                    "=" => GetAgrumentValue(0, variables) == GetAgrumentValue(1, variables),
                    "OR" => GetAgrumentValue(0, variables) == $"{true}" || GetAgrumentValue(1, variables) == $"{true}",
                    "AND" => GetAgrumentValue(0, variables) == $"{true}" && GetAgrumentValue(1, variables) == $"{true}",
                    _ => false,
                };
            }

            return false;
        }

        /// <summary>
        /// Gets value of an argument from text, variable or evaluate predicate
        /// </summary>
        /// <param name="index">Argument index</param>
        /// <param name="variables">Provided variables</param>
        /// <returns>Variable value as <see cref="string"/></returns>
        public string? GetAgrumentValue(int index, IDictionary<string, string> variables)
        {
            var argument = Arguments?[index];
            if (argument != null)
            {
                if (parser.IsVariable(argument) && variables.ContainsKey(argument))
                {
                    return variables[argument];
                }
                else if (parser.IsExpression(argument) && preparedArguments.ContainsKey(index))
                {
                    var value = preparedArguments[index].Evaluate(variables).ToString();
                    return value;
                }
            }

            return argument;
        }
    }
}
