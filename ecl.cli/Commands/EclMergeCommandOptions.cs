namespace ecl.cli.Commands;

internal class EclMergeCommandOptions
{
    [CommandLine.Option('i', "input", Required = true, HelpText = "Input ECL files to convert to JSON. (Glob patterns are supported)")]
    public IEnumerable<string> InputFiles { get; set; }
    [CommandLine.Option('o', "output", Required = false, HelpText = "Output JSON file. If not specified, output will be printed to console.")]
    public string OutputFile { get; set; }
}