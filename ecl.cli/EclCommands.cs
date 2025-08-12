namespace ecl.cli;

internal class EclCommands
{
    private readonly List<EclCommand> _commands = new();
    
    public EclCommands()
    {
        AddCommand(new EclHelpCommand(this));
    }

    public EclCommands AddCommand(EclCommand command)
    {
        _commands.Add(command);
        return this;
    }

    public void Run(string commandName, string[] args)
    {
        var command = _commands.FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
        if (command == null)
        {
            Console.WriteLine($"Unknown command: {commandName}");
            return;
        }
        command.Run(args);
    }

    public void ShowHelp()
    {
        Console.WriteLine("Available commands:");
        foreach (var command in _commands)
        {
            Console.WriteLine($"\t{command.Name} - {command.Description}");
        }
    }
}