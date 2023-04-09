namespace IMPL.SourceGen.MemberWriters;


public class BackingFieldModifier : IImplModifier
{
    public static string GetBackingFieldName(PropertySig propertySig)
    {
        string propName = propertySig.Name;
        Span<char> buffer = stackalloc char[propName.Length + 1];
        buffer[0] = '_';
        buffer[1] = char.ToLower(propName[0]);
        TextHelper.CopyTo(propName.AsSpan(1), buffer[2..]);
        return buffer.ToString();
    }

    public void PreRegister(Implementer implementer)
    {
        // What properties already exist?
        var properties = implementer.Members.Select(p => p.Item2).OfType<PropertySig>().ToList();

        foreach (var property in properties)
        {
            FieldSig field = new()
            {
                Name = GetBackingFieldName(property),
                FieldType = property.PropertyType,
                Instic = property.Instic,
                Visibility = Visibility.Private,
                Keywords = default,
            };

            // No setter, field can be readonly
            if (property.SetMethod is null)
            {
                field.Keywords |= Keywords.Readonly;
            }

            // Add this field
            implementer.AddMember(field);
        }

        implementer.PropertyWriter = new BackingPropertySigWriter();
    }
}