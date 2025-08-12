using System.CodeDom.Compiler;
using ecl.core.Tokens;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal class EclFunctionCallExpression : EclExpression
{
    public EclFunctionCallExpression(EclSourceSpan position, EclInterpreterFunction function, EclExpression[] arguments, string methodName) : base(position)
    {
        Function = function;
        Arguments = arguments;
        MethodName = methodName;
    }

    public string MethodName { get; }
    public EclInterpreterFunction Function;
    public EclExpression[] Arguments;
    public override EclToken Execute(EclInterpreterContext context)
    {
        var argValues = new EclToken[Arguments.Length];
        for (int i = 0; i < Arguments.Length; i++)
        {
            argValues[i] = Arguments[i].Execute(context);
        }
        return Function.Invoke(context, argValues);
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclMethodCall: ");
        writer.Write(MethodName);
        writer.Write("(");
        for (int i = 0; i < Arguments.Length; i++)
        {
            if (i > 0) writer.Write(", ");
            Arguments[i].GetDebugString(writer);
        }
        writer.WriteLine(")");
    }
}