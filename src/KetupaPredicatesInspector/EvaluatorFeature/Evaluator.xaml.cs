using System.Windows.Controls;

namespace Trogon.KetupaPredicates.Inspector.EvaluatorFeature;

/// <summary>
/// Interaction logic for Evaluator.xaml
/// </summary>
public partial class Evaluator : UserControl {
    /// <summary>
    /// Logic and data for the UI.
    /// </summary>
    public EvaluatorViewModel ViewModel { get; protected set; }

    /// <summary>
    /// Initialize ViewModel instance and bind it to the view initialization.
    /// </summary>
    public Evaluator() {
        ViewModel = new EvaluatorViewModel();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
