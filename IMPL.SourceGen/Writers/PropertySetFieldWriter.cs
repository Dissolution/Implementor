namespace IMPL.SourceGen.MemberWriters;

public class PropertySetFieldWriter : IPropertySigWriter
{
    public void Write(PropertySig propertySig, CodeBuilder codeBuilder)
    {
        codeBuilder
            .AppendValue(propertySig.Visibility, "lc")
            .AppendIf(propertySig.Instic == Instic.Instance, " ", " static ")
            .AppendKeywords(propertySig.Keywords)
            .Append(propertySig.Name).Append(" {")
            .AppendIf(propertySig.GetMethod is not null, " get;")
            .If(propertySig.SetMethod is not null, setBlock =>
            {
                if (propertySig.SetMethod!.Keywords.HasFlag(Keywords.Init))
                    setBlock.Append(" init;");
                else
                    setBlock.Append(" set;");
            })
           .Append(" }").NewLine();
    }
}
