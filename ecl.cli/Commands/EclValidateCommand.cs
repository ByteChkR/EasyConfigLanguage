namespace ecl.cli.Commands;

internal class EclValidateCommand : EclCommand
{
    public EclValidateCommand() : base("validate", "Validate ECL files and display their parsed structure.")
    {
    }

    public override void Run(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide the path to an ECL file to validate.");
            return;
        }

        foreach (var file in EclCliUtils.ExpandPatterns(args))
        {
            Console.WriteLine(EclValidate.GetReport(file));
        }
    }
}