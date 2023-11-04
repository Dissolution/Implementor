using Implementor.Reflection;

namespace Implementor.Scratch;

public record class PropertySignature : MemberSignature
{
    public static PropertySignature? Create(IPropertySymbol? propertySymbol)
    {
        if (propertySymbol is null) return null;
        return new PropertySignature()
        {
            Name = propertySymbol.Name,
            Visibility = propertySymbol.GetVisibility(),
            Keywords = propertySymbol.GetKeywords(),
            Attributes = Attributes.From(propertySymbol),
            ValueType = TypeSignature.Create(propertySymbol.Type),
            Getter = MethodSignature.Create(propertySymbol.GetMethod),
            Setter = MethodSignature.Create(propertySymbol.SetMethod),
            IsIndexer = propertySymbol.IsIndexer,
            Parameters = Parameters.From(propertySymbol.Parameters),
        };
    }

    public FieldSignature? BackingField { get; set; } = null;
    public TypeSignature? ValueType { get; set; } = null;
    public MethodSignature? Getter { get; set; } = null;
    public MethodSignature? Setter { get; set; } = null;
    public bool IsIndexer { get; set; } = false;
    public Parameters Parameters { get; set; } = new();
}