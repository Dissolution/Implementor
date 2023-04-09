using System.Reflection;

namespace Jay.SourceGen.Signatures;

public sealed class TypeSig : MemberSig, IEquatable<TypeSig>, IEquatable<ITypeSymbol>, IEquatable<Type>
{
    public static implicit operator TypeSig(Type type) => new TypeSig(type);

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

    public ObjType ObjType { get; set; }
    public bool CanBeNull { get; set; }

    public TypeSig(ITypeSymbol typeSymbol) : base(typeSymbol)
    {
        this.MemberType = MemberTypes.TypeInfo;
        this.CanBeNull = !typeSymbol.IsValueType;
        switch (typeSymbol.TypeKind)
        {
            case TypeKind.Struct:
            {
                this.ObjType = ObjType.Struct;
                break;
            }
            case TypeKind.Interface:
            {
                this.ObjType = ObjType.Interface;
                break;
            }
            case TypeKind.Class:
            {
                this.ObjType = ObjType.Class;
                break;
            }
            default:
                throw new InvalidOperationException();
        }
    }

    public TypeSig(Type type) : base(type)
    {
        this.MemberType = MemberTypes.TypeInfo;
        this.FullName = type.FullName;
        this.CanBeNull = !type.IsValueType;
        if (type.IsValueType)
        {
            this.ObjType = ObjType.Struct;
        }
        else if (type.IsInterface)
        {
            this.ObjType = ObjType.Interface;
        }
        else if (type.IsClass)
        {
            this.ObjType = ObjType.Class;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public TypeSig() : base()
    {
        this.MemberType = MemberTypes.TypeInfo;
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
