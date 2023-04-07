namespace Jay.SourceGen.Signatures;

public sealed class TypeSig : IEquatable<TypeSig>, IEquatable<ITypeSymbol>, IEquatable<Type>
{
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

    public string Name { get; }
    public string FullName { get; }
    public bool CanBeNull { get; }

    public TypeSig(ITypeSymbol typeSymbol)
    {
        Name = typeSymbol.Name;
        FullName = typeSymbol.GetFullName();
        CanBeNull = !typeSymbol.IsValueType;
    }

    public TypeSig(Type type)
    {
        Name = type.Name;
        FullName = type.FullName;
        CanBeNull = !type.IsValueType;
    }

    public bool Equals(TypeSig? typeSig)
    {
        return typeSig is not null &&
            string.Equals(FullName, typeSig.FullName);
    }

    public bool Equals(ITypeSymbol? typeSymbol)
    {
        return typeSymbol is not null &&
            string.Equals(FullName, typeSymbol.GetFullName());
    }

    public bool Equals(Type? type)
    {
        return type is not null &&
            string.Equals(FullName, type.FullName);
    }

    public bool Equals<T>() => Equals(typeof(T));

    public override bool Equals(object? obj)
    {
        if (obj is TypeSig typeSig) return Equals(typeSig);
        if (obj is ITypeSymbol typeSymbol) return Equals(typeSymbol);
        if (obj is Type type) return Equals(type);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(FullName);
    }

    public override string ToString()
    {
        return FullName;
    }
}
