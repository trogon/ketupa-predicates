using System.Windows;
using System.Windows.Controls;

namespace Trogon.KetupaPredicates.Inspector.DesignerFeature;

/// <summary>
/// Interaction logic for Designer.xaml
/// </summary>
public partial class Designer : UserControl {
    /// <summary>
    /// Logic and data for the UI.
    /// </summary>
    public DesignerViewModel ViewModel { get; protected set; }

    /// <summary>
    /// Initialize ViewModel instance and bind it to the view initialization.
    /// </summary>
    public Designer() {
        ViewModel = new DesignerViewModel();
        DataContext = ViewModel;
        InitializeComponent();
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
        // Update selected predicate node in ViewModel (unable to bind the value).
        if (e.NewValue == null) {
           ViewModel.SelectedNode = null;
        } else if (e.NewValue is PredicateNode node) {
           ViewModel.SelectedNode = node;
        } else {
            System.Diagnostics.Debug.WriteLine($"Selected item in TreeView has unexpected type {e?.NewValue?.GetType()}");
        }
    }

    private void OperandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        // User shortcut, select option as focus changed on input control.
        ViewModel.IsOperand = true;
    }

    private void VariableComboBox_GotFocus(object sender, RoutedEventArgs e) {
        // User shortcut, select option as focus changed on input control.
        ViewModel.IsVariable = true;
    }

    private void ConstantTextBox_GotFocus(object sender, RoutedEventArgs e) {
        // User shortcut, select option as focus changed on input control.
        ViewModel.IsConstant = true;
    }
}
