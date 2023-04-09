namespace IMPL.SourceGen.MemberWriters;

public interface IEventSigWriter
{
    void Write(EventSig eventSig, CodeBuilder codeBuilder);
}
