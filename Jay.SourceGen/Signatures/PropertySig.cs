using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jay.SourceGen.Signatures;

public sealed class PropertySig : MemberSig, IEquatable<PropertySig>, IEquatable<IPropertySymbol>, IEquatable<PropertyInfo>
{
    public static bool operator ==(PropertySig? left, PropertySig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(PropertySig? left, PropertySig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }

    public bool HasGet { get; } = false;
    public bool HasSet { get; } = false;
    public bool HasInit { get; } = false;

    public TypeSig PropertyType { get; }
    public IReadOnlyList<ParameterSig> Parameters { get; }

    public PropertySig(IPropertySymbol propertySymbol)
        : base(propertySymbol)
    {
        MemberType = MemberTypes.Property;
        PropertyType = new(propertySymbol.Type);
        Parameters = propertySymbol.Parameters.Select(static p => new ParameterSig(p)).ToList();

        // Getter
        var getMethod = propertySymbol.GetMethod;
        HasGet = getMethod is not null;
        // Setter
        var setMethod = propertySymbol.SetMethod;
        if (setMethod is not null)
        {
            if (setMethod.IsInitOnly)
            {
                HasInit = true;
            }
            else
            {
                HasSet = true;
            }
        }
    }

    public PropertySig(PropertyInfo propertyInfo)
        : base(propertyInfo)
    {
        MemberType = MemberTypes.Property;
        PropertyType = new(propertyInfo.PropertyType);
        Parameters = propertyInfo.GetIndexParameters().Select(static p => new ParameterSig(p)).ToList();

        // Getter
        var getMethod = propertyInfo.GetMethod;
        HasGet = getMethod is not null;
        // Setter
        var setMethod = propertyInfo.SetMethod;
        if (setMethod is not null)
        {
            var customMods = setMethod.ReturnParameter.GetRequiredCustomModifiers();
            if (customMods.Contains(typeof(IsExternalInit)))
            {
                HasInit = true;
            }
            else
            {
                HasSet = true;
            }
        }
    }

    public bool Equals(PropertySig? propertySig)
    {
        return propertySig is not null &&
            FullName == propertySig.FullName &&
            PropertyType.Equals(propertySig.PropertyType) &&
            Parameters.SequenceEqual(propertySig.Parameters);
    }

    public bool Equals(IPropertySymbol? propertySymbol)
    {
        return propertySymbol is not null &&
           FullName == propertySymbol.GetFullName() &&
           PropertyType.Equals(propertySymbol.Type) &&
           Parameters.SequenceEqual(propertySymbol.Parameters.Select(static p => new ParameterSig(p)));
    }

    public bool Equals(PropertyInfo? propertyInfo)
    {
        return propertyInfo is not null &&
           FullName == $"{propertyInfo.ReflectedType.FullName}.{propertyInfo.Name}" &&
           PropertyType.Equals(propertyInfo.PropertyType) &&
           Parameters.SequenceEqual(propertyInfo.GetIndexParameters().Select(static p => new ParameterSig(p)));
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is PropertySig propertySig && Equals(propertySig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IPropertySymbol propertySymbol && Equals(propertySymbol);
    }

    public override bool Equals(MemberInfo? member)
    {
        return member is PropertyInfo propertyInfo && Equals(propertyInfo);
    }

    public override bool Equals(object? obj)
    {
        if (obj is PropertySig propertySig) return Equals(propertySig);
        if (obj is IPropertySymbol propertySymbol) return Equals(propertySymbol);
        if (obj is PropertyInfo propertyInfo) return Equals(propertyInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(FullName, PropertyType, Parameters);
    }

    public override string ToString()
    {
        return $$"""
            [{{string.Join(", ", Attributes)}}]
            {{Access}} {{Instic}} {{Keywords}} {{FullName}} {{{(HasGet ? " get;" : "")}}{{(HasSet ? " set;" : HasInit ? " init;" : "")}}}
            """;
    }
}