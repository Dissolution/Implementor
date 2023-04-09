using Jay.SourceGen.Enums;
using Jay.SourceGen.Signatures;

using Microsoft.CodeAnalysis;

namespace Jay.SourceGen.Reflection;

[Flags]
public enum SigType
{
    Field = 1 << 0,
    Property = 1 << 1,
    Event = 1 << 2,
    Constructor = 1 << 3,
    Method = 1 << 4,
    Type = 1 << 5,
    Operator = 1 << 6,
    Parameter = 1 << 7,
    Attribute = 1 << 8,
}

partial class MemberSig
{
    public static bool operator ==(MemberSig? left, MemberSig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(MemberSig? left, MemberSig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }

    public static MemberSig? Create(ISymbol? symbol)
    {
        return symbol switch
        {
            IFieldSymbol fieldSymbol => new FieldSig(fieldSymbol),
            IPropertySymbol propertySymbol => new PropertySig(propertySymbol),
            IEventSymbol eventSymbol => new EventSig(eventSymbol),
            IMethodSymbol methodSymbol => new MethodSig(methodSymbol),
            ITypeSymbol typeSymbol => new TypeSig(typeSymbol),
            _ => null,
        };
    }

    public static MemberSig? Create(MemberInfo? member)
    {
        return member switch
        {
            FieldInfo fieldInfo => new FieldSig(fieldInfo),
            PropertyInfo propertyInfo => new PropertySig(propertyInfo),
            EventInfo eventInfo => new EventSig(eventInfo),
            ConstructorInfo ctorInfo => new MethodSig(ctorInfo),
            MethodInfo methodInfo => new MethodSig(methodInfo),
            Type type => new TypeSig(type),
            _ => null,
        };
    }
}

public abstract partial class MemberSig :
    IEquatable<MemberSig>, IEquatable<ISymbol>, IEquatable<MemberInfo>
{
    public SigType SigType { get; set; } = default;

    public TypeSig? ParentType { get; set; } = null;

    public string? Name { get; set; } = null;

    public string FullName
    {
        get
        {
            if (ParentType is null)
            {
                return Name ?? "";
            }
            return $"{ParentType}.{Name ?? "???"}";
        }
    }

    public Visibility Visibility { get; set; } = default;
    public Instic Instic { get; set; } = default;
    public Keywords Keywords { get; set; } = default;


    protected MemberSig(SigType sigType)
    {
        this.SigType = sigType;
    }

    protected MemberSig(SigType sigType, ISymbol symbol)
        : this(sigType)
    {
        this.Name = symbol.Name;
        this.ParentType = TypeSig.Create(symbol.ContainingType);
        this.Visibility = symbol.GetVisibility();
        this.Instic = symbol.GetInstic();
        this.Keywords = symbol.GetKeywords();
    }

    protected MemberSig(SigType sigType, MemberInfo member)
        : this(sigType)
    {
        this.Name = member.Name;
        this.ParentType = TypeSig.Create(member.ReflectedType ?? member.DeclaringType);
        this.Visibility = member.GetVisibility();
        this.Instic = member.GetInstic();
        this.Keywords = member.GetKeywords();
    }

    public virtual bool Equals(MemberSig? memberSig)
    {
        return memberSig is not null &&
            this.SigType == memberSig.SigType &&
            string.Equals(this.Name, memberSig.Name);
    }

    public virtual bool Equals(ISymbol? symbol) => Equals(Create(symbol));

    public virtual bool Equals(MemberInfo? member) => Equals(Create(member));

    public override bool Equals(object obj)
    {
        if (obj is MemberSig memberSig) return Equals(memberSig);
        if (obj is ISymbol symbol) return Equals(symbol);
        if (obj is MemberInfo member) return Equals(member);
        return false;
    }

    public sealed override int GetHashCode()
    {
        throw new NotSupportedException("All MemberSigs are mutable and not suited for hashcodes");
    }

    public override string ToString()
    {
        return $"{SigType} {Name}";
    }


}

public class FieldSig : MemberSig
//,    IEquatable<FieldSig>, IEquatable<IFieldSymbol>, IEquatable<FieldInfo>
{
    public TypeSig? FieldType { get; set; } = null;

    public FieldSig()
        : base(SigType.Field)
    {

    }

    public FieldSig(IFieldSymbol fieldSymbol)
        : base(SigType.Field, fieldSymbol)
    {
        this.FieldType = new TypeSig(fieldSymbol.Type);
    }

    public FieldSig(FieldInfo fieldInfo)
        : base(SigType.Field, fieldInfo)
    {
        this.FieldType = new TypeSig(fieldInfo.FieldType);
    }
}

public class PropertySig : MemberSig,
    IEquatable<PropertySig>, IEquatable<IPropertySymbol>, IEquatable<PropertyInfo>
{

}

public class EventSig : MemberSig,
    IEquatable<EventSig>, IEquatable<IEventSymbol>, IEquatable<EventInfo>
{

}

public class MethodSig : MemberSig,
    IEquatable<MethodSig>, IEquatable<IMethodSymbol>, IEquatable<MethodBase>
{

}

public class TypeSig : MemberSig,
    IEquatable<TypeSig>, IEquatable<ITypeSymbol>, IEquatable<Type>
{
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

    public string? Namespace {get; set;}
}

public class ParameterSig :
    IEquatable<ParameterSig>, IEquatable<IParameterSymbol>, IEquatable<ParameterInfo>
{

}

public class AttributeSig :
    IEquatable<AttributeSig>, IEquatable<AttributeData>, IEquatable<CustomAttributeData>
{

}

