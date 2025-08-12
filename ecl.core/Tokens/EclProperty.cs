using System.CodeDom.Compiler;
using ecl.core.Tokens.Containers;

namespace ecl.core.Tokens;

public abstract class EclProperty
{
    protected EclProperty(EclContainer parent)
    {
        Parent = parent;
    }

    public EclContainer Parent { get; }
    public abstract EclToken GetValue(EclToken? initialValue);
    public abstract void SetValue(EclToken value);
    public abstract string GetDebugString();
    public abstract void GetDebugString(IndentedTextWriter writer);
    
    public override string ToString()
    {
        var writer = new System.IO.StringWriter();
        GetDebugString(new IndentedTextWriter(writer));
        writer.Flush();
        return writer.ToString();
    }

}

public abstract class EclProperty<TParent, TKey> : EclProperty
    where TParent: EclContainer
{
    protected EclProperty(TParent parent, TKey key) : base(parent)
    {
        Parent = parent;
        Key = key;
    }

    public new TParent Parent { get; }
    public TKey Key { get; }
}