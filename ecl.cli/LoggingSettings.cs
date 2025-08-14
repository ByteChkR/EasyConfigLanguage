namespace ecl.cli;

internal class LoggingSettings
{
    public bool Enabled { get; set; } = true;
    public int MinLevel { get; set; } = 0; // 0 = Debug, 1 = Info, 2 = Warning, 3 = Error
}