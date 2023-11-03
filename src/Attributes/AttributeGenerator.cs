using System.Text;
using Microsoft.CodeAnalysis.Text;
using static Implementor.Attributes.AttributeSource;

namespace Implementor.Attributes;

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