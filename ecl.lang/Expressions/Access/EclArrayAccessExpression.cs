using System;
using System.CodeDom.Compiler;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.core.Tokens.Literals;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions.Access;

internal class EclArrayAccessExpression : EclLValueExpression
{
    public EclArrayAccessExpression(EclSourceSpan position, EclExpression left, EclExpression index) : base(position)
    {
        Left = left;
        Index = index;
    }

    public EclExpression Left { get; }
    public EclExpression Index { get; }

    public override EclToken Execute(EclInterpreterContext context)
    {
        return ToReference(context);
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclArrayAccess: ");
        Left.GetDebugString(writer);
        writer.Write("[");
        Index.GetDebugString(writer);
        writer.Write("]");
    }

    public override EclTokenReference ToReference(EclInterpreterContext context)
    {
        var indexValue = Index.Execute(context);
        var leftValue = Left.Execute(context).Dereference(new EclObject());
        if (leftValue is EclArray arr)
        {
            if (indexValue is EclNumber indexInt && indexInt.IsValidInt)
            {
                return new EclTokenReference(arr[indexInt.AsInt]);
            }
            throw new InvalidOperationException($"Index must be an integer: {indexValue.GetType().Name}");
        }
        else if(leftValue is EclObject obj)
        {
            if (indexValue is EclString indexStr)
            {
                return new EclTokenReference(obj[indexStr.Value]);
            }
            throw new InvalidOperationException($"Index must be a string for EclObject: {indexValue.GetType().Name}");
        }
        throw new InvalidOperationException($"Left expression does not evaluate to an EclArray: {leftValue.GetType().Name}");
    }
}