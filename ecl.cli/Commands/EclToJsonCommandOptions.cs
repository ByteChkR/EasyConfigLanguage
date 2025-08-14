namespace ecl.cli.Commands;

internal class EclToJsonCommandOptions
{
    [CommandLine.Option('i', "input", Required = true, HelpText = "Input ECL files to convert to JSON. (Glob patterns are supported)")]
    public IEnumerable<string> InputFiles { get; set; }
    [CommandLine.Option('o', "output", Required = false, HelpText = "Output JSON file. If not specified, output will be printed to console.")]
    public string OutputFile { get; set; }

    [CommandLine.Option('p', "pretty", Required = false, Default = false,
        HelpText = "Format JSON output with indentation.")]
    public bool PrettyPrint { get; set; }
}