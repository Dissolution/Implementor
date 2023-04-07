using Jay.SourceGen.Coding;


namespace IMPL.SourceGen.MemberWriters;

public interface IMemberWriter
{
    bool CanWriteMember(ISymbol memberSymbol);
    void WriteMember(ISymbol member, CodeBuilder code);
}
