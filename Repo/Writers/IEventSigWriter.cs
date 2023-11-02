namespace Implementor.SourceGen.Writers;

public interface IEventSigWriter
{
    void Write(EventSig eventSig, CodeBuilder codeBuilder);
}
