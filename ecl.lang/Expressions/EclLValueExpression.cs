using ecl.core.Tokens;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal abstract class EclLValueExpression : EclExpression
{
    protected EclLValueExpression(EclSourceSpan position) : base(position)
    {
    }

    public abstract EclTokenReference ToReference(EclInterpreterContext context);
}