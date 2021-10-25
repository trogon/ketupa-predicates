// See https://aka.ms/new-console-template for more information
var exampleVariables = new Dictionary<string, object>
{
    { "$test", "42" }
};

Console.Clear();
Console.Write("Enter the predicate expression: ");
var expression = Console.ReadLine();

if (expression != null)
{
    var predicate = new Trogon.KetupaPredicates.PredicateExpression(expression);
    predicate.Prepare();

    new Trogon.KetupaPredicates.Cli.ExpressionPrinter().Print(expression, predicate);
    new Trogon.KetupaPredicates.Cli.ExpressionPrinter().PrintVariables(exampleVariables);

    var result = predicate.Evaluate(exampleVariables);
    Console.WriteLine($"\tEvaluation: {result}");
}
else
{
    Console.WriteLine("Expression not entered.");
}
