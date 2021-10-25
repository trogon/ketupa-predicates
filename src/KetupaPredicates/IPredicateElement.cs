namespace Trogon.KetupaPredicates
{
    using System.Collections.Generic;

    /// <summary>
    /// Predicate element
    /// </summary>
    public interface IPredicateElement
    {
        /// <summary>
        /// Gets the predicate element value using provided variables
        /// </summary>
        /// <param name="variables">Dictionary with variables</param>
        /// <returns>Value of an element</returns>
#if NET5_0_OR_GREATER
        object? GetValue(IDictionary<string, object> variables);
#else
        object GetValue(IDictionary<string, object> variables);
#endif
    }
}
