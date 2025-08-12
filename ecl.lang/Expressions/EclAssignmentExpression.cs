using System.CodeDom.Compiler;
using ecl.core.Tokens;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal class EclAssignmentExpression : EclExpression
{
    public EclAssignmentExpression(EclSourceSpan position, EclLValueExpression left, EclExpression right) : base(position)
    {
        Left = left;
        Right = right;
    }

    public EclLValueExpression Left { get; }
    public EclExpression Right { get; }
    public override EclToken Execute(EclInterpreterContext context)
    {
        var leftRef = Left.ToReference(context);
        var rightValue = Right.Execute(context);
        leftRef.Property.SetValue(rightValue);
        return rightValue;
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclAssignment: ");
        Left.GetDebugString(writer);
        writer.Write(" = ");
        Right.GetDebugString(writer);
    }
}