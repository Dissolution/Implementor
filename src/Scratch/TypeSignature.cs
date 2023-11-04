using Implementor.Reflection;

namespace Implementor.Scratch;

public record class TypeSignature : MemberSignature
{
    public static TypeSignature? Create(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) return null;
        TypeSignature signature = new()
        {
            Name = typeSymbol.Name,
            Visibility = typeSymbol.GetVisibility(),
            Keywords = typeSymbol.GetKeywords(),
            Namespace = typeSymbol.GetNamespace(),
            Kind = typeSymbol.TypeKind,
            BaseType = Create(typeSymbol.BaseType),
            Attributes = Attributes.From(typeSymbol),
        };
        return signature;
    }
    
    
    public string? Namespace { get; set; } = null;
    public TypeKind Kind { get; set; } = TypeKind.Unknown;

    public string FullyQualifiedName => $"{Namespace}.{Name}";
}