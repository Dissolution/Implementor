using Implementor.Reflection;

namespace Implementor.Scratch;

public record class ParameterSignature : Signature
{
    public static ParameterSignature? Create(IParameterSymbol? parameterSymbol)
    {
        if (parameterSymbol is null) return null;
        return new ParameterSignature()
        {
            Name = parameterSymbol.Name,
            Visibility = parameterSymbol.GetVisibility(),
            Keywords = parameterSymbol.GetKeywords(),
            Attributes = Attributes.From(parameterSymbol),
            ValueType = TypeSignature.Create(parameterSymbol.Type),
            RefKind = parameterSymbol.RefKind,
            IsInstance = parameterSymbol.IsThis,
            IsParams = parameterSymbol.IsParams,
            DefaultValue = parameterSymbol.HasExplicitDefaultValue ? new(parameterSymbol.ExplicitDefaultValue) : default,
        };
    }
    
    public TypeSignature? ValueType { get; set; } = null;
    public RefKind RefKind { get; set; } = RefKind.None;
    public Optional<object?> DefaultValue { get; set; } = default;
    public bool IsInstance { get; set; } = false;
    public bool IsParams { get; set; } = false;
}