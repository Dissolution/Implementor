﻿using System.Diagnostics;
using Implementor.Text;
using Implementor.Utilities;

namespace Implementor;

public sealed record class InterfaceImplementation(
    string Namespace,
    string InterfaceName,
    string ClassName,
    string Code,
    Type[] CtorParamTypes);

public class ImplementationBuilder
{
    public TypeDeclarationSyntax TypeDeclaration { get; }
    public INamedTypeSymbol TypeSymbol { get; }
    public AttributeData ImplementAttributeData { get; }

    public ImplementationBuilder(
        TypeDeclarationSyntax typeDeclaration, 
        INamedTypeSymbol typeSymbol, 
        AttributeData implementAttributeData)
    {
        TypeDeclaration = typeDeclaration;
        TypeSymbol = typeSymbol;
        ImplementAttributeData = implementAttributeData;
    }


    public InterfaceImplementation GetSourceFile()
    {
        // What interfaces do we implement?
        var interfaces = this.TypeSymbol.AllInterfaces;

        if (!interfaces.IsDefaultOrEmpty)
        {
            // We'll sub-process each
            Debugger.Break();
        }

        var properties = this.TypeSymbol.GetMembers().OfType<IPropertySymbol>().ToList();
        if (properties.Count > 0)
        {
            Debugger.Break();
        }

        string @namespace = TypeSymbol.GetFqNamespace();
        string interfaceName = TypeSymbol.Name;
        string className = Naming.GetImplementationName(interfaceName);
        

        using var fileBuilder = new CSharpFileBuilder();
        fileBuilder.AutoGeneratedHeader()
            .Nullable(true)
            .Namespace(@namespace)
            .Code
            .Append($$"""
                      public class {{className}} : {{interfaceName}}
                      {
                          public override string ToString()
                          {
                              return "Implementation of {{@namespace}}.{{interfaceName}}";
                          }
                      }
                      """);

        string code = fileBuilder.GetSourceCode();

        return new(@namespace, interfaceName, className, code, Array.Empty<Type>());
    }
}