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
            Logger.Error("Please provide the path to an ECL file to convert to JSON.");
            return;
        }

        var result = EclLoader.Load(EclSource.FromFiles(EclUtils.ExpandPatterns(files).ToArray()));
        var resultJson = result.ToJToken().ToString(args.PrettyPrint ? Formatting.Indented : Formatting.None);
        if (string.IsNullOrEmpty(args.OutputFile))
        {
            Logger.Info(resultJson);
        }
        else
        {
            try
            {
                File.WriteAllText(args.OutputFile, resultJson);
                Logger.Info($"JSON output written to {args.OutputFile}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error writing to file {args.OutputFile}: {ex.Message}");
            }
        }
    }
}