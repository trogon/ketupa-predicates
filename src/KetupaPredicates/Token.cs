namespace Trogon.KetupaPredicates
{
    /// <summary>
    /// Syntax token
    /// </summary>
    public enum Token
    {
        /// <summary>
        /// Character without special meaning
        /// </summary>
        None = 0,

        /// <summary>
        /// Argument separator character
        /// </summary>
        ArgumentSeparator = 1,

        /// <summary>
        /// Predicate start bracket
        /// </summary>
        PredicateStart = 2,

        /// <summary>
        /// Predicate end bracket
        /// </summary>
        PredicateEnd = 3,

        /// <summary>
        /// Variable start character
        /// </summary>
        Variable = 4,

        /// <summary>
        /// Index start bracket
        /// </summary>
        IndexStart = 5,

        /// <summary>
        /// Index end bracket
        /// </summary>
        IndexEnd = 6,

        /// <summary>
        /// Token escape symbol
        /// </summary>
        TokenEscape = 7
    }
}
