namespace Trogon.KetupaPredicates
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Predicate expression representation
    /// </summary>
    public class PredicateExpression
    {
        private readonly ExpressionParser parser = new ExpressionParser();
        private readonly string expression;
        private Dictionary<int, PredicateExpression> predicateArguments = new Dictionary<int, PredicateExpression>();

        public PredicateExpression(string expression)
        {
            this.expression = parser.TrimExpression(expression);
        }

        public bool IsPrepared { get; private set; }
#if NET5_0_OR_GREATER
        public string? Operation { get; private set; }
        public IReadOnlyList<string>? Arguments { get; private set; }
#else
        public string Operation { get; private set; }
        public IReadOnlyList<string> Arguments { get; private set; }
#endif

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
            Operation = parser.GetFirstArgument(expression);

            int startIndex = Operation.Length + 1;
            string nextArgument;

            while ((nextArgument = parser.GetNextArgument(expression, startIndex)) != string.Empty)
            {
                var trimmedArgument = nextArgument.Trim();
                arguments.Add(trimmedArgument);
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
                            predicate.predicateArguments.Add(argumentIndex, preparedArgument);
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
#if NET5_0_OR_GREATER
                return Operation switch
                {
                    "NOT" => !string.Equals(GetAgrumentValue(0, variables), $"{true}", StringComparison.InvariantCultureIgnoreCase),
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case "NOT":
                        return !string.Equals(GetAgrumentValue(0, variables), $"{true}", StringComparison.InvariantCultureIgnoreCase);
                    default:
                        return false;
                }
#endif
            }

            if (Arguments?.Count == 2)
            {
#if NET5_0_OR_GREATER
                return Operation switch
                {
                    "=" => GetAgrumentValue(0, variables) == GetAgrumentValue(1, variables),
                    "OR" => GetAgrumentValue(0, variables) == $"{true}" || GetAgrumentValue(1, variables) == $"{true}",
                    "AND" => GetAgrumentValue(0, variables) == $"{true}" && GetAgrumentValue(1, variables) == $"{true}",
                    "HasFlag" => HasFlag(variables),
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case "=":
                        return GetAgrumentValue(0, variables) == GetAgrumentValue(1, variables);
                    case "OR":
                        return GetAgrumentValue(0, variables) == $"{true}" || GetAgrumentValue(1, variables) == $"{true}";
                    case "AND":
                        return GetAgrumentValue(0, variables) == $"{true}" && GetAgrumentValue(1, variables) == $"{true}";
                    case "HasFlag":
                        return HasFlag(variables);
                    default:
                        return false;
                }
#endif
            }

            return false;
        }

        /// <summary>
        /// Gets value of an argument from text, variable or evaluate predicate
        /// </summary>
        /// <param name="index">Argument index</param>
        /// <param name="variables">Provided variables</param>
        /// <returns>Variable value as <see cref="string"/></returns>
#if NET5_0_OR_GREATER
        public string? GetAgrumentValue(int index, IDictionary<string, string> variables)
#else
        public string GetAgrumentValue(int index, IDictionary<string, string> variables)
#endif
        {
            var argument = Arguments?[index];
            if (argument != null)
            {
                if (parser.IsVariable(argument) && variables.ContainsKey(argument))
                {
                    return variables[argument];
                }
                else if (parser.IsExpression(argument) && predicateArguments.ContainsKey(index))
                {
                    var value = predicateArguments[index].Evaluate(variables).ToString();
                    return value;
                }
            }

            return argument;
        }

        /// <summary>
        /// Checks if first argument has flag indicated by second argument
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if has flag, otherwise False</returns>
        public bool HasFlag(IDictionary<string, string> variables)
        {
            var valA = GetAgrumentValue(0, variables);
            var valB = GetAgrumentValue(1, variables);
            if (int.TryParse(valA, out int enumA) && int.TryParse(valB, out int enumB))
            {
                return (enumA & enumB) == enumB;
            }

            return false;
        }
    }
}
