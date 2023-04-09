namespace IMPL.SourceGen.MemberWriters;

public interface IFieldSigWriter
{
    void Write(FieldSig fieldSig, CodeBuilder codeBuilder);
}
