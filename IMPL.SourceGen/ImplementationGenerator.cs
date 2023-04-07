using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

using Jay.SourceGen.Extensions;
using System.Data;
using System.Reflection;

namespace IMPL.SourceGen;

[Generator]
public sealed class ImplementationGenerator : IIncrementalGenerator
{
    [Conditional("DEBUG")]
    private static void Log(FormattableString message)
    {
        string? msg;
        if (message.ArgumentCount == 0)
        {
            msg = message.Format;
        }
        else
        {
            var args = message.GetArguments();
            for (var i = 0; i < args.Length; i++)
            {
                object? arg = args[i];
                Type? argType = arg?.GetType();
                // Process???
                // args[i] = arg;
            }
            msg = string.Format(message.Format, args);
        }
        Debug.WriteLine($"{DateTime.Now:t}: {msg}");
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Initial filter for things with our attribute
        var typeDeclarations = context.SyntaxProvider
             .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: Names.ImplementAttr.FullName,
                predicate: static (syntaxNode, _) => syntaxNode is TypeDeclarationSyntax,
                transform: static (ctx, _) => (TypeDeclarationSyntax)ctx.TargetNode)
             .Where(t => t is not null)!;

        // Combine with Compilation
        var compAndDecl = context.CompilationProvider
            .Combine(typeDeclarations.Collect());

        // Send for further processing
        context.RegisterSourceOutput(compAndDecl,
            static (sourceContext, compAndDecls) => Process(compAndDecls.Left, sourceContext, compAndDecls.Right));
    }

    private static void Process(Compilation compilation,
        SourceProductionContext sourceProductionContext,
        ImmutableArray<TypeDeclarationSyntax> typeDeclarations)
    {
        // If we have nothing to process, exit quickly
        if (typeDeclarations.IsDefaultOrEmpty) return;

#if ATTACH
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
            Log($"Debugger attached");
        }
#endif

        // Get a passable CancellationToken
        var token = sourceProductionContext.CancellationToken;

        // Load our attribute's symbol
        INamedTypeSymbol? attributeSymbol = compilation
            .GetTypesByMetadataName(Names.ImplementAttr.FullName)
            .FirstOrDefault();
        if (attributeSymbol is null)
        {
            // Cannot!
            throw new InvalidOperationException($"Could not load {nameof(INamedTypeSymbol)} for {Names.ImplementAttr.FullName}");
        }

        // As per several examples, we need a distinct list or a grouping on SyntaxTree
        // I'm going with System.Text.Json's example

        foreach (var group in typeDeclarations.GroupBy(static td => td.SyntaxTree))
        {
            SyntaxTree syntaxTree = group.Key;
            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
            CompilationUnitSyntax unitSyntax = (syntaxTree.GetRoot(token) as CompilationUnitSyntax)!;

            // Now, process each thing our attribute is on
            foreach (TypeDeclarationSyntax typeDeclaration in typeDeclarations)
            {
                // Type's Symbol
                INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
                if (typeSymbol is null)
                {
                    throw new InvalidOperationException($"Could not get Declared {nameof(INamedTypeSymbol)} for {nameof(TypeDeclarationSyntax)}");
                }

                // ImplementAttribute
                AttributeData? implementAttributeData = typeSymbol
                    .GetAttributes()
                    .FirstOrDefault(attr => string.Equals(attr.AttributeClass?.GetFullName(), Names.ImplementAttr.FullName));

                if (implementAttributeData is null)
                {
                    throw new InvalidOperationException($"Could not get find our {nameof(ImplementAttribute)}");
                }

                HashSet<INamedTypeSymbol> interfaces = new(SymbolEqualityComparer.Default);
                if (typeDeclaration is InterfaceDeclarationSyntax)
                    interfaces.Add(typeSymbol);

                HashSet<ISymbol> members = new(SymbolEqualityComparer.Default);
                typeSymbol
                    .GetMembers()
                    .Consume(m => members.Add(m));
                typeSymbol
                    .AllInterfaces
                    .SelectMany(interfaceSymbol =>
                    {
                        interfaces.Add(interfaceSymbol);
                        return interfaceSymbol.GetMembers();
                    })
                    .Consume(m => members.Add(m));

                Debugger.Break();

            }
        }

    }

}

/*
public sealed class GenerateInfo
{
    public INamedTypeSymbol InterfaceTypeSymbol { get; }
    
    public required string ImplementationTypeName { get; init; }


    public Visibility Visibility { get; set; } = Visibility.Public;
    public MemberKeywords MemberKeywords { get; set; } = MemberKeywords.Sealed;
    public ObjType ObjType { get; set; } = ObjType.Class;

    public ImmutableArray<INamedTypeSymbol> Interfaces { get; set; }
    public HashSet<MemberSig> Members { get; internal set;} = new();

    public GenerateInfo(INamedTypeSymbol interfaceTypeSymbol)
    {
        this.InterfaceTypeSymbol = interfaceTypeSymbol;
    }

    public void GetLocals(
        out string implementationTypeName, 
        out string implementationVariableName)
    {
        implementationTypeName = this.ImplementationTypeName;
        implementationVariableName = this.ImplementationTypeName.ToVariableName();
    }

    public bool HasInterface<TInterface>()
        where TInterface : class
    {
        return this.Interfaces.Any(isym => isym.GetFQN() == typeof(TInterface).FullName);
    }

    public bool HasMember(
        Instic instic,
        Visibility visibility,
        MemberType memberType,
        string? name = null,
        Func<TypeSig, bool>? returnType = null,
        Func<IReadOnlyList<ParameterSig>, bool>? paramTypes = null)
    {
        foreach (var member in Members)
        {
            if (!member.Instic.HasFlag(instic)) continue;
            if (!member.Visibility.HasFlag(visibility)) continue;
            if (!member.MemberType.HasFlag(memberType)) continue;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!string.Equals(name, member.Name))
                    continue;
            }
            if (returnType != null)
            {
                if (!returnType(member.ReturnType)) continue;
            }
            if (paramTypes != null)
            {
                if (!paramTypes(member.ParamTypes)) continue;
            }
            return true;
        }
        return false;
    }

    public IEnumerable<MemberSig> MembersWithAttribute(string attributeFQN)
    {
        return this.Members.Where(m => m.Attributes.Any(attr => attr.AttributeClass?.GetFQN() == attributeFQN));
    }
}
*/