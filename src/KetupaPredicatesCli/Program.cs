// See https://aka.ms/new-console-template for more information
Console.Clear();
Console.Write("Enter the predicate expression: ");
var expression = Console.ReadLine();
if (expression != null)
{
    var predicate = new Trogon.KetupaPredicates.PredicateExpression(expression);
    predicate.Prepare();
    Console.WriteLine("Analysis:");
    Console.WriteLine($"Expression: {expression}");
    Console.WriteLine($"\tOperation: {predicate.Operation}");
    Console.WriteLine($"\tArguments (count {predicate.Arguments?.Count ?? 0}): ");
    int argNumber = 1;
    if (predicate.Arguments != null)
    {
        foreach (var arg in predicate.Arguments)
        {
            Console.WriteLine($"\t\tArgument {argNumber++}: {arg}");
        }
    }
    var result = predicate.Evaluate();
    Console.WriteLine($"\tEvaluation: {result}");
}
else
{
    Console.WriteLine("Expression not entered.");
}
