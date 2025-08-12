using System.CodeDom.Compiler;
using System.Globalization;
using ecl.core.Tokens.Literals;

namespace ecl.core.Tokens;

public abstract class EclLiteral : EclToken
{
    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.Write(GetDebugString());
    }


    public override EclToken Clone()
    {
        return this;
    }
    public static EclString CreateString(string value)
    {
        return new EclString(value);
    }
        
    public static EclNumber CreateNumber(string value) 
    {
        return new EclNumber(value);
    }
    public static EclNumber CreateNumber(float value) 
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(double value) 
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(decimal value) 
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(long value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }

    public static EclNumber CreateNumber(int value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(ulong value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(uint value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(short value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(ushort value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(byte value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
    public static EclNumber CreateNumber(sbyte value)
    {
        return new EclNumber(value.ToString(CultureInfo.InvariantCulture));
    }
        
    public static EclNull Null { get; } = new EclNull();
    public static EclBoolean True { get; } = new EclBoolean(true);
    public static EclBoolean False { get; } = new EclBoolean(false);
}