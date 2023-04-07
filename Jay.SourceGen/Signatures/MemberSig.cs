using System.Reflection;

namespace Jay.SourceGen.Signatures;

public abstract class MemberSig :
    IEquatable<MemberSig>, IEquatable<ISymbol>, IEquatable<MemberInfo>
{
    public IReadOnlyList<AttributeSig> Attributes { get; init; } = Array.Empty<AttributeSig>();
    public required MemberTypes MemberType { get; init; }

    public Accessibility Access { get; init; } = default;
    public Instic Instic { get; init; } = default;
    public Keywords Keywords { get; init; } = default;

    public required string Name { get; init; }
    public required string FullName { get; init; }

    protected MemberSig(ISymbol symbol)
    {
        Attributes = symbol.GetAttributes().Select(static attr => new AttributeSig(attr)).ToList();
        // MemberType set by implementing class
        Access = symbol.DeclaredAccessibility;
        Instic = symbol.IsStatic ? Instic.Static : Instic.Instance;

    }
    protected MemberSig(MemberInfo member)
    {
        Attributes = member.GetCustomAttributesData().Select(static attrData => new AttributeSig(attrData)).ToList();
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
            {Access} {Instic} {Keywords} {Name}
            """;
    }
}
