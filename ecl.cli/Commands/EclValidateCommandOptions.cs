namespace ecl.cli.Commands;

internal class EclValidateCommandOptions
{
    [CommandLine.Option('i', "input", Required = true, HelpText = "Input ECL files to validate. (Glob patterns are supported)")]
    public IEnumerable<string> InputFiles { get; set; }
}