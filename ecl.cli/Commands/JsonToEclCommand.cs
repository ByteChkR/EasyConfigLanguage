using ecl.json;
using ecl.lang;
using Newtonsoft.Json.Linq;

namespace ecl.cli.Commands;

internal class EclMergeCommandOptions
{
    [CommandLine.Option('i', "input", Required = true, HelpText = "Input ECL files to convert to JSON. (Glob patterns are supported)")]
    public IEnumerable<string> InputFiles { get; set; }
    [CommandLine.Option('o', "output", Required = false, HelpText = "Output JSON file. If not specified, output will be printed to console.")]
    public string OutputFile { get; set; }
}
internal class EclMergeCommand : EclCommand<EclMergeCommandOptions>
{
    public EclMergeCommand() : base("merge", "Merges multiple ECL files into a single ECL file.")
    {
    }


    public override void Run(EclMergeCommandOptions args)
    {
        var files = args.InputFiles.ToArray();
        if (files.Length == 0)
        {
            Console.WriteLine("Please provide the path to ECL files to merge.");
            return;
        }

        try
        {
            var eclSources = EclCliUtils.ExpandPatterns(files).Select(EclSource.FromFile).ToArray();
            var mergedEcl = EclLoader.Load(eclSources);

            if (string.IsNullOrEmpty(args.OutputFile))
            {
                Console.WriteLine(mergedEcl.GetDebugString());
            }
            else
            {
                File.WriteAllText(args.OutputFile, mergedEcl.GetDebugString());
                Console.WriteLine($"Merged ECL output written to {args.OutputFile}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing files: {ex.Message}");
        }
    }
}

internal class JsonToEclCommand : EclCommand<JsonToEclCommandOptions>
{
    public JsonToEclCommand() : base("2ecl", "Converts ECL into Json format.")
    {
    }

    public override void Run(JsonToEclCommandOptions args)
    {
        if (string.IsNullOrEmpty(args.InputFile))
        {
            Console.WriteLine("Please provide the path to a JSON file to convert to ECL.");
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(args.InputFile);
            var jsonResult = JToken.Parse(jsonContent);
            var eclToken = jsonResult.ToEclToken();

            if (string.IsNullOrEmpty(args.OutputFile))
            {
                Console.WriteLine(eclToken);
            }
            else
            {
                File.WriteAllText(args.OutputFile, eclToken.ToString());
                Console.WriteLine($"ECL output written to {args.OutputFile}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {args.InputFile}: {ex.Message}");
        }
    }
}