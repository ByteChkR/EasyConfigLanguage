using System.CodeDom.Compiler;
using ecl.core.Tokens;

namespace ecl.lang.Interpreter;

public class EclTokenReference : EclToken
{
    public EclTokenReference(EclProperty property)
    {
        Property = property;
    }

    public EclProperty Property { get; }
    public override string GetDebugString()
    {
        return Property.GetValue(null).GetDebugString();
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        Property.GetValue(null).GetDebugString(writer);
    }

    public override EclToken Clone()
    {
        return Property.GetValue(null).Clone();
    }
}