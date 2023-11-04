using Implementor.Collections;
using Implementor.Reflection;

namespace Implementor.Scratch;

public record class AttributeSignature : Signature
{
    public static AttributeSignature? Create(AttributeData? attributeData)
    {
        if (attributeData is null) return null;
        var signature = new AttributeSignature
        {
            Name = attributeData.AttributeClass?.Name ?? attributeData.ToString(),
            Visibility = Visibility.Public | Visibility.Static,
            //Type = TypeSignature.Create(attributeData.AttributeClass),
            Arguments = AttributeArguments.Create(attributeData),
        };
        return signature;
    }
    
    public AttributeArguments Arguments { get; set; } = new();
}