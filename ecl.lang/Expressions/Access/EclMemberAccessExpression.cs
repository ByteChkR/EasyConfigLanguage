using System;
using System.CodeDom.Compiler;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal class EclMemberAccessExpression : EclLValueExpression
{
    public EclMemberAccessExpression(EclSourceSpan position, string name, EclExpression left) : base(position)
    {
        Name = name;
        Left = left;
    }

    public string Name { get; }
    public EclExpression Left { get; }
    public override EclToken Execute(EclInterpreterContext context)
    {
        return ToReference(context);
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclMember: ");
        Left.GetDebugString(writer);
        writer.Write(".");
        writer.Write(Name);
    }

    public override EclTokenReference ToReference(EclInterpreterContext context)
    {
        var leftValue = Left.Execute(context).Dereference(new EclObject());
        if (leftValue is EclObject obj)
        {
            return new EclTokenReference(obj[Name]);
        }
        throw new InvalidOperationException($"Left expression does not evaluate to an EclObject: {leftValue.GetType().Name}");
    }
}