namespace Trogon.KetupaPredicates.Tester.Data
{
    /// <summary>
    /// Data model of the variable for UI.
    /// </summary>
    public class PredicateVariable
    {
        /// <summary>
        /// Initialize the values of the variable.
        /// </summary>
        /// <param name="name">Name for the variable.</param>
        /// <param name="value">Value for the variable.</param>
        public PredicateVariable(string name, string value)
        {
            Name = name;
            TextValue = value;
        }

        /// <summary>
        /// Gets the variable name.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets the variable value as text.
        /// </summary>
        public string TextValue { get; protected set; }
    }
}
