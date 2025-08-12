namespace ecl.cli.Commands;

internal abstract class EclCommand
{
    protected EclCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }
    public string Description { get; }
    public abstract void Run(string[] args);
}