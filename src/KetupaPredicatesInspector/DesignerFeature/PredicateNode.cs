using System.Collections.ObjectModel;
using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Trogon.KetupaPredicates.Inspector.DesignerFeature;

public partial class PredicateNode : ObservableObject
{
    [ObservableProperty]
    private bool isExpanded = true;

    [ObservableProperty]
    private bool isSelected = false;

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty]
    private string displayText = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty]
    private string predicateText = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public PredicateNode? Parent { get; protected set; }

    /// <summary>
    /// List of arguments.
    /// </summary>
    public ObservableCollection<PredicateNode> Arguments { get; protected set; } = new ObservableCollection<PredicateNode>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    internal virtual void AddChild(PredicateNode node) {
        Debug.Assert(node != null);
        Arguments.Add(node);
        node.Parent = this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="locationNode"></param>
    /// <param name="placement"></param>
    internal virtual void InsertChild(PredicateNode node, PredicateNode locationNode, InsertPlacement placement) {
        if (placement == InsertPlacement.Before) {
            Debug.Assert(locationNode?.Parent == this);
            var location = Arguments.IndexOf(locationNode);

            Debug.Assert(node != null);
            Debug.Assert(location >= 0, "Location node not found in collection.");
            Arguments.Insert(location, node);
            node.Parent = this;
        }
        else {
            Debug.Assert(locationNode?.Parent == this);
            var location = Arguments.IndexOf(locationNode);

            Debug.Assert(node != null);
            Debug.Assert(location >= 0, "Location node not found in collection.");
            Arguments.Insert(location + 1, node);
            node.Parent = this;
        }
    }

    /// <summary>
    /// Replace given node in current prodicate with other.
    /// </summary>
    /// <param name="node">Node to place.</param>
    /// <param name="oldNode">Node to remove.</param>
    internal virtual void ReplaceChild(PredicateNode node, PredicateNode oldNode) {
        Debug.Assert(oldNode?.Parent == this);
        var location = Arguments.IndexOf(oldNode);

        Debug.Assert(node != null);
        Debug.Assert(location >= 0, "Location node not found in collection.");
        if (Arguments.Remove(oldNode)) {
            Arguments.Insert(location, node);
            node.Parent = this;
            oldNode.Deconstruct();
        }
    }

    /// <summary>
    /// Remove node from current predicate.
    /// </summary>
    /// <param name="node">Predicate node to remove.</param>
    internal virtual void DeleteChild(PredicateNode node) {
        Debug.Assert(node != null);
        if (Arguments.Remove(node)) {
            node.Parent = null;
            node.Deconstruct();
        }
    }

    /// <summary>
    /// Removes loop dependency, allow GC to remove objects.
    /// </summary>
    internal virtual void Deconstruct() {
        Queue<PredicateNode> queue = new();
        queue.Enqueue(this);

        while (queue.Count > 0) {
            var node = queue.Dequeue();
            node.Parent = null;

            foreach (var child in node.Arguments) {
                queue.Enqueue(child);
            }
        }
    }
}
