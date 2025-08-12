using System.Collections.Generic;

namespace ecl.lang.Interpreter;

public class EclInterpreterFunctions
{
    private readonly Dictionary<string, EclInterpreterFunction> _functions =
        new Dictionary<string, EclInterpreterFunction>();

    public static EclInterpreterFunctions Default { get; } = new EclInterpreterFunctions()
        .AddFunction<EclLoadFileFunction>("load")
        .AddFunction<EclMergeTokenFunction>("merge");
    public EclInterpreterFunctions AddFunction(string name, EclInterpreterFunction function)
    {
        _functions[name] = function;
        return this;
    }
    public EclInterpreterFunctions AddFunction<T>(string name) where T : EclInterpreterFunction, new()
    {
        return AddFunction(name, new T());
    }
    
    public bool HasFunction(string name)
    {
        return _functions.ContainsKey(name);
    }

    public EclInterpreterFunction GetFunction(string name)
    {
        return _functions.TryGetValue(name, out var function)
            ? function
            : throw new KeyNotFoundException($"Function '{name}' not found in interpreter functions.");
    }
}