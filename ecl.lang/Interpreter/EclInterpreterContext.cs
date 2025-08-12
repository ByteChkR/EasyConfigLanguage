using System;
using System.Linq;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.lang.Expressions;

namespace ecl.lang.Interpreter;

public class EclInterpreterContext
{
    public EclInterpreterFunctions Functions { get; }
    public EclInterpreterContext(EclInterpreterFunctions functions) : this(functions, new EclObject())
    {
    }
    public EclInterpreterContext(EclInterpreterFunctions functions, EclContainer current)
    {
        Functions = functions ?? throw new ArgumentNullException(nameof(functions), "Functions cannot be null.");
        Current = current ?? throw new ArgumentNullException(nameof(current), "Current container cannot be null.");
    }

    private EclInterpreterContext(EclInterpreterContext parent, EclContainer current) : this(parent.Functions, current)
    {
        Parent = parent;
    }

    public EclContainer Current { get; }
    public EclInterpreterContext? Parent { get; }

    public EclToken? TryGetVariable(string name)
    {
        EclInterpreterContext? current = this;
        while (current != null)
        {
            if (current.Current is EclObject obj)
            {
                //Check if property exists in the current object
                if (obj.Properties.Any(x => x.Key == name)) return new EclTokenReference(obj[name]);
            }
            current = current.Parent;
        }

        return null;
    }

    public EclTokenReference GetLocalVariable(string name)
    {
        if(Current is not EclObject obj) throw new InvalidOperationException("Current context is not an EclObject.");
        return new EclTokenReference(obj[name]);
    }

    public EclInterpreterContext CreateChild(EclContainer current)
    {
        return new EclInterpreterContext(this, current);
    }
}