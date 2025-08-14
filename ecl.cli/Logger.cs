namespace ecl.cli;

internal static class Logger
{
    private static LoggingSettings Settings => Program.Settings.GetToken("Commandline").ToObject<LoggingSettings>("Logging") ?? new LoggingSettings();
    
    public static void Log(string message, int level = 0)
    {
        if (!Settings.Enabled || level < Settings.MinLevel)
        {
            return;
        }
        
        var prefix = level switch
        {
            0 => "[DEBUG] ",
            1 => "[INFO] ",
            2 => "[WARN] ",
            3 => "[ERROR] ",
            _ => "[UNKNOWN] "
        };
        
        Console.WriteLine(prefix + message);
    }
    
    public static void Error(string message)
    {
        Log(message, 3);
    }
    
    public static void Warn(string message)
    {
        Log(message, 2);
    }
    
    public static void Info(string message)
    {
        Log(message, 1);
    }
    
    public static void Debug(string message)
    {
        Log(message, 0);
    }
}