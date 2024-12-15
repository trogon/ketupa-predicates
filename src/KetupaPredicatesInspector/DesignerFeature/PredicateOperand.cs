namespace Trogon.KetupaPredicates.Inspector.DesignerFeature;

/// <summary>
/// Predicate operand details.
/// </summary>
/// <param name="Name">User friendly name.</param>
/// <param name="Token">Token used in predicate.</param>
public record class PredicateOperand(string Name, string Token) {
}
