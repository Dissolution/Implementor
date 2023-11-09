namespace Implementor;

internal static class AttributeSource
{
    public const string Namespace = "Implementor";

    public static class ImplementAttribute
    {
        public const string Name = nameof(ImplementAttribute);
        public const string FullyQualifiedName = $"{Namespace}.{Name}";
        public const string Code = $$"""
            #nullable enable
            
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
    
    public static class EntityAttribute
    {
        public const string Name = nameof(EntityAttribute);
        public const string FullyQualifiedName = $"{Namespace}.{Name}";
        public const string Code = $$"""
            #nullable enable
            
            using System;
            
            namespace {{Namespace}};
            
            [AttributeUsage(AttributeTargets.Interface)]
            public sealed class {{Name}} : Attribute
            {
                internal string[]? KeyNames { get; }
                
                public {{Name}}()
                {
                    this.KeyNames = null;
                }
                
                public {{Name}}(string keyName)
                {
                    this.KeyNames = new string[1] { keyName };
                }
                
                public {{Name}}(params string[]? keyNames)
                {
                    if (keyNames is null || keyNames.Length == 0)
                    {
                        this.KeyNames = null;
                    }
                    else
                    {
                        this.KeyNames = keyNames;
                    }
                }
            }
            """;
    }
}


