using Implementor.Reflection;

namespace Implementor.Scratch;

public record class FieldSignature : MemberSignature
{
    public static FieldSignature? Create(IFieldSymbol? fieldSymbol)
    {
        if (fieldSymbol is null) return null;
        return new FieldSignature()
        {
            Name = fieldSymbol.Name,
            Visibility = fieldSymbol.GetVisibility(),
            Keywords = fieldSymbol.GetKeywords(),
            Attributes = Attributes.From(fieldSymbol),
            ValueType = TypeSignature.Create(fieldSymbol.Type),
        };
    }
    
    public TypeSignature? ValueType { get; set; } = null;
}