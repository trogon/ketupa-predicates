namespace Trogon.KetupaPredicates.Cli
{
    using System;
    using System.Collections.Generic;

    public class ExpressionPrinter
    {
        public void Print(string expression, PredicateExpression predicate)
        {
            Console.WriteLine("Analysis:");
            Console.WriteLine($"Expression: [{expression}]");
            Console.WriteLine($"\tOperation: [{predicate.Operation}]");
            Console.WriteLine($"\tArguments (count {predicate.Arguments?.Count ?? 0}): ");

            int argIndex = 0;
            if (predicate.Arguments != null)
            {
                foreach (var arg in predicate.Arguments)
                {
                    if (predicate.PredicateElements.ContainsKey(argIndex))
                    {
                        var element = predicate.PredicateElements[argIndex];
                        if (element is PredicateExpression innerExpression)
                        {
                            Console.WriteLine($"\t\tArgument {argIndex + 1} is an expression.");
                            Print(arg, innerExpression);
                            Console.WriteLine();
                        }
                        else if (element is PredicateVariable variable)
                        {
                            Console.WriteLine($"\t\tArgument {argIndex + 1} is an variable (name={variable.Name}, indices={string.Join(",", variable.Indices ?? new List<int>())}).");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\t\tArgument {argIndex + 1}: [{arg}]");
                    }
                    argIndex++;
                }
            }
        }

        public void PrintVariables(IDictionary<string, object> variables)
        {
            if (variables != null)
            {
                Console.WriteLine($"\tVariables:");
                foreach (var variable in variables)
                {
                    Console.WriteLine($"\t\t{variable.Key}={variable.Value}");
                }
            }
            else
            {
                Console.WriteLine("Variables not entered.");
            }
        }
    }
}
