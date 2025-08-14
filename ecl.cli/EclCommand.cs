using ecl.cli.Commands;

namespace ecl.cli;

internal abstract class EclCommand<TOpts> : EclCommand
{
    protected EclCommand(string name, string description) : base(name, description)
    {
    }

    public abstract void Run(TOpts args);
    public override void Run(string[] args) => Run(CommandLine.Parser.Default.ParseArguments<TOpts>(args).Value);
}