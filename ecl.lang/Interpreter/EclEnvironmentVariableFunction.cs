using System;
using ecl.core.Tokens;
using ecl.core.Tokens.Literals;

namespace ecl.lang.Interpreter;

public class EclEnvironmentVariableFunction : EclInterpreterFunction
{
    public override EclToken Invoke(EclInterpreterContext caller, EclToken[] args)
    {
        if(args.Length is < 1 or > 2)throw new ArgumentException("env function requires one or two arguments (variable name, default value).", nameof(args));
        var varName = ((EclString)args[0].Dereference()).Value;
        var varValue = Environment.GetEnvironmentVariable(varName);
        if (string.IsNullOrEmpty(varValue) && args.Length > 1)
        {
            return args[1].Dereference();
        }
        return EclLiteral.CreateString(varValue ?? string.Empty);
    }
}