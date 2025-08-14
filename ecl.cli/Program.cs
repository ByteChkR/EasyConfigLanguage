using ecl.cli.Commands;
using ecl.json;
using ecl.lang;

namespace ecl.cli;

class Program
{
    private static EclCommands GetCommands()
    {
        return new EclCommands()
            .AddCommand(new EclValidateCommand())
            .AddCommand(new EclToJsonCommand())
            .AddCommand(new JsonToEclCommand())
            .AddCommand(new EclMergeCommand());
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