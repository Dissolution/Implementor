namespace Jay.SourceGen.Reflection;

public sealed class PropertySig : MemberSig,
    IEquatable<PropertySig>, IEquatable<IPropertySymbol>, IEquatable<PropertyInfo>
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
    public static bool operator ==(PropertySig? left, IPropertySymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(PropertySig? left, IPropertySymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(PropertySig? left, PropertyInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(PropertySig? left, PropertyInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    public TypeSig? PropertyType { get; set; } = null;
    public MethodSig? GetMethod { get; set; } = null;
    public MethodSig? SetMethod { get; set; } = null;
    public bool IsIndexer { get; set; } = false;
    public IReadOnlyList<ParameterSig> Parameters { get; set; } = Array.Empty<ParameterSig>();

    public PropertySig()
        : base(SigType.Property)
    {

    }

    public PropertySig(IPropertySymbol propertySymbol)
        : base(SigType.Property, propertySymbol)
    {
        this.PropertyType = new TypeSig(propertySymbol.Type);
        this.GetMethod = new MethodSig(propertySymbol.GetMethod);
        this.SetMethod = new MethodSig(propertySymbol.SetMethod);
        this.IsIndexer = propertySymbol.IsIndexer;
        this.Parameters = propertySymbol.Parameters.Select(static p => new ParameterSig(p)).ToList();
    }

    public PropertySig(PropertyInfo propertyInfo)
        : base(SigType.Property, propertyInfo)
    {
        this.PropertyType = new TypeSig(propertyInfo.PropertyType);
        this.GetMethod = new MethodSig(propertyInfo.GetMethod);
        this.SetMethod = new MethodSig(propertyInfo.SetMethod);
        this.Parameters = propertyInfo.GetIndexParameters().Select(static p => new ParameterSig(p)).ToList();
        this.IsIndexer = this.Parameters.Count > 0;
    }

    public bool Equals(PropertySig? propertySig)
    {
        return propertySig is not null &&
            this.FullName == propertySig.FullName &&
            this.PropertyType == propertySig.PropertyType &&
            this.GetMethod == propertySig.GetMethod &&
            this.SetMethod == propertySig.SetMethod &&
            this.Parameters.SequenceEqual(propertySig.Parameters);
    }

    public bool Equals(IPropertySymbol? propertySymbol)
    {
        return propertySymbol is not null &&
            this.FullName == propertySymbol.GetFullName() &&
            this.PropertyType == propertySymbol.Type &&
            this.GetMethod == propertySymbol.GetMethod &&
            this.SetMethod == propertySymbol.SetMethod &&
            this.Parameters.SequenceEqual(propertySymbol.Parameters.Select(static p => new ParameterSig(p)));
    }

    public bool Equals(PropertyInfo? propertyInfo)
    {
        return propertyInfo is not null &&
            this.FullName == propertyInfo.GetFullName() &&
            this.PropertyType == propertyInfo.PropertyType &&
            this.GetMethod == propertyInfo.GetMethod &&
            this.SetMethod == propertyInfo.SetMethod &&
            this.Parameters.SequenceEqual(propertyInfo.GetIndexParameters().Select(static p => new ParameterSig(p)));
    }

    public override bool Equals(object obj)
    {
        if (obj is PropertySig propertySig) return Equals(propertySig);
        if (obj is IPropertySymbol propertySymbol) return Equals(propertySymbol);
        if (obj is PropertyInfo propertyInfo) return Equals(propertyInfo);
        return false;
    }

    public override string ToString()
    {
        return $"Property '{Name}'";
    }
}
