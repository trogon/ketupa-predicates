namespace Trogon.KetupaPredicates.Tester.Logic
{
    using Data;

    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Add specified variable to list and prepare model for next variable definition.
    /// </summary>
    public class AddVariableCommand : ICommand
    {
        private PredicateConfiguration configuration;
        private bool lastCanExecute = false;

        /// <summary>
        /// Constructor to preapare and bind to configuration data.
        /// </summary>
        /// <param name="configuration">Configuration model with data.</param>
        public AddVariableCommand(PredicateConfiguration configuration)
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
                || string.IsNullOrEmpty(configuration.VariableName)
                || string.IsNullOrEmpty(configuration.VariableTextValue)
                || configuration.Variables == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds the new variable to the data model.
        /// Fetch the variable specification from data model.
        /// Clears data for new variable in the data model.
        /// </summary>
        /// <param name="parameter">NOT USED.</param>
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                var name = configuration.VariableName;
                var value = configuration.VariableTextValue;
                configuration.Variables.Add(new PredicateVariable(name, value));
                configuration.VariableName = string.Empty;
                configuration.VariableTextValue = string.Empty;

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
    }
}
