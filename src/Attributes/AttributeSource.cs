namespace Implementor.Attributes;

internal static class AttributeSource
{
    public const string Namespace = "Implementor";

    public static class ImplementAttribute
    {
        public const string Name = nameof(ImplementAttribute);
        public const string FullyQualifiedName = $"{Namespace}.{Name}";
        public const string Code = $$"""
            using System;
            
            namespace {{Namespace}};
            
            [AttributeUsage(AttributeTargets.Interface)]
            public sealed class {{Name}} : Attribute
            {
                public {{Name}}()
                {
                    
                }
            }                     
            """;
    }
}

