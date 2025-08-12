using System.CodeDom.Compiler;
using ecl.core.Tokens;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions.Access;

internal class EclIdentifierExpression : EclLValueExpression
{
    public EclIdentifierExpression(EclSourceSpan position, string name) : base(position)
    {
        Name = name;
    }

    public string Name { get; }
    public override EclToken Execute(EclInterpreterContext context)
    {
        return context.TryGetVariable(Name) ?? context.GetLocalVariable(Name);
    }

    public override EclTokenReference ToReference(EclInterpreterContext context)
    {
        return context.GetLocalVariable(Name);
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclIdentifier: ");
        writer.Write(Name);
    }
}