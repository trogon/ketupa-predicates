namespace Trogon.KetupaPredicates
{
    /// <summary>
    /// State helper for parser
    /// </summary>
    public class ParserState
    {
        /// <summary>
        /// Current bracket level
        /// </summary>
        public int BracketLevel { get; private set; }

        /// <summary>
        /// Updates state
        /// </summary>
        /// <param name="token">Token of current character</param>
        /// <returns>Token of current character</returns>
        public void Update(Token token)
        {
            BracketLevel = GetBracketLevel(token, BracketLevel);
        }

        /// <summary>
        /// Increments or decrements bracket level
        /// </summary>
        /// <param name="token">Token of current character</param>
        /// <returns>Current bracket level</returns>
        public int GetBracketLevel(Token token, int level)
        {
#if NET5_0_OR_GREATER
            return token switch
            {
                Token.PredicateStart => level + 1,
                Token.PredicateEnd => level - 1,
                _ => level
            };
#else
            switch (token)
            {
                case Token.PredicateStart:
                    return level + 1;
                case Token.PredicateEnd:
                    return level - 1;
                default:
                    return level;
            }
#endif

        }
    }
}
