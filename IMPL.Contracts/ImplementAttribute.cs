namespace IMPL.Contracts;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class)]
public class ImplementAttribute : Attribute
{
    /// <summary>
    /// Specifies declaration keywords to be added to the implementation
    /// </summary>
    public string? Declaration { get; init; } = null;

    /// <summary>
    /// Specifies an override name for the implementation (if applied to an <c>interface</c>)
    /// </summary>
    public string? Name { get; init; } = null;

    public ImplementAttribute() { }
}
