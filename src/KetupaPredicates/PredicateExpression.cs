﻿namespace Trogon.KetupaPredicates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Predicate expression representation
    /// </summary>
    public class PredicateExpression : IPredicateElement
    {
        private readonly string expression;
        private Dictionary<int, IPredicateElement> predicateElements = new Dictionary<int, IPredicateElement>();

        /// <summary>
        /// Constructs <see cref="PredicateExpression"/>
        /// </summary>
        /// <param name="expression">Text representation of predicate</param>
        public PredicateExpression(string expression)
        {
            this.expression = expression;
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
        /// Operation arguments in text format
        /// </summary>
        public IReadOnlyDictionary<int, IPredicateElement> PredicateElements => predicateElements;

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public object? GetValue(IDictionary<string, object> variables) => Evaluate(variables).ToString();
#else
        public object GetValue(IDictionary<string, object> variables) => Evaluate(variables).ToString();
#endif

        /// <summary>
        /// Prepares the predicate tree before it is ready to evaluate
        /// </summary>
        public void Prepare()
        {
            var parser = new ExpressionParser();
            PrepareArguments(parser);
            PrepareTree(this, parser);

            IsPrepared = true;
        }

        /// <summary>
        /// Prepares the predicate before it is ready to evaluate
        /// </summary>
        /// <remarks>Method sutable for simple predicates</remarks>
        public void PrepareArguments(ExpressionParser parser)
        {
            var timmedExpression = parser.TrimExpression(expression);
            var arguments = new List<string>();

            string nextArgument = parser.GetFirstArgument(timmedExpression);
            int startIndex = nextArgument.Length;
            Operation = nextArgument.Trim();

            while (parser.HasMoreArguments(timmedExpression, startIndex))
            {
                nextArgument = parser.GetNextArgument(timmedExpression, startIndex + 1);
                var trimmedArgument = nextArgument.Trim();
                if (parser.IsVariable(trimmedArgument))
                {
                    var preparedArgument = new PredicateVariable(trimmedArgument);
                    preparedArgument.Prepare(parser);
                    predicateElements.Add(arguments.Count, preparedArgument);
                }
                else
                {
                    var stringBuilder = new StringBuilder();
                    var escapeTokens = 0;
                    for (int i = 0; i < trimmedArgument.Length; i++)
                    {
                        var token = parser.GetToken(trimmedArgument[i]);
                        escapeTokens += (token == Token.TokenEscape) ? 1 : -escapeTokens;
                        if (escapeTokens % 2 == 0)
                        {
                            stringBuilder.Append(trimmedArgument[i]);
                        }
                    }
                    trimmedArgument = stringBuilder.ToString();
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
                            preparedArgument.PrepareArguments(parser);
                            predicate.predicateElements.Add(argumentIndex, preparedArgument);
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
                    var op when "NOT".Equals(op, StringComparison.OrdinalIgnoreCase) => !string.Equals(GetAgrumentValue(0, variables)?.ToString(), $"{true}", StringComparison.InvariantCultureIgnoreCase),
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case var op when "NOT".Equals(op, StringComparison.OrdinalIgnoreCase):
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
                    var op when "OR".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateOr(variables),
                    var op when "AND".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateAnd(variables),
                    var op when "IN".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateIn(variables),
                    var op when "HasFlag".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateHasFlag(variables),
                    var op when "Matches".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateRegexMatches(variables),
                    var op when "StartsWith".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateStartsWith(variables),
                    var op when "EndsWith".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateEndsWith(variables),
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
                    case var op when "OR".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateOr(variables);
                    case var op when "AND".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateAnd(variables);
                    case var op when "IN".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateIn(variables);
                    case var op when "HasFlag".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateHasFlag(variables);
                    case var op when "Matches".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateRegexMatches(variables);
                    case var op when "StartsWith".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateStartsWith(variables);
                    case var op when "EndsWith".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateEndsWith(variables);
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
                    var op when "OR".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateOr(variables),
                    var op when "AND".Equals(op, StringComparison.OrdinalIgnoreCase) => EvaluateAnd(variables),
                    _ => false,
                };
#else
                switch (Operation)
                {
                    case var op when "OR".Equals(op, StringComparison.OrdinalIgnoreCase):
                        return EvaluateOr(variables);
                    case var op when "AND".Equals(op, StringComparison.OrdinalIgnoreCase):
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
        /// Checks if comparison of two number arguments is true
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True arguments are numbers and comparison is true, otherwise False</returns>
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
        /// Checks if first argument ends with second argument text
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if input ends with text, otherwise False</returns>
        public bool EvaluateEndsWith(IDictionary<string, object> variables)
        {
            var valA = GetAgrumentValue(0, variables)?.ToString() ?? string.Empty;
            var valB = GetAgrumentValue(1, variables)?.ToString() ?? string.Empty;

            return valA.EndsWith(valB);
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
        /// Checks if right argument text contains left argument text
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if all arguments are true, otherwise False</returns>
        public bool EvaluateIn(IDictionary<string, object> variables)
        {
            var valA = GetAgrumentValue(0, variables)?.ToString() ?? string.Empty;
            var argB = GetAgrumentValue(1, variables);

            if (argB is IEnumerable<string> textArray)
            {
                return textArray.Contains(valA);
            }
            else
            {
                var valB = argB?.ToString() ?? string.Empty;
                return valB.Contains(valA);
            }
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
        /// Checks if first argument matches second argument pattern
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if input matches pattern, otherwise False</returns>
        public bool EvaluateRegexMatches(IDictionary<string, object> variables)
        {
            var valA = GetAgrumentValue(0, variables)?.ToString() ?? string.Empty;
            var valB = GetAgrumentValue(1, variables)?.ToString() ?? string.Empty;

            return Regex.IsMatch(valA, valB);
        }

        /// <summary>
        /// Checks if first argument starts with second argument text
        /// </summary>
        /// <param name="variables">Provided variables</param>
        /// <returns>True if input starts with text, otherwise False</returns>
        public bool EvaluateStartsWith(IDictionary<string, object> variables)
        {
            var valA = GetAgrumentValue(0, variables)?.ToString() ?? string.Empty;
            var valB = GetAgrumentValue(1, variables)?.ToString() ?? string.Empty;

            return valA.StartsWith(valB);
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
            if (predicateElements.ContainsKey(index))
            {
                return predicateElements[index].GetValue(variables);
            }
            else
            {
                return Arguments?[index];
            }
        }
    }
}
