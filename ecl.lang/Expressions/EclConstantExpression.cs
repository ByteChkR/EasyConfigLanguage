using System.CodeDom.Compiler;
using ecl.core.Tokens;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal class EclConstantExpression : EclExpression
{
    private readonly EclToken _token;
    public EclToken Value => _token;

    public EclConstantExpression(EclSourceSpan position, EclToken token) : base(position)
    {
        _token = token;
    }

    public override EclToken Execute(EclInterpreterContext context)
    {
        return _token.Dereference().Clone();
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclConstant: ");
        _token.GetDebugString(writer);
    }
}