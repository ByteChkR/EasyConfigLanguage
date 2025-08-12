using ecl.cli.Commands;
using ecl.json;
using ecl.lang;

namespace ecl.cli;

internal abstract class EclCommand<TOpts> : EclCommand
{
    protected EclCommand(string name, string description) : base(name, description)
    {
    }

    public abstract void Run(TOpts args);
    public override void Run(string[] args) => Run(EclLoader.FromSource("command", string.Join(' ', args)).ToObject<TOpts>()!);
}

class Program
{
    private static EclCommands GetCommands()
    {
        return new EclCommands()
            .AddCommand(new EclValidateCommand());
    }
    static void Main(string[] args)
    {
        var commands = GetCommands();
        if(args.Length == 0)
        {
            Console.WriteLine("No command provided. Use 'help' to see available commands.");
            return;
        }
        
        var commandName = args[0];
        var commandArgs = args.Skip(1).ToArray();

        commands.Run(commandName, commandArgs);
    }
}