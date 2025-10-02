using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using ecl.mapper.generator.Model;
using Microsoft.CodeAnalysis;

namespace ecl.mapper.generator;

public class EclMapperSourceWriter
{
    private readonly SourceProductionContext _context;

    public EclMapperSourceWriter(SourceProductionContext context)
    {
        _context = context;
    }

    public string GenerateSource(EclMappedEntity model)
    {
        IndentedTextWriter tw = new IndentedTextWriter(new StringWriter());


        tw.WriteLine("#nullable enable");
        tw.WriteLine("#pragma warning disable 1591");
        tw.WriteLine();
        tw.WriteLine("using Newtonsoft.Json.Linq;");
        tw.WriteLine("using ecl.merge;");
        tw.WriteLine("using ecl.json;");
        tw.WriteLine();
        tw.WriteLine($"namespace {model.Namespace}");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine($"public partial class {model.ClassName} : ecl.mapper.EclObjectMap<{model.ClassName}>");
        tw.WriteLine("{");
        tw.Indent++;
        
        List<EclMappedProperty> backingFields = new List<EclMappedProperty>();

        foreach (var property in model.Properties)
        {
            //Implement backing field and property with get/set
            /*
             Example for NON-nested property
                get => Token["<name>"].GetValue(null).ToJToken().ToObject<<type>>() ?? <default>;
                set => Token["<name>"].SetValue(JToken.FromObject(value).ToEclToken());
            
             Example for nested property
                 get => <backingField>;
                 set
                 {
                     // Merge the existing one and the new one
                     EclMerge.MergeInplace(<backingField>.Token, value.Token);
                     Token["<name>"].SetValue(<backingField>.Token);
                 }
            */
            
            if (property.IsNested)
            {
                tw.WriteLine($"private {property.PropertyType} _{property.PropertyName.ToLowerInvariant()} = new {property.PropertyType}();");
                tw.WriteLine($"public partial {property.PropertyType} {property.PropertyName}");
                tw.WriteLine("{");
                tw.Indent++;
                tw.WriteLine($"get => _{property.PropertyName.ToLowerInvariant()};");
                tw.WriteLine("set");
                tw.WriteLine("{");
                tw.Indent++;
                tw.WriteLine($"// Merge the existing one and the new one");
                tw.WriteLine($"EclMerge.MergeInplace(_{property.PropertyName.ToLowerInvariant()}.Token, value.Token);");
                tw.Indent--;
                tw.WriteLine("}");
                tw.Indent--;
                tw.WriteLine("}");
                backingFields.Add(property);
            }
            else
            {
                string defaultValue = property.PropertyType switch
                {
                    "string" => "ecl.core.Tokens.EclLiteral.CreateString(string.Empty)",
                    "int" or "long" or "float" or "double" or "decimal" => $"ecl.core.Tokens.EclLiteral.CreateNumber(default({property.PropertyType}))",
                    "bool" => "ecl.core.Tokens.EclLiteral.False",
                    _ => "ecl.core.Tokens.EclLiteral.Null"
                };
                
                tw.WriteLine($"public partial {property.PropertyType} {property.PropertyName}");
                tw.WriteLine("{");
                tw.Indent++;
                tw.WriteLine($"get => Token[\"{property.PropertyName}\"].GetValue({defaultValue}).ToJToken().ToObject<{property.PropertyType}>();");
                tw.WriteLine($"set => Token[\"{property.PropertyName}\"].SetValue(value != null ? JToken.FromObject(value).ToEclToken() : {defaultValue});");
                tw.Indent--;
                tw.WriteLine("}");
            }
            tw.WriteLine();
        }
        
        tw.WriteLine($"public {model.ClassName}()");
        tw.WriteLine("{");
        tw.Indent++;
        foreach (var field in backingFields)
        {
            tw.WriteLine($"Token[\"{field.PropertyName}\"].SetValue(_{field.PropertyName.ToLowerInvariant()}.Token);");
            tw.WriteLine($"_{field.PropertyName.ToLowerInvariant()}.Token.OnChanged += (_, _) => RaiseChanged();");
        }
        
        tw.Indent--;
        tw.WriteLine("}");
        tw.Indent--;
        tw.WriteLine("}");
        tw.Indent--;
        tw.WriteLine("}");
        
        return tw.InnerWriter.ToString();
    }
}