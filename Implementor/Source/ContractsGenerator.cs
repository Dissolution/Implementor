 using System.Text;
 using Microsoft.CodeAnalysis.Text;
 using static Implementor.SourceCode;

 namespace Implementor;

 [Generator(LanguageNames.CSharp)]
 public sealed class ContractsGenerator : IIncrementalGenerator
 {
     public void Initialize(IncrementalGeneratorInitializationContext context)
     {
         context.RegisterPostInitializationOutput(static ctx =>
         {
             ctx.AddSource(
                 $"{ImplementAttribute.Name}.g.cs",
                 SourceText.From(ImplementAttribute.Code, Encoding.UTF8));
             ctx.AddSource(
                 $"{KeyAttribute.Name}.g.cs",
                 SourceText.From(KeyAttribute.Code, Encoding.UTF8));
             ctx.AddSource(
                 $"{Implementation.Name}.core.g.cs",
                 SourceText.From(Implementation.Code, Encoding.UTF8));
         });
     }
 }