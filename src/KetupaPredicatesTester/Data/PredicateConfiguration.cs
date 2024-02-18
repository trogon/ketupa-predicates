namespace Trogon.KetupaPredicates.Tester.Data
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Data model for the Main view.
    /// </summary>
    public class PredicateConfiguration : INotifyPropertyChanged
    {
        /// <summary>
        /// List of variables for predicate evaluation.
        /// </summary>
        public ObservableCollection<PredicateVariable> Variables { get; protected set; } = new ObservableCollection<PredicateVariable>();

        /// <summary>
        /// Stores the predicate expression text.
        /// </summary>
        protected string predicateText = string.Empty;
        /// <summary>
        /// Gets and sets the predicate expression text.
        /// </summary>
        public string PredicateText
        {
            get => predicateText;
            set => SetPropertyValue(ref predicateText, value);
        }

        /// <summary>
        /// Stores the variable name that can be added next.
        /// </summary>
        protected string variableName = string.Empty;
        /// <summary>
        /// Gets and sets the variable name that can be added next.
        /// </summary>
        public string VariableName
        {
            get => variableName;
            set => SetPropertyValue(ref variableName, value);
        }

        /// <summary>
        /// Stores the variable text value that can be added next.
        /// </summary>
        protected string variableTextValue = string.Empty;
        /// <summary>
        /// Gets and sets the variable text value that can be added next.
        /// </summary>
        public string VariableTextValue
        {
            get => variableTextValue;
            set => SetPropertyValue(ref variableTextValue, value);
        }

        /// <summary>
        /// Stores the selected variable in UI.
        /// </summary>
        protected PredicateVariable? selectedVariable;
        /// <summary>
        /// Gets and sets the selected variable in UI.
        /// </summary>
        public PredicateVariable? SelectedVariable
        {
            get => selectedVariable;
            set => SetPropertyValue(ref selectedVariable, value);
        }

        /// <summary>
        /// Stores the last predicate evaluation.
        /// </summary>
        protected string evaluationResult = string.Empty;
        /// <summary>
        /// Gets and sets the result of last predicate evaluation.
        /// </summary>
        public string EvaluationResult
        {
            get => evaluationResult;
            set => SetPropertyValue(ref evaluationResult, value);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Shortcut method to update and notify about property value change.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="store">Property value storage.</param>
        /// <param name="value">New property value.</param>
        /// <param name="propertyName">Property name.</param>
        protected void SetPropertyValue<T>(ref T store, T value, [CallerMemberName] string? propertyName = null)
        {
            store = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
