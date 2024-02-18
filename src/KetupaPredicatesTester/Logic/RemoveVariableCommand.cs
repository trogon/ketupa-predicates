namespace Trogon.KetupaPredicates.Tester.Logic
{
    using Data;

    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Remove selected variable from list.
    /// </summary>
    public class RemoveVariableCommand : ICommand
    {
        private PredicateConfiguration configuration;
        private bool lastCanExecute = false;

        /// <summary>
        /// Constructor to preapare and bind to configuration data.
        /// </summary>
        /// <param name="configuration">Configuration model with data.</param>
        public RemoveVariableCommand(PredicateConfiguration configuration)
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
                || configuration.SelectedVariable == null
                || configuration.Variables == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes the selected variable from the data model.
        /// </summary>
        /// <param name="parameter">NOT USED.</param>
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                configuration.Variables.Remove(configuration.SelectedVariable!);
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
