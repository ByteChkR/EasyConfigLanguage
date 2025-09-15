using System.Text;
using ecl.mapper.generator.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ecl.mapper.generator;

[Generator]
public class EclMapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(EclMapperStaticCode.RegisterStaticSource);
        
        IncrementalValuesProvider<EclMappedEntity> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            EclMapperStaticCode.ECL_OBJECT_MAP_ATTRIBUTE,
            static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            static (context, _) =>
            {
                INamedTypeSymbol api = (INamedTypeSymbol)context.TargetSymbol;
                EclMapperModelBuilder builder = new EclMapperModelBuilder(context);

                return builder.GenerateModel(api);
            }
        );

        context.RegisterSourceOutput(
            pipeline,
            static (context, model) =>
            {
                foreach (Diagnostic diagnostic in model.Diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }

                EclMapperSourceWriter gen = new EclMapperSourceWriter(context);
                SourceText source = SourceText.From(
                    gen.GenerateSource(model),
                    Encoding.UTF8
                );


                context.AddSource($"{model.ClassName}.g.cs", source);
            }
        );
    }
}