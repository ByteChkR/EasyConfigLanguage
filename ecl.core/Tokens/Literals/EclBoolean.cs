namespace ecl.core.Tokens.Literals;

public class EclBoolean : EclLiteral
{
    public bool Value { get; }

    internal EclBoolean(bool value)
    {
        Value = value;
    }

    public override string GetDebugString()
    {
        return Value ? "true" : "false";
    }

}