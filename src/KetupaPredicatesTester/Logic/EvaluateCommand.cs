namespace Trogon.KetupaPredicates.Tester.Logic
{
    using Data;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Evaluate the provided predicate expression with variables.
    /// </summary>
    public class EvaluateCommand : ICommand
    {
        private PredicateConfiguration configuration;
        private bool lastCanExecute = false;

        /// <summary>
        /// Constructor to preapare and bind to configuration data.
        /// </summary>
        /// <param name="configuration">Configuration model with data.</param>
        public EvaluateCommand(PredicateConfiguration configuration)
        {
            this.configuration = configuration;
            lastCanExecute = CanExecute(null);
            configuration.PropertyChanged += Configuration_PropertyChanged;
        }

        /// <summary>
        /// Checks if it is safe to run the command with current data.
        /// </summary>
        /// <param name="parameter">NOT USED.</param>
        /// <returns>True if command can be run, otherwise False.</returns>
        public bool CanExecute(object? parameter)
        {
            if (configuration == null
                || string.IsNullOrEmpty(configuration.PredicateText)
                || configuration.Variables == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Performs the evaluation command and set the result in data model.
        /// </summary>
        /// <param name="parameter">NOT USED.</param>
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                var variables = PrepareVariables();
                if (variables != null)
                {
                    var expression = new PredicateExpression(configuration.PredicateText);
                    expression.Prepare();
                    configuration.EvaluationResult = expression.Evaluate(variables).ToString();
                }

                RiseCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Updates the can execute and raise an event if value changed.
        /// </summary>
        protected void RiseCanExecuteChanged()
        {
            var previousCanExecute = lastCanExecute;
            lastCanExecute = CanExecute(null);

            if (previousCanExecute != lastCanExecute)
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Configuration_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RiseCanExecuteChanged();
        }

        private IDictionary<string, object>? PrepareVariables()
        {
            var variables = new Dictionary<string, object>();
            foreach (var item in configuration.Variables.ToList())
            {
                if (variables.ContainsKey(item.Name))
                {
                    configuration.EvaluationResult =
                        $"Variable is defined twice: {item.Name}. Unable to continue, please remove duplicate definition.";
                    return null;
                }

                variables[item.Name] = item.TextValue;
            }

            return variables;
        }
    }
}
