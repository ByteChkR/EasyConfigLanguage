using ecl.json;
using ecl.lang;
using Newtonsoft.Json;

namespace ecl.cli.Commands;

internal class EclToJsonCommand : EclCommand<EclToJsonCommandOptions>
{
    public EclToJsonCommand() : base("2json", "Converts ECL into Json format.")
    {
    }

    public override void Run(EclToJsonCommandOptions args)
    {
        var files = args.InputFiles.ToArray();
        if (files.Length == 0)
        {
            Console.WriteLine("Please provide the path to an ECL file to convert to JSON.");
            return;
        }

        var result = EclLoader.Load(EclSource.FromFiles(EclCliUtils.ExpandPatterns(files).ToArray()));
        var resultJson = result.ToJToken().ToString(args.PrettyPrint ? Formatting.Indented : Formatting.None);
        if (string.IsNullOrEmpty(args.OutputFile))
        {
            Console.WriteLine(resultJson);
        }
        else
        {
            try
            {
                File.WriteAllText(args.OutputFile, resultJson);
                Console.WriteLine($"JSON output written to {args.OutputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file {args.OutputFile}: {ex.Message}");
            }
        }
    }
}