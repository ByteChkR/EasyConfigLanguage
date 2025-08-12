using System.CodeDom.Compiler;
using System.IO;
using ecl.core.Tokens;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal abstract class EclExpression
{
    protected EclExpression(EclSourceSpan position)
    {
        Position = position;
    }

    public EclSourceSpan Position { get; }
    
    public abstract EclToken Execute(EclInterpreterContext context);
    public abstract void GetDebugString(IndentedTextWriter writer);
    public override string ToString()
    {
        using var writer = new StringWriter();
        GetDebugString(new IndentedTextWriter(writer));
        writer.Flush();
        return writer.ToString();
    }

}