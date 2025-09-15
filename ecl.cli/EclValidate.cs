using System.CodeDom.Compiler;
using System.Globalization;
using ecl.json;
using ecl.lang;
using Newtonsoft.Json;

namespace ecl.cli;

internal class EclValidate
{
    private static EclValidationSettings Settings => Program.Settings.GetToken("Commandline").GetToken("Commands").GetToken("Validate").ToObject<EclValidationSettings>() ?? new EclValidationSettings();
    private static void WriteIndented(IndentedTextWriter tw, string str)
    {
        var lines = str.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            tw.WriteLine(line);
        }
    }

    public static string GetReport(string file)
    {
        var result = EclLoader.Load(EclSource.FromFile(file));
        var sw = new StringWriter(CultureInfo.InvariantCulture);
        using (var writer = new IndentedTextWriter(sw, "|   "))
        {
            writer.WriteLine("Results for " + file);
            writer.Indent++;
            if (Settings.DisplayOriginal)
            {
                writer.WriteLine("Original:");
                writer.Indent++;
                WriteIndented(writer, File.ReadAllText(file));
                writer.Indent--;
                writer.WriteLine();
            }

            if (Settings.DisplayEcl)
            {
                writer.WriteLine("Parsed(ECL):");
                writer.Indent++;
                WriteIndented(writer, result.GetDebugString());
                writer.Indent--;
                writer.WriteLine();
            }
            

            if (Settings.DisplayJson)
            {
                writer.WriteLine("Parsed(JSON):");
                writer.Indent++;
                WriteIndented(writer, result.ToJToken().ToString(Formatting.Indented));
                writer.Indent--;
            }
            
            if (Settings.DisplayYaml)
            {
                writer.WriteLine("Parsed(YAML):");
                writer.Indent++;
                WriteIndented(writer, result.ToJToken().ToYamlString());
                writer.Indent--;
            }
        }

        var str = sw.ToString();
        return str;
    }
    
    
}