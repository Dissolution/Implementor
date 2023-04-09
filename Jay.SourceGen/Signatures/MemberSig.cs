using Jay.SourceGen.Enums;

using System.Reflection;

namespace Jay.SourceGen.Signatures;

public abstract class MemberSig :
    IEquatable<MemberSig>, IEquatable<ISymbol>, IEquatable<MemberInfo>
{
     public static MemberSig Create(ISymbol symbol)
    {
        switch (symbol)
        {
            case IFieldSymbol field:
                return new FieldSig(field);
            case IPropertySymbol property:
                return new PropertySig(property);
            //case IEventSymbol @event:
                //return new EventSig(@event);
            case IMethodSymbol method:
                return new MethodSig(method);
            default:
                throw new NotImplementedException();
        }
    }

    public static MemberSig Create(MemberInfo member)
    {
        switch (member)
        {
            case FieldInfo field:
                return new FieldSig(field);
            case PropertyInfo property:
                return new PropertySig(property);
            //case EventInfo @event:
                //return new EventSig(@event);
            case MethodBase method:
                return new MethodSig(method);
            default:
                throw new NotImplementedException();
        }
    }



    public IReadOnlyList<AttributeSig> Attributes { get; set; } = Array.Empty<AttributeSig>();
    public MemberTypes MemberType { get; set; }

    public Visibility Visibility { get; set; } = default;
    public Instic Instic { get; set; } = default;
    public Keywords Keywords { get; set; } = default;

    public string Name { get; set; } = null!;
    public string FullName { get; set; } = null!;

    protected MemberSig(ISymbol symbol)
    {
        Attributes = symbol.GetAttributes().Select(static attr => new AttributeSig(attr)).ToList();
        // MemberType set by implementing class
        Visibility = symbol.DeclaredAccessibility.ToVisibility();
        Instic = symbol.IsStatic ? Instic.Static : Instic.Instance;
        Keywords = KeywordUtil.FromSymbol(symbol);
    }
    protected MemberSig(MemberInfo member)
    {
        Attributes = member.GetCustomAttributesData().Select(static attrData => new AttributeSig(attrData)).ToList();
        Visibility = member.GetVisibility();
        Instic = member.IsStatic() ? Instic.Static : Instic.Instance;
        Keywords = KeywordUtil.FromMember(member);
    }
    protected MemberSig()
    {

    }


    public bool HasAttribute<TAttr>()
        where TAttr : Attribute
    {
        return Attributes.Any(attr => string.Equals(attr.FullName, typeof(TAttr).FullName));
    }

    public virtual bool Equals(MemberSig? memberSig)
    {
        return memberSig is not null &&
            memberSig.MemberType == MemberType &&
            memberSig.FullName == FullName;
    }

    public virtual bool Equals(ISymbol? symbol)
    {
        return symbol is not null &&
            symbol.GetMemberType() == MemberType &&
            symbol.GetFullName() == FullName;
    }

    public virtual bool Equals(MemberInfo? member)
    {
        return member is not null &&
            member.MemberType == MemberType &&
            $"{member.ReflectedType.FullName}.{member.Name}" == FullName;
    }

    public override bool Equals(object? obj)
    {
        if (obj is MemberSig memberSig) return Equals(memberSig);
        if (obj is ISymbol symbol) return Equals(symbol);
        if (obj is MemberInfo member) return Equals(member);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(MemberType, FullName);
    }

    public override string ToString()
    {
        return $"""
            [{string.Join(", ", Attributes)}]
            {Visibility} {Instic} {Keywords} {Name}
            """;
    }
}
