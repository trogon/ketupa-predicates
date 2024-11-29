namespace Trogon.KetupaPredicates.Inspector.DesignerFeature;

using System;
using System.Diagnostics;
using System.Text;

/// <summary>
/// Manages predicate construction.
/// </summary>
public class PredicateTree : PredicateNode
{
    private PredicateNode? rootPredicate;

    /// <summary>
    /// Main predicate object.
    /// </summary>
    public PredicateNode? RootPredicate {
        get => rootPredicate;
        set {
            rootPredicate = value;
            Arguments.Clear();
            if (value != null) {
                Arguments.Add(value);
            }
        }
    }

    internal override void AddChild(PredicateNode node) {
        Debug.Assert(node != null);
        throw new InvalidOperationException("Unable to add child. Root have only one child.");
    }

    internal override void DeleteChild(PredicateNode node) {
        Debug.Assert(node != null);
        throw new InvalidOperationException("Unable to add child. Root have only one child.");
    }

    internal override void ReplaceChild(PredicateNode node, PredicateNode oldNode) {
        Debug.Assert(node != null);
        throw new InvalidOperationException("Unable to add child. Root have only one child.");
    }

    internal override void InsertChild(PredicateNode node, PredicateNode locationNode, InsertPlacement placement) {
        Debug.Assert(node != null);
        throw new InvalidOperationException("Unable to add child. Root have only one child.");
    }

    /// <summary>
    /// Construct predicate text.
    /// </summary>
    /// <returns>Usable predicate text.</returns>
    internal string GetPredicateText() {
        StringBuilder predicateBuilder = new();
        Stack<PredicateNode> predicates = new();

        if (RootPredicate != null) {
            predicates.Push(RootPredicate);
        }

        int openGroups = 0;

        while (predicates.Count > 0) {
            var node = predicates.Pop();
            foreach (var child in node.Arguments.Reverse()) {
                predicates.Push(child);
            }

            // Start group for operator (only operator has children)
            if (node.Arguments.Count > 0) {
                predicateBuilder.Append('{');
                openGroups++;
            }

            predicateBuilder.Append(node.PredicateText);

            if (predicates.TryPeek(out var nextNode)) {
                // End group for current operator.
                if (nextNode.Parent != node.Parent && nextNode.Parent != node) {
                    predicateBuilder.Append('}');
                    openGroups--;
                }
                predicateBuilder.Append(',');
            }
        }

        // Close open groups.
        for (int i = 0; i < openGroups; i++) {
            predicateBuilder.Append('}');
        }

        return predicateBuilder.ToString();
    }
}
