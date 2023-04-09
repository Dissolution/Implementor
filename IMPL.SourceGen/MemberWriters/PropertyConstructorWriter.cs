using Jay.SourceGen.Signatures;

namespace IMPL.SourceGen.MemberWriters;

public class PropertyConstructorWriter : ICodeWriter
{
    public static readonly MemberPos MemberPos = new(Instic.Instance, MemberTypes.Constructor, Visibility.Public);

    public void Write(Implementer implementer, CodeBuilder codeBuilder)
    {
        var spec = implementer.ImplSpec;
        var implType = spec.ImplType;

        // Gather all read-only properties
        var roProps = implementer.GetMembers<PropertySig>()
            .Where(ps => ps.SetMethod is null)
            .ToList();

        codeBuilder.Code($"public {implType}(")
            .Delimit(", ", roProps, (b,p) =>
            {
                b.Append(p.PropertyType).Append(' ').Append(p.Name.ToVariableName());
            }).AppendLine(')')
            .BracketBlock(ctorBlock =>
            {
                // Set properties
                ctorBlock.LineDelimit(roProps, (b,p) =>
                {
                    string thingName;
                    if (implementer.PropertyWriter is BackingPropertySigWriter)
                    {
                        thingName = BackingFieldModifier.GetBackingFieldName(p);
                    }
                    else
                    {
                        thingName = p.Name;
                    }

                    b.Append("this.").Append(thingName).Append(" = ").Append(p.Name.ToVariableName()).Append(';');
                });
            }).NewLine();
    }
}
