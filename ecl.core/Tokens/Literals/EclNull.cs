namespace ecl.core.Tokens.Literals;

public class EclNull : EclLiteral
{
    internal EclNull() { }
    public override string GetDebugString()
    {
        return "null";
    }
}