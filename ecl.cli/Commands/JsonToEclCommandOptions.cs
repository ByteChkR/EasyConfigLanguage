namespace ecl.cli.Commands;

internal class JsonToEclCommandOptions
{
    [CommandLine.Option('i', "input", Required = true, HelpText = "Input Json file to convert to ECL.")]
    public string InputFile { get; set; }
    [CommandLine.Option('o', "output", Required = false, HelpText = "Output ECL file. If not specified, output will be printed to console.")]
    public string OutputFile { get; set; }
}