namespace Implementor.SourceGen.Writers;

public interface IPropertySigWriter
{
    void Write(PropertySig propertySig, CodeBuilder codeBuilder);
}
