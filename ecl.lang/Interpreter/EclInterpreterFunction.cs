using ecl.core.Tokens;

namespace ecl.lang.Interpreter;

public abstract class EclInterpreterFunction
{
    public abstract EclToken Invoke(EclInterpreterContext caller, EclToken[] args);
}