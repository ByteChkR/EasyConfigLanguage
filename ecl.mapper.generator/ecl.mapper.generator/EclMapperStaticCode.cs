using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ecl.mapper.generator;

public class EclMapperStaticCode
{
    public const string ECL_OBJECT_MAP = "ecl.mapper.EclObjectMap";
    public const string ECL_OBJECT_MAP_ATTRIBUTE = "ecl.mapper.EclMappedAttribute";

    private const string STATIC_CODE = """
                                       #pragma warning disable 1591
                                       namespace ecl.mapper
                                       {

                                           internal class EclMappedAttribute : System.Attribute
                                           {
                                               public EclMappedAttribute()
                                               {
                                               }
                                           }

                                       }
                                       """;
    
    public static void RegisterStaticSource(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("EclMapper.g.cs", SourceText.From(STATIC_CODE, Encoding.UTF8));
    }
}