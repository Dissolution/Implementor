namespace Jay.SourceGen.Signatures;

public sealed class ParameterSig :
    IEquatable<ParameterSig>,
    IEquatable<IParameterSymbol>,
    IEquatable<ParameterInfo>
{
    public static bool operator ==(ParameterSig? left, ParameterSig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(ParameterSig? left, ParameterSig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }

    public string Name { get; set;} = null!;
    public TypeSig Type { get; set;} = null!;

    public bool IsParams { get; set; } = false;
    public bool HasDefault { get; set;} = false;
    public object? Default { get; set;} = null;

    public ParameterSig(IParameterSymbol parameterSymbol)
    {
        Name = parameterSymbol.Name;
        Type = new(parameterSymbol.Type);
        IsParams = parameterSymbol.IsParams;
        HasDefault = parameterSymbol.HasExplicitDefaultValue;
        if (HasDefault)
        {
            Default = parameterSymbol.ExplicitDefaultValue;
        }
        else
        {
            Default = null;
        }
    }

    public ParameterSig(ParameterInfo parameterInfo)
    {
        Name = parameterInfo.Name;
        Type = new(parameterInfo.ParameterType);
        IsParams = parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
        HasDefault = parameterInfo.HasDefaultValue;
        if (HasDefault)
        {
            Default = parameterInfo.DefaultValue;
        }
        else
        {
            Default = null;
        }
    }

    public ParameterSig()
    {

    }

    public bool Equals(ParameterSig? parameterSig)
    {
        return parameterSig is not null &&
           string.Equals(Name, parameterSig.Name) &&
           Type.Equals(parameterSig.Type) &&
           IsParams == parameterSig.IsParams &&
           HasDefault == parameterSig.HasDefault;
    }

    public bool Equals(IParameterSymbol? parameterSymbol)
    {
        return parameterSymbol is not null &&
            string.Equals(Name, parameterSymbol.Name) &&
            Type.Equals(parameterSymbol.Type) &&
            IsParams == parameterSymbol.IsParams &&
            HasDefault == parameterSymbol.HasExplicitDefaultValue;
    }

    public bool Equals(ParameterInfo? parameterInfo)
    {
        return parameterInfo is not null &&
            string.Equals(Name, parameterInfo.Name) &&
            Type.Equals(parameterInfo.ParameterType) &&
            IsParams == (parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null) &&
            HasDefault == parameterInfo.HasDefaultValue;
    }

    public override bool Equals(object? obj)
    {
        if (obj is ParameterSig parameterSig) return Equals(parameterSig);
        if (obj is IParameterSymbol parameterSymbol) return Equals(parameterSymbol);
        if (obj is ParameterInfo parameterInfo) return Equals(parameterInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(Name, Type, IsParams, HasDefault);
    }

    public override string ToString()
    {
        return $"{(IsParams ? "params " : "")}{Type} {Name}{(HasDefault ? " = " : "")}{(HasDefault ? Default : null)}";
    }
}

