namespace Implementor.Utilities;

internal static class Naming
{
    public static string GetImplementationName(string interfaceName)
    {
        if (interfaceName.StartsWith("I"))
            return interfaceName.Substring(1);
        else
            return "impl_" + interfaceName;
    }

    public static string GetFieldName(IPropertySymbol property)
    {
        string propertyName = property.Name;
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return $"_field_{Guid.NewGuid():N}";
        }
        Span<char> fieldName = stackalloc char[propertyName.Length + 1];
        fieldName[0] = '_';
        fieldName[1] = char.ToLower(propertyName[0]);
        propertyName.AsSpan(1).CopyTo(fieldName.Slice(2));
        return fieldName.ToString();
    }
}