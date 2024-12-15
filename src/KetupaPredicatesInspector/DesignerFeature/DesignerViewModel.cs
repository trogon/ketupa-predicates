using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Text.RegularExpressions;

namespace Trogon.KetupaPredicates.Inspector.DesignerFeature;

/// <summary>
/// ViewModel for the Designer control.
/// </summary>
public partial class DesignerViewModel : ObservableObject
{
    private static Regex variablePattern = new(@"^[a-zA-Z0-9]+(\[[0-9]+\])*$");

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AppendCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplaceCommand))]
    private PredicateNode? selectedNode;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AppendCommand))]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplaceCommand))]
    private PredicateOperand? selectedOperand;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AppendCommand))]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplaceCommand))]
    private string? constantValue = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AppendCommand))]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplaceCommand))]
    private string? variableName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AppendCommand))]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplaceCommand))]
    private bool? isOperand = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AppendCommand))]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplaceCommand))]
    private bool? isConstant = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AppendCommand))]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplaceCommand))]
    private bool? isVariable = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    private bool? isInsertBefore = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(InsertCommand))]
    private bool? isInsertAfter = true;

    [ObservableProperty]
    private string predicateText = string.Empty;

    /// <summary>
    /// List of operands to select from for new node.
    /// </summary>
    public List<PredicateOperand> AvailableOperands { get; protected set; }


    /// <summary>
    /// Current predicate visual tree.
    /// </summary>
    public PredicateTree Predicate { get; protected set; }

    /// <summary>
    /// Prepare AvailableOperands and Predicate.
    /// </summary>
    public DesignerViewModel() {
        AvailableOperands = new List<PredicateOperand>()
        {
            new("Equals", "="),
            new("Less than", "<"),
            new("Greater than", ">"),
            new("Less than or equal", "<="),
            new("Greater than or equal", ">="),

            new("Logical negation", "NOT"),
            new("Logical or", "OR"),
            new("Logical and", "AND"),

            new("Binary has flag", "HasFlag"),
            new("Text contains", "IN"),
        };

        SelectedOperand = AvailableOperands.FirstOrDefault();

        Predicate = new PredicateTree();
    }

    internal bool CanMakePredicateNode() {
        if (IsOperand == true && SelectedOperand != null) {
            return true;
        }

        if (IsVariable == true && !string.IsNullOrEmpty(VariableName)
            && variablePattern.IsMatch(VariableName)) {
            return true;
        }

        if (IsConstant == true && ConstantValue != null) {
            return true;
        }

        return false;
    }

    internal PredicateNode? GetPredicateNode() {
        PredicateNode? node = null;

        if (IsOperand == true && SelectedOperand != null) {
            node = new PredicateNode() {
                DisplayText = SelectedOperand.Name,
                PredicateText = SelectedOperand.Token
            };
        }

        if (IsVariable == true && !string.IsNullOrEmpty(VariableName)) {
            node = new PredicateNode() {
                DisplayText = $"${VariableName}",
                PredicateText = $"${VariableName}"
            };
            VariableName = string.Empty;
        }

        if (IsConstant == true && ConstantValue != null) {
            node = new PredicateNode() {
                DisplayText = ConstantValue == string.Empty ? "<Empty>" : ConstantValue,
                PredicateText = ConstantValue,
            };
            ConstantValue = string.Empty;
        }

        return node;
    }

    internal void UpdatePredicateText() {
        PredicateText = Predicate.GetPredicateText();
    }

    private bool CanDelete() {
        return SelectedNode != null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete() {
        var nodeToRemove = SelectedNode;
        if (nodeToRemove != null) {
            var nodeParent = nodeToRemove.Parent;
            if (nodeParent != null) {
                var remIndex = nodeParent.Arguments.IndexOf(nodeToRemove);
                nodeParent.DeleteChild(nodeToRemove);
                if (nodeParent.Arguments.Count > 0) {
                    SelectedNode = nodeParent.Arguments[remIndex > 0 ? remIndex - 1 : 0];
                }
                else {
                    SelectedNode = nodeParent;
                }
                UpdatePredicateText();
            }
            else {
                Predicate.RootPredicate = null;
                nodeToRemove.Deconstruct();
                SelectedNode = null;
                UpdatePredicateText();
            }
        }
    }

    private bool CanAppend() {
        return CanMakePredicateNode()
            && (SelectedNode != null || Predicate.RootPredicate == null);
    }

    [RelayCommand(CanExecute = nameof(CanAppend))]
    private void Append() {
        var currentNode = SelectedNode;
        PredicateNode? predicateNode = GetPredicateNode();
        if (predicateNode != null) {
            if (currentNode != null) {
                currentNode.AddChild(predicateNode);
                SelectedNode = predicateNode;
                UpdatePredicateText();
            }
            else if (Predicate.RootPredicate == null) {
                Predicate.RootPredicate = predicateNode;
                SelectedNode = predicateNode;
                UpdatePredicateText();
            }
        }
    }

    private bool CanInsert() {
        return CanMakePredicateNode()
            && SelectedNode?.Parent != null;
    }

    [RelayCommand(CanExecute = nameof(CanInsert))]
    private void Insert() {
        var currentNode = SelectedNode;
        PredicateNode? predicateNode = GetPredicateNode();
        if (predicateNode != null
            && currentNode?.Parent != null) {
            var parentNode = currentNode.Parent;
            parentNode.InsertChild(predicateNode, currentNode,
                IsInsertBefore == true ? InsertPlacement.Before : InsertPlacement.After);
            UpdatePredicateText();
            SelectedNode = predicateNode;
        }
    }

    private bool CanReplace() {
        return CanMakePredicateNode()
            && SelectedNode != null;
    }


    [RelayCommand(CanExecute = nameof(CanReplace))]
    private void Replace() {
        var nodeToReplace = SelectedNode;
        PredicateNode? predicateNode = GetPredicateNode();
        if (predicateNode != null
            && nodeToReplace != null) {
            var nodeParent = nodeToReplace.Parent;
            if (nodeParent != null) {
                nodeParent.ReplaceChild(predicateNode, nodeToReplace);
                UpdatePredicateText();
                SelectedNode = predicateNode;
            }
            else {
                Predicate.RootPredicate = predicateNode;
                nodeToReplace.Deconstruct();
                UpdatePredicateText();
                SelectedNode = predicateNode;
            }
        }
    }

    partial void OnSelectedNodeChanging(PredicateNode? oldValue, PredicateNode? newValue) {
        if (oldValue != null) {
            oldValue.IsSelected = false;
        }
        if (newValue != null) {
            newValue.IsSelected = true;
        }
    }
}
