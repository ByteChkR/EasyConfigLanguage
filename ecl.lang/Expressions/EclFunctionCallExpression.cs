using System.CodeDom.Compiler;
using System.Collections.Generic;
using ecl.core.Tokens;
using ecl.core.Tokens.Literals;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal class EclFormatStringExpression : EclExpression
{
    public EclFormatStringExpression(EclSourceSpan position, IReadOnlyList<EclExpression> parts) : base(position)
    {
        Parts = parts;
    }

    public IReadOnlyList<EclExpression> Parts { get; }

    public override EclToken Execute(EclInterpreterContext context)
    {
        //Execute all parts and concatenate them as strings
        var result = "";
        foreach (var part in Parts)
        {
            var partValue = part.Execute(context).Dereference();
            if (partValue is EclString str)
            {
                result += str.Value;
            }
            else
            {
                result += partValue.ToString();
            }
        }
        
        return EclLiteral.CreateString(result);
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclFormatString: ");
        for (int i = 0; i < Parts.Count; i++)
        {
            if (i > 0) writer.Write(" + ");
            Parts[i].GetDebugString(writer);
        }
        writer.WriteLine();
    }
}
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