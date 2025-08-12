using System.CodeDom.Compiler;
using System.Globalization;
using ecl.json;
using ecl.lang;
using Newtonsoft.Json;

namespace ecl.cli;

internal class EclValidate
{
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
        var result = EclLoader.FromFile(file);
        var sw = new StringWriter(CultureInfo.InvariantCulture);
        using (var writer = new IndentedTextWriter(sw, "|   "))
        {
            writer.WriteLine("Results for " + file);
            writer.Indent++;
            writer.WriteLine("Original:");
            writer.Indent++;
            WriteIndented(writer, File.ReadAllText(file));
            writer.Indent--;
            writer.WriteLine();
            writer.WriteLine("Parsed(ECL):");
            writer.Indent++;
            WriteIndented(writer, result.GetDebugString());
            ;
            writer.Indent--;
            writer.WriteLine();
            writer.WriteLine("Parsed(JSON):");
            writer.Indent++;
            WriteIndented(writer, result.ToJToken().ToString(Formatting.Indented));
            writer.Indent--;
        }

        var str = sw.ToString();
        return str;
    }
    
    
}