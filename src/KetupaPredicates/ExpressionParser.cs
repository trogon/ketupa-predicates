namespace Trogon.KetupaPredicates
{
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

            if (index > predicate.Length)
            {
                return string.Empty;
            }

            while (index < predicate.Length && predicate[index] != ',')
            {
                index++;
            }

            return predicate[startIndex..index];
        }
    }
}
