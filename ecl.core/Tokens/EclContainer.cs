using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;

namespace ecl.core.Tokens;

public abstract class EclContainer : EclToken, IEnumerable<EclProperty>
{
    public event EclTokenChangedEventHandler OnChanged = delegate { };

    public override string GetDebugString()
    {
        var writer = new System.IO.StringWriter();
        GetDebugString(new IndentedTextWriter(writer));
        writer.Flush();
        return writer.ToString();
    }

    protected void RaiseChanged(EclProperty property, EclToken oldValue)
    {
        OnChanged(property, oldValue);
    }
    public abstract IEnumerator<EclProperty> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}