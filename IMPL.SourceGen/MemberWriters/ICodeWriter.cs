namespace IMPL.SourceGen.MemberWriters;

public interface ICodeWriter
{
    void Write(Implementer implementer, CodeBuilder codeBuilder);
}


public interface IMemberCode
{
    MemberPos Pos { get; }
    void Write(Implementer implementer, CodeBuilder codeBuilder);
}
