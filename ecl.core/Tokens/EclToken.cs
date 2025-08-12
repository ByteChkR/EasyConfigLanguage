using System.CodeDom.Compiler;

namespace ecl.core.Tokens;

public abstract class EclToken
{
    public abstract string GetDebugString();
    public abstract void GetDebugString(IndentedTextWriter writer);
    
    public override string ToString()
    {
        return GetDebugString();
    }

    public abstract EclToken Clone();
}