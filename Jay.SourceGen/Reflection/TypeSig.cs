namespace Jay.SourceGen.Reflection;

public sealed class TypeSig : MemberSig,
    IEquatable<TypeSig>, IEquatable<ITypeSymbol>, IEquatable<Type>
{
    [return: NotNullIfNotNull(nameof(type))]
    public static implicit operator TypeSig?(Type? type) => TypeSig.Create(type);

    public static bool operator ==(TypeSig? left, TypeSig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(TypeSig? left, TypeSig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }
    public static bool operator ==(TypeSig? left, ITypeSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(TypeSig? left, ITypeSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(TypeSig? left, Type? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(TypeSig? left, Type? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    public static TypeSig? Create(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) return null;
        return new TypeSig(typeSymbol);
    }

    public static TypeSig? Create(Type? type)
    {
        if (type is null) return null;
        return new TypeSig(type);
    }

    public string? Namespace { get; set; }
    public override string FullName { get; }

    public TypeSig(ITypeSymbol typeSymbol)
        : base(SigType.Type, typeSymbol)
    {
        this.Namespace = typeSymbol.GetNamespace();
        this.FullName = typeSymbol.GetFullName();
    }

    public TypeSig(Type type)
       : base(SigType.Type, type)
    {
        this.Namespace = type.Namespace;
        this.FullName = type.FullName;
    }

    public TypeSig()
       : base(SigType.Type)
    {

    }

    public bool Equals(TypeSig? typeSig)
    {
        return typeSig is not null &&
            string.Equals(this.FullName, typeSig.FullName);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is TypeSig typeSig && Equals(typeSig);
    }

    public bool Equals(ITypeSymbol? typeSymbol)
    {
        return typeSymbol is not null &&
            string.Equals(this.FullName, typeSymbol.GetFullName());
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is ITypeSymbol typeSymbol && Equals(typeSymbol);
    }

    public bool Equals(Type? type)
    {
        return type is not null &&
                   string.Equals(this.FullName, type.FullName);
    }

    public override bool Equals(MemberInfo? member)
    {
        return member is Type type && Equals(type);
    }

    public override bool Equals(object obj)
    {
        if (obj is TypeSig typeSig) return Equals(typeSig);
        if (obj is ITypeSymbol typeSymbol) return Equals(typeSymbol);
        if (obj is Type type) return Equals(type);
        return false;
    }

    public override string ToString()
    {
        return FullName;
    }

}