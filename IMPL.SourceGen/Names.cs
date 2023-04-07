using IMPL.Contracts;

namespace IMPL.SourceGen;

internal static class Names
{
    public static class ImplementAttr
    {
        public static readonly string FullName = typeof(ImplementAttribute).FullName;
        public static readonly string KeywordsPropertyName = nameof(ImplementAttribute.Keywords);
        public static readonly string NamePropertyName = nameof(ImplementAttribute.Name);
    }
    
}
