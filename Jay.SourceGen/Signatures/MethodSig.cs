using System.Runtime.CompilerServices;

namespace Jay.SourceGen.Signatures;

public sealed class MethodSig : MemberSig, IEquatable<MethodSig>, IEquatable<IMethodSymbol>, IEquatable<MethodBase>
{
    public static bool operator ==(MethodSig? left, MethodSig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(MethodSig? left, MethodSig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }

    public TypeSig ReturnType { get; set;}
    public IReadOnlyList<ParameterSig> Parameters { get; set;}


    public MethodSig(IMethodSymbol methodSymbol)
        : base(methodSymbol)
    {
        MemberType = MemberTypes.Method;
        ReturnType = new(methodSymbol.ReturnType);
        Parameters = methodSymbol.Parameters.Select(static p => new ParameterSig(p)).ToList();
        if (methodSymbol.IsInitOnly)
            this.Keywords |= Keywords.Init;
    }

    public MethodSig(MethodInfo methodInfo)
        : base(methodInfo)
    {
        MemberType = MemberTypes.Method;
        ReturnType = new(methodInfo.ReturnType);
        Parameters = methodInfo.GetParameters().Select(static p => new ParameterSig(p)).ToList();
        
        var retMods = methodInfo.ReturnParameter.GetRequiredCustomModifiers();
        if (retMods.Contains(typeof(IsExternalInit)))
            this.Keywords |= Keywords.Init;
    }
    public MethodSig(ConstructorInfo ctorInfo)
       : base(ctorInfo)
    {
        MemberType = MemberTypes.Method;
        ReturnType = new(ctorInfo.DeclaringType!);
        Parameters = ctorInfo.GetParameters().Select(static p => new ParameterSig(p)).ToList();
    }

    public MethodSig() : base()
    {
        MemberType = MemberTypes.Method;

    }

    public bool Equals(MethodSig? MethodSig)
    {
        return MethodSig is not null &&
            FullName == MethodSig.FullName &&
            ReturnType.Equals(MethodSig.ReturnType) &&
            Parameters.SequenceEqual(MethodSig.Parameters);
    }

    public bool Equals(IMethodSymbol? MethodSymbol)
    {
        return MethodSymbol is not null &&
           FullName == MethodSymbol.GetFullName() &&
           ReturnType.Equals(MethodSymbol.ReturnType) &&
           Parameters.SequenceEqual(MethodSymbol.Parameters.Select(static p => new ParameterSig(p)));
    }

    public bool Equals(MethodBase? method)
    {
        return method is not null &&
           FullName == $"{method.ReflectedType.FullName}.{method.Name}" &&
           ReturnType.Equals(method is MethodInfo methodInfo ? methodInfo.ReturnType : method.DeclaringType!) &&
           Parameters.SequenceEqual(method.GetParameters().Select(static p => new ParameterSig(p)));
    }


    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is MethodSig MethodSig && Equals(MethodSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IMethodSymbol MethodSymbol && Equals(MethodSymbol);
    }

    public override bool Equals(MemberInfo? member)
    {
        return member is MethodBase MethodInfo && Equals(MethodInfo);
    }

    public override bool Equals(object? obj)
    {
        if (obj is MethodSig MethodSig) return Equals(MethodSig);
        if (obj is IMethodSymbol MethodSymbol) return Equals(MethodSymbol);
        if (obj is MethodBase MethodInfo) return Equals(MethodInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(FullName, ReturnType, Parameters);
    }

    public override string ToString()
    {
        return $$"""
            [{{string.Join(", ", Attributes)}}]
            {{Visibility}} {{Instic}} {{Keywords}} {{ReturnType}} {{FullName}}({{{string.Join(", ", Parameters)}})
            """;
    }
}