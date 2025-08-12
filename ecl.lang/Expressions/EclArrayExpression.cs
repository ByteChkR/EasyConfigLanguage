using System.CodeDom.Compiler;
using System.Collections.Generic;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal class EclArrayExpression : EclExpression
{
    public List<EclExpression> Elements { get; }

    public EclArrayExpression(EclSourceSpan position, List<EclExpression> elements) : base(position)
    {
        Elements = elements;
    }

    public override EclToken Execute(EclInterpreterContext context)
    {
        var arr = new EclArray(Elements.Count);
        var child = context.CreateChild(arr);
        for (var i = 0; i < Elements.Count; i++)
        {
            var element = Elements[i];
            var value = element.Execute(child);
            arr[i].SetValue(value);
        }
        return arr;
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclArray: ");
        if (Elements.Count == 0)
        {
            writer.Write("[]");
            return;
        }
        writer.WriteLine("[");
        writer.Indent++;
        for (int i = 0; i < Elements.Count; i++)
        {
            Elements[i].GetDebugString(writer);
            writer.WriteLine();
        }
        writer.Indent--;
        writer.Write("]");
    }
}