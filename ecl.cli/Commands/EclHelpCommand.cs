namespace ecl.cli.Commands;

internal class EclHelpCommand : EclCommand
{
    private readonly EclCommands _commands;
    public EclHelpCommand(EclCommands commands) : base("help", "Display help information for ECL commands.")
    {
        _commands = commands;
    }

    public override void Run(string[] args)
    {
        _commands.ShowHelp();
    }
}