using System.Reflection;

namespace Jay.SourceGen.Signatures;

public sealed class AttributeSig : IEquatable<AttributeSig>, IEquatable<AttributeData>
{
    public static bool operator ==(AttributeSig left, AttributeSig right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(AttributeSig left, AttributeSig right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }

    public string Name { get; }
    public string FullName { get; }
    public AttributeArgsCollection Data { get; }

    public AttributeSig(AttributeData attributeData)
    {
        if (attributeData.AttributeClass is null)
            throw new ArgumentException("", nameof(attributeData));
        Name = attributeData.AttributeClass.Name;
        FullName = attributeData.AttributeClass.GetFullName();
        Data = attributeData.GetArgs();
    }

    public AttributeSig(CustomAttributeData attrData)
    {
        var attrType = attrData.AttributeType;
        Name = attrType.Name;
        FullName = attrType.FullName;
        Data = new AttributeArgsCollection(attrData);
    }

    public bool Equals(AttributeSig? attributeSig)
    {
        return attributeSig is not null &&
            FullName == attributeSig.FullName &&
            Data.SequenceEqual(attributeSig.Data);
    }

    public bool Equals(AttributeData? attributeData)
    {
        return attributeData is not null &&
            FullName == attributeData.AttributeClass?.GetFullName() &&
            Data.SequenceEqual(attributeData.GetArgs());
    }

    public override bool Equals(object obj)
    {
        if (obj is AttributeSig attributeSig) return Equals(attributeSig);
        if (obj is AttributeData attributeData) return Equals(attributeData);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(FullName, Data);
    }

    public override string ToString()
    {
        return $"[{FullName}({string.Join<string>(",", Data.Select(p => $"{p.Key} = {p.Value}"))})]";
    }
}
