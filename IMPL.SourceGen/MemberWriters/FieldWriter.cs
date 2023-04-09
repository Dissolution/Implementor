//using Jay.SourceGen.Signatures;

//namespace IMPL.SourceGen.MemberWriters;

//public sealed class FieldWriter : MemberSigWriter<FieldSig>
//{
//    public FieldWriter(FieldSig fieldSig)
//        : base(fieldSig)
//    {

//    }

//    public override void Write(CodeBuilder codeBuilder)
//    {
//        codeBuilder
//           .AppendValue(Member.Visibility, "lc")
//           .AppendIf(Member.Instic == Instic.Instance, " ", " static ")
//           .AppendKeywords(Member.Keywords)
//           .Append(Member.Name)
//           .AppendLine(';');
//    }
//}
