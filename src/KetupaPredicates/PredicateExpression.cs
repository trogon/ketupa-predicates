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
        private Dictionary<int, PredicateVariable> variableArguments = new Dictionary<int, PredicateVariable>();

        /// <summary>
        /// Constructs <see cref="PredicateExpression"/>
        /// </summary>
        /// <param name="expression">Text representation of predicate</param>
        public PredicateExpression(string expression)
        {
            this.expression = parser.TrimExpression(expression);
        }

        /// <summary>
        /// Is prepared for evaluation
        /// </summary>
        public bool IsPrepared { get; private set; }
#if NET5_0_OR_GREATER
        /// <summary>
        /// Operation name
        /// </summary>
        public string? Operation { get; private set; }
        /// <summary>
        /// Operation arguments in text format
        /// </summary>
        public IReadOnlyList<string>? Arguments { get; private set; }
#else
        /// <summary>
        /// Operation name
        /// </summary>
        public string Operation { get; private set; }
        /// <summary>
        /// Operation arguments in text format
        /// </summary>
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
                if (parser.IsVariable(trimmedArgument))
                {
                    var preparedArgument = new PredicateVariable(trimmedArgument);
                    preparedArgument.Prepare();
                    variableArguments.Add(arguments.Count, preparedArgument);
                }
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
            var variables = new Dictionary<string, object>();
            return Evaluate(variables);
        }

        /// <summary>
        /// Evaluates the predicate using provided variables
        /// </summary>
        /// <param name="variables">Dictionary with variables</param>
        /// <returns>True if predicate is logicaly true, otherwise False</returns>
        public bool Evaluate(IDictionary<string, object> variables)
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
                    "NOT" => !string.Equals(GetAgrumentValue(0, variables)?.ToString(), $"{true}", StringComparison.InvariantCultureIgnoreCase),
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case "NOT":
                        return !string.Equals(GetAgrumentValue(0, variables)?.ToString(), $"{true}", StringComparison.InvariantCultureIgnoreCase);
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
                    "=" => GetAgrumentValue(0, variables)?.ToString()?.Equals(GetAgrumentValue(1, variables)?.ToString()) == true,
                    "<" => EvaluateCompare(variables),
                    ">" => EvaluateCompare(variables),
                    "<=" => EvaluateCompare(variables),
                    ">=" => EvaluateCompare(variables),
                    "OR" => EvaluateOr(variables),
                    "AND" => EvaluateAnd(variables),
                    "HasFlag" => EvaluateHasFlag(variables),
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case "=":
                        return GetAgrumentValue(0, variables)?.ToString()?.Equals(GetAgrumentValue(1, variables)?.ToString()) == true;
                    case "<":
                        return EvaluateCompare(variables);
                    case ">":
                        return EvaluateCompare(variables);
                    case "<=":
                        return EvaluateCompare(variables);
                    case ">=":
                        return EvaluateCompare(variables);
                    case "OR":
                        return EvaluateOr(variables);
                    case "AND":
                        return EvaluateAnd(variables);
                    case "HasFlag":
                        return EvaluateHasFlag(variables);
                    default:
                        return false;
                }
#endif
            }

            if (Arguments?.Count > 2)
            {
#if NET5_0_OR_GREATER
                return Operation switch
                {
                    "OR" => EvaluateOr(variables),
                    "AND" => EvaluateAnd(variables),
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case "OR":
                        return EvaluateOr(variables);
                    case "AND":
                        return EvaluateAnd(variables);
                    default:
                        return false;
                }
#endif
            }

            return false;
        }

        /// <summary>
        /// Checks if all arguments are True
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if all arguments are true, otherwise False</returns>
        public bool EvaluateAnd(IDictionary<string, object> variables)
        {
            if (Arguments == null || Arguments.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if ($"{false}".Equals(GetAgrumentValue(i, variables)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if first argument has flag indicated by second argument
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if has flag, otherwise False</returns>
        public bool EvaluateCompare(IDictionary<string, object> variables)
        {
            var valA = GetAgrumentValue(0, variables);
            var valB = GetAgrumentValue(1, variables);
            if (long.TryParse(valA?.ToString(), out long numA) && long.TryParse(valB?.ToString(), out long numB))
            {
#if NET5_0_OR_GREATER
                return Operation switch
                {
                    "<" => numA < numB,
                    ">" => numA > numB,
                    "<=" => numA <= numB,
                    ">=" => numA >= numB,
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case "<":
                        return numA < numB;
                    case ">":
                        return numA > numB;
                    case "<=":
                        return numA <= numB;
                    case ">=":
                        return numA >= numB;
                    default:
                        return false;
                }
#endif
            }

            return false;
        }

        /// <summary>
        /// Checks if first argument has flag indicated by second argument
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if has flag, otherwise False</returns>
        public bool EvaluateHasFlag(IDictionary<string, object> variables)
        {
            var valA = GetAgrumentValue(0, variables);
            var valB = GetAgrumentValue(1, variables);
            if (int.TryParse(valA?.ToString(), out int enumA) && int.TryParse(valB?.ToString(), out int enumB))
            {
                return (enumA & enumB) == enumB;
            }

            return false;
        }

        /// <summary>
        /// Checks if any argument is True
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if any argument is true, otherwise False</returns>
        public bool EvaluateOr(IDictionary<string, object> variables)
        {
            if (Arguments == null || Arguments.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if ($"{true}".Equals(GetAgrumentValue(i, variables)))
                {
                    return true;
                }
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
        public object? GetAgrumentValue(int index, IDictionary<string, object> variables)
#else
        public object GetAgrumentValue(int index, IDictionary<string, object> variables)
#endif
        {
            var argument = Arguments?[index];
            if (argument != null)
            {
                if (parser.IsVariable(argument) && variableArguments.ContainsKey(index))
                {
                    return variableArguments[index].GetValue(variables);
                }
                else if (parser.IsExpression(argument) && predicateArguments.ContainsKey(index))
                {
                    var value = predicateArguments[index].Evaluate(variables).ToString();
                    return value;
                }
            }

            return argument;
        }
    }
}
