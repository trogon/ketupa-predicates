namespace Trogon.KetupaPredicates
{
    using System;

    /// <summary>
    /// Extracts predicate data from text representation
    /// </summary>
    public class ExpressionParser
    {
        /// <summary>
        /// Gets first argument of predicate
        /// </summary>
        /// <param name="predicate">Text representation of predicate</param>
        /// <returns>Value of argument, or empty if end of string</returns>
        public string GetFirstArgument(string predicate)
        {
            return GetNextArgument(predicate, 0);
        }

        /// <summary>
        /// Gets next argument of predicate, starting from specified index
        /// </summary>
        /// <param name="predicate">Text representation of predicate</param>
        /// <param name="startIndex">Begining of the argument</param>
        /// <returns>Value of argument, or empty if end of string</returns>
        public string GetNextArgument(string predicate, int startIndex)
        {
            int index = startIndex;
            ParserState state = new ParserState();

            if (index > predicate.Length)
            {
                return string.Empty;
            }

            while (index < predicate.Length)
            {
                var token = GetToken(predicate[index]);
                state.Update(token);
                if (token == Token.ArgumentSeparator && state.BracketLevel == 0)
                {
                    break;
                }
                index++;
            }

            return predicate[startIndex..index];
        }

        /// <summary>
        /// Gets token of character
        /// </summary>
        /// <param name="symbol">Current character</param>
        /// <returns>Token of current character</returns>
        public Token GetToken(char symbol)
        {
            return symbol switch
            {
                ',' => Token.ArgumentSeparator,
                '{' => Token.PredicateStart,
                '}' => Token.PredicateEnd,
                '$' => Token.Variable,
                _ => Token.None,
            };
        }

        /// <summary>
        /// Checks if text is in brackets
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns>True if text is an expression, otherwise False</returns>
        public bool IsExpression(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return GetToken(text[0]) == Token.PredicateStart && GetToken(text[^1]) == Token.PredicateEnd;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if text is a variable name
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns>True if text is a variable, otherwise False</returns>
        public bool IsVariable(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return GetToken(text[0]) == Token.Variable;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Trims text and far left bracket and far right bracket
        /// </summary>
        /// <param name="text">Text for trimming</param>
        /// <returns>Trimmed text</returns>
        public string TrimExpression(string text)
        {
            var trimmedText = text.Trim();
            if (IsExpression(trimmedText))
            {
                return trimmedText[1..^1].Trim();
            }

            return text;
        }
    }
}
