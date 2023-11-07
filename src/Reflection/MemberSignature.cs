namespace Implementor.Reflection;

public record class MemberSignature : Signature
{
    public TypeSignature? BaseType { get; set; } = null;
}