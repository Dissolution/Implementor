﻿namespace Implementor;

internal static class SourceCode
{
    public const string Namespace = nameof(Implementor);

    public static class ImplementAttribute
    {
        public const string Name = nameof(ImplementAttribute);
        public const string FullyQualifiedName = $"{Namespace}.{Name}";

        public const string Code =
            $$"""
              #nullable enable

              using System;

              namespace {{Namespace}};

              public enum EqualityBehavior
              {
                  Reference = 0,
                  Record = 1,
                  Entity = 2,
              }

              [AttributeUsage(AttributeTargets.Interface)]
              public sealed class {{Name}} : Attribute
              {
                  public EqualityBehavior EqualityBehavior { get; set; } = EqualityBehavior.Reference;
                  
                  public {{Name}}()
                  {
                  }
              
                  public {{Name}}(EqualityBehavior equalityBehavior)
                  {
                      this.EqualityBehavior = equalityBehavior;
                  }
              }
              """;
    }

    public static class KeyAttribute
    {
        public const string Name = nameof(KeyAttribute);
        public const string FullyQualifiedName = $"{Namespace}.{Name}";

        public const string Code =
            $$"""
              #nullable enable

              using System;

              namespace {{Namespace}};

              [AttributeUsage(AttributeTargets.Property)]
              public sealed class {{Name}} : Attribute
              {
                  public {{Name}}() { }
              }
              """;
    }
    
    public static class Implementation
    {
        public const string Name = nameof(Implementation);
        public const string FullyQualifiedName = $"{Namespace}.{Name}";

        public const string Code =
            $$"""
              #nullable enable

              using System;

              namespace {{Namespace}};

              public static partial class {{Name}}
              {
              
              }
              """;
    }
    
    public static class EntityAttribute
    {
        public const string Name = nameof(EntityAttribute);
        public const string FullyQualifiedName = $"{Namespace}.{Name}";

        public const string Code =
            $$"""
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