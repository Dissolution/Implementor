using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Implementor.SourceGen;

[Generator(LanguageNames.CSharp)]
public sealed class ImplementationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            const string code = """
                                namespace Woah;
                                public static class Hello
                                {
                                    public static void World()
                                    {
                                        Console.WriteLine("Hello, World!");
                                    }
                                }
                                """;
            ctx.AddSource("ExampleGenerator.g", SourceText.From(code, Encoding.UTF8));
        });

#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
            Debug.WriteLine($"Debugger Attached");
        }

        Debugger.Break();
#endif

        // Initial filter for things with our attribute
        var typeDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: ContractNames.ImplementAttribute.FullName,
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

    private static void Process(
        Compilation compilation,
        SourceProductionContext sourceProductionContext,
        ImmutableArray<TypeDeclarationSyntax> typeDeclarations)
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
            Debug.WriteLine($"Debugger Attached");
        }

        Debugger.Break();

        Jay.Debugging.Hold.Onto(147);
#endif

        // If we have nothing to process, exit quickly
        if (typeDeclarations.IsDefaultOrEmpty) return;

        // Get a passable CancellationToken
        var token = sourceProductionContext.CancellationToken;


        // Load our attribute's symbol
        INamedTypeSymbol? attributeSymbol = compilation
            .GetTypesByMetadataName(ContractNames.ImplementAttribute.FullName)
            .FirstOrDefault();
        if (attributeSymbol is null)
        {
            // Cannot!
            throw new InvalidOperationException($"Could not load {nameof(INamedTypeSymbol)} for {ContractNames.ImplementAttribute.FullName}");
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
                    .FirstOrDefault(attr => string.Equals(attr.AttributeClass?.Name, ContractNames.ImplementAttribute.FullName));

                if (implementAttributeData is null)
                {
                    throw new InvalidOperationException($"Could not get find our {ContractNames.ImplementAttribute.FullName}");
                }

                Debugger.Break();


                // Add it to the source output
                //sourceProductionContext.AddSource(sourceCode.FileName, sourceCode.Code);
            }
        }
    }
}