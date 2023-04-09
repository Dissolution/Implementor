namespace IMPL.SourceGen.MemberWriters;

public interface IPropertySigWriter
{
    void Write(PropertySig propertySig, CodeBuilder codeBuilder);
}
