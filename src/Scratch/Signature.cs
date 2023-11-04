using Implementor.Reflection;

namespace Implementor.Scratch;

public abstract record class Signature
{
    public string? Name { get; set; } = null;
    public Visibility Visibility { get; set; } = Visibility.None;
    public Keywords Keywords { get; set; } = new();
    public Attributes Attributes { get; set; } = new();
}