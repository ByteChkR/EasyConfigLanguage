using ecl.json;
using Newtonsoft.Json.Linq;

namespace ecl.cli.Commands;

internal class JsonToEclCommand : EclCommand<JsonToEclCommandOptions>
{
    public JsonToEclCommand() : base("2ecl", "Converts ECL into Json format.")
    {
    }

    public override void Run(JsonToEclCommandOptions args)
    {
        if (string.IsNullOrEmpty(args.InputFile))
        {
            Logger.Error("Please provide the path to a JSON file to convert to ECL.");
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(args.InputFile);
            var jsonResult = JToken.Parse(jsonContent);
            var eclToken = jsonResult.ToEclToken();

            if (string.IsNullOrEmpty(args.OutputFile))
            {
                Logger.Info(eclToken.ToString());
            }
            else
            {
                File.WriteAllText(args.OutputFile, eclToken.ToString());
                Logger.Info($"ECL output written to {args.OutputFile}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error processing file {args.InputFile}: {ex.Message}");
        }
    }
}