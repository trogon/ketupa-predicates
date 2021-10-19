namespace Trogon.KetupaPredicates
{
    using System;

    public class PredicateExpression
    {
        private readonly ExpressionParser parser = new ExpressionParser();
        private readonly string expression;

        public PredicateExpression(string expression)
        {
            this.expression = expression;
        }

        public bool IsPrepared { get; private set; }
        public string? Operation { get; private set; }
        public IReadOnlyList<string>? Arguments { get; private set; }

        public void Prepare()
        {
            var arguments = new List<string>();

            // Act
            int startIndex = 0;
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
                    return Arguments[0] == Arguments[1];
                }
            }

            return false;
        }
    }
}
