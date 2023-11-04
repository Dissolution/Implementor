using System.Text;
using Microsoft.CodeAnalysis.Text;
using static Implementor.AttributeSource;

namespace Implementor;

[Generator(LanguageNames.CSharp)]
public sealed class AttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(
                $"{ImplementAttribute.Name}.g.cs",
                SourceText.From(ImplementAttribute.Code, Encoding.UTF8));
        });
    }
}