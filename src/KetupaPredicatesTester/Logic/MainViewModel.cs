namespace Trogon.KetupaPredicates.Tester.Logic
{
    using Data;

    /// <summary>
    /// ViewModel for the Main view.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Data for the view.
        /// </summary>
        public PredicateConfiguration Configuration { get; protected set; } = new PredicateConfiguration();

        /// <summary>
        /// Add variable ICommand for UI button.
        /// </summary>
        public AddVariableCommand AddVariableCommand { get; protected set; }
        /// <summary>
        /// Remove variable ICommand for UI button.
        /// </summary>
        public RemoveVariableCommand RemoveVariableCommand { get; protected set; }
        /// <summary>
        /// Evaluate predicate expression ICommand for UI button.
        /// </summary>
        public EvaluateCommand EvaluateCommand { get; protected set; }

        /// <summary>
        /// Prepare instanced for the ViewModel.
        /// </summary>
        public MainViewModel()
        {
            Configuration = new PredicateConfiguration();
            AddVariableCommand = new AddVariableCommand(Configuration);
            RemoveVariableCommand = new RemoveVariableCommand(Configuration);
            EvaluateCommand = new EvaluateCommand(Configuration);
        }
    }
}
