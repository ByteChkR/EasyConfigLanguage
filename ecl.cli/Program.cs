using System.IO.Compression;
using ecl.cli.Commands;
using ecl.lang;

namespace ecl.cli;

class Program
{
    public static readonly ProgramSettings Settings;

    public static CommandlineSettings CommandlineSettings =>
        Settings.ToObject<CommandlineSettings>("Commandline");

    static Program()
    {
        Settings = LoadSettings();
    }
    private static EclCommands GetCommands()
    {
        Logger.Debug("Creating Commands...");
        return new EclCommands()
            .AddCommand(new EclValidateCommand())
            .AddCommand(new EclToJsonCommand())
            .AddCommand(new JsonToEclCommand())
            .AddCommand(new EclMergeCommand());
    }

    private static ProgramSettings LoadSettings()
    {
        var file = "data/config/Config.ecl";
        if (!File.Exists(file))
        {
            File.WriteAllText(file, "{}");
        }
        var eclSource = EclSource.FromFile(file);
        var ecl = EclLoader.Load(eclSource);
        return new ProgramSettings(ecl);
    }
    
    static void Main(string[] args)
    {
        if(CommandlineSettings.DisplayVersionInfo)
        {
            Logger.Info($"ECL CLI Version: {typeof(Program).Assembly.GetName().Version}");
        }

        var commands = GetCommands();
        if(args.Length == 0)
        {
            Logger.Error("No command provided. Use 'help' to see available commands.");
            return;
        }
        
        var commandName = args[0];
        var commandArgs = args.Skip(1).ToArray();

        commands.Run(commandName, commandArgs);
    }
}