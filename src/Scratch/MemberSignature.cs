namespace Implementor.Scratch;

public record class MemberSignature : Signature
{
    public TypeSignature? BaseType { get; set; } = null;
}