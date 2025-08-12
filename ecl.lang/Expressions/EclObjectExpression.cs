using System.CodeDom.Compiler;
using System.Collections.Generic;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.lang.Interpreter;

namespace ecl.lang.Expressions;

internal class EclObjectExpression : EclExpression
{
    public List<EclExpression> Properties { get; }

    public EclObjectExpression(EclSourceSpan position, List<EclExpression> properties) : base(position)
    {
        Properties = properties;
    }

    public override EclToken Execute(EclInterpreterContext context)
    {
        var obj = new EclObject();
        var child = context.CreateChild(obj);
        foreach (var property in Properties)
        {
            property.Execute(child);
        }
        return obj;
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write("EclObject: ");
        if (Properties.Count == 0)
        {
            writer.Write("{}");
            return;
        }
        writer.WriteLine("{");
        writer.Indent++;
        for (int i = 0; i < Properties.Count; i++)
        {
            var property = Properties[i];
            property.GetDebugString(writer);
        }
        writer.Indent--;
        writer.Write("}");
    }
}