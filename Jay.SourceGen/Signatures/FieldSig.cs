namespace Jay.SourceGen.Signatures;

public sealed class FieldSig : MemberSig, IEquatable<FieldSig>, IEquatable<IFieldSymbol>, IEquatable<FieldInfo>
{
    public static bool operator ==(FieldSig? left, FieldSig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(FieldSig? left, FieldSig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }

    public TypeSig FieldType { get; set;}

    public FieldSig(IFieldSymbol fieldSymbol)
        : base(fieldSymbol)
    {
        MemberType = MemberTypes.Field;
        FieldType = new(fieldSymbol.Type);
    }

    public FieldSig(FieldInfo fieldInfo)
        : base(fieldInfo)
    {
        MemberType = MemberTypes.Field;
        FieldType = new(fieldInfo.FieldType);
    }

    public FieldSig() : base()
    {
        MemberType = MemberTypes.Field;
    }

    public bool Equals(FieldSig? FieldSig)
    {
        return FieldSig is not null &&
            FullName == FieldSig.FullName &&
            FieldType.Equals(FieldSig.FieldType);
    }

    public bool Equals(IFieldSymbol? fieldSymbol)
    {
        return fieldSymbol is not null &&
           FullName == fieldSymbol.GetFullName() &&
           FieldType.Equals(fieldSymbol.Type);
    }

    public bool Equals(FieldInfo? FieldInfo)
    {
        return FieldInfo is not null &&
           FullName == $"{FieldInfo.ReflectedType.FullName}.{FieldInfo.Name}" &&
           FieldType.Equals(FieldInfo.FieldType);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is FieldSig fieldSig && Equals(fieldSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IFieldSymbol fieldSymbol && Equals(fieldSymbol);
    }

    public override bool Equals(MemberInfo? member)
    {
        return member is FieldInfo fieldInfo && Equals(fieldInfo);
    }

    public override bool Equals(object? obj)
    {
        if (obj is FieldSig fieldSig) return Equals(fieldSig);
        if (obj is IFieldSymbol fieldSymbol) return Equals(fieldSymbol);
        if (obj is FieldInfo fieldInfo) return Equals(fieldInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(FullName, FieldType);
    }

    public override string ToString()
    {
        return $$"""
            [{{string.Join(", ", Attributes)}}]
            {{Visibility}} {{Instic}} {{Keywords}} {{FullName}}
            """;
    }
}