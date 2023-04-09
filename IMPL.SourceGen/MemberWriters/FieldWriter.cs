using Jay.SourceGen.Signatures;

namespace IMPL.SourceGen.MemberWriters;

public sealed class FieldWriter : MemberWriter<FieldSig>
{
    public static FieldWriter Default { get; } = new();

    public override void Write(FieldSig field, CodeBuilder codeBuilder)
    {
        codeBuilder
           .AppendValue(field.Visibility, "lc")
           .AppendIf(field.Instic == Instic.Instance, " ", " static ")
           .AppendKeywords(field.Keywords)
           .Append(field.Name)
           .AppendLine(';');
    }
}
