using Implementor.Reflection;

namespace Implementor.Scratch;

public record class MethodSignature : MemberSignature
{
    public static MethodSignature? Create(IMethodSymbol? methodSymbol)
    {
        if (methodSymbol is null) return null;
        return new MethodSignature()
        {
            Name = methodSymbol.Name,
            Visibility = methodSymbol.GetVisibility(),
            Keywords = methodSymbol.GetKeywords(),
            Attributes = Scratch.Attributes.From(methodSymbol),
            Parameters = Parameters.From(methodSymbol.Parameters),
            ReturnType = TypeSignature.Create(methodSymbol.ReturnType),
        };
    }

    public Parameters Parameters { get; set; } = new();
    public TypeSignature? ReturnType { get; set; } = null;
}