using ecl.lang;

namespace ecl.cli.Commands;

internal class EclValidateCommand : EclCommand<EclValidateCommandOptions>
{
    public EclValidateCommand() : base("validate", "Validate ECL files and display their parsed structure.")
    {
    }

    public override void Run(EclValidateCommandOptions args)
    {
        var files = args.InputFiles.ToArray();
        if (files.Length == 0)
        {
            Logger.Error("Please provide the path to an ECL file to validate.");
            return;
        }

        foreach (var file in EclUtils.ExpandPatterns(files))
        {
            Logger.Info(EclValidate.GetReport(file));
        }
    }
}