using System.Collections.Generic;
using System.Linq;
using ecl.mapper.generator.Model;
using Microsoft.CodeAnalysis;

namespace ecl.mapper.generator;

public class EclMapperModelBuilder
{
    private readonly GeneratorAttributeSyntaxContext _context;

    public EclMapperModelBuilder(GeneratorAttributeSyntaxContext context)
    {
        _context = context;
    }

    public EclMappedEntity GenerateModel(INamedTypeSymbol symbol)
    {
        List<Diagnostic> diagnostics = new List<Diagnostic>();
        if (symbol.Implements(EclMapperStaticCode.ECL_OBJECT_MAP))
        {
            diagnostics.Add(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "ECL001",
                        "Type must not explicitly implement EclObjectMap",
                        "Type {0} must not explicitly implement EclObjectMap",
                        "EclMapper",
                        DiagnosticSeverity.Error,
                        true
                    ),
                    symbol.Locations.First()
                )
            );
        }
        
        IEnumerable<EclMappedProperty> properties = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(
                x =>
                    x is { IsReadOnly: false, IsStatic: false, IsAbstract: false, DeclaredAccessibility: Accessibility.Public, IsPartialDefinition: true }
            )
            .Select(
                x => new EclMappedProperty(
                    x.Name,
                    x.Type.ToDisplayString(),
                    x.Type.Implements(EclMapperStaticCode.ECL_OBJECT_MAP)
                )
            );

        return new EclMappedEntity(
            symbol.ContainingNamespace?.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle
                    .Omitted))!,
            symbol.Name, 
            properties.ToArray(),
            diagnostics.ToArray());
    }
}