using ecl.lang;

namespace ecl.cli.Commands;

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
            Logger.Error("Please provide the path to ECL files to merge.");
            return;
        }

        try
        {
            var eclSources = EclUtils.ExpandPatterns(files).Select(EclSource.FromFile).ToArray();
            var mergedEcl = EclLoader.Load(eclSources);

            if (string.IsNullOrEmpty(args.OutputFile))
            {
                Logger.Info(mergedEcl.GetDebugString());
            }
            else
            {
                File.WriteAllText(args.OutputFile, mergedEcl.GetDebugString());
                Logger.Info($"Merged ECL output written to {args.OutputFile}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error processing files: {ex.Message}");
        }
    }
}