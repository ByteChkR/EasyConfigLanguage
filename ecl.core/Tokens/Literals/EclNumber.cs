using System.Numerics;

namespace ecl.core.Tokens.Literals;

public class EclNumber : EclLiteral
{
    // Represents a numeric value, could be an integer or a floating-point number. Can be of arbitrary precision and size. Ensure to handle large numbers appropriately.
    // We do not use System.Decimal or System.Double to avoid precision issues.
    public string Value { get; }
    internal EclNumber(string value)
    {
        Value = value;
    }
        
    //Properties for all numeric types can be added here, such as IsInteger, IsFloat, etc.
    public bool IsInteger => decimal.TryParse(Value, out var result) && result % 1 == 0;
    public bool IsFloat => !IsInteger;
    public decimal AsDecimal => decimal.Parse(Value);
    public double AsDouble => double.Parse(Value);
    public long AsLong => long.Parse(Value);
    public int AsInt => int.Parse(Value);
    public float AsFloat => float.Parse(Value);
    public ulong AsULong => ulong.Parse(Value);
    public uint AsUInt => uint.Parse(Value);
    public short AsShort => short.Parse(Value);
    public ushort AsUShort => ushort.Parse(Value);
    public byte AsByte => byte.Parse(Value);
    public sbyte AsSByte => sbyte.Parse(Value);
    public BigInteger AsBigInteger => BigInteger.Parse(Value);
    public bool IsBigInteger => BigInteger.TryParse(Value, out _);
    public BigInteger AsBigIntegerOrDefault => IsBigInteger ? BigInteger.Parse(Value) : BigInteger.Zero;
    public bool IsZero => AsDecimal == 0;
    public bool IsPositive => AsDecimal > 0;
    public bool IsNegative => AsDecimal < 0;
    public bool IsEven => IsInteger && AsDecimal % 2 == 0;
    public bool IsOdd => IsInteger && AsDecimal % 2 != 0;
    public bool IsNaN => double.IsNaN(AsDouble);
    public bool IsInfinity => double.IsInfinity(AsDouble);
    public bool IsFinite => double.IsFinite(AsDouble);
    public bool IsNegativeInfinity => double.IsNegativeInfinity(AsDouble);
    public bool IsPositiveInfinity => double.IsPositiveInfinity(AsDouble);
    public bool IsValidNumber => decimal.TryParse(Value, out _);
    public bool IsValidDouble => double.TryParse(Value, out _);
    public bool IsValidFloat => float.TryParse(Value, out _);
    public bool IsValidLong => long.TryParse(Value, out _);
    public bool IsValidInt => int.TryParse(Value, out _);
    public bool IsValidULong => ulong.TryParse(Value, out _);
    public bool IsValidUInt => uint.TryParse(Value, out _);
    public bool IsValidShort => short.TryParse(Value, out _);
    public bool IsValidUShort => ushort.TryParse(Value, out _);
    public bool IsValidByte => byte.TryParse(Value, out _);
    public bool IsValidSByte => sbyte.TryParse(Value, out _);
    public bool IsValidBigInteger => BigInteger.TryParse(Value, out _);
    public bool IsValid => IsValidNumber || IsValidDouble || IsValidFloat || IsValidLong || IsValidInt || IsValidULong || IsValidUInt || IsValidShort ||
                           IsValidUShort || IsValidByte || IsValidSByte || IsValidBigInteger;

    public override string GetDebugString()
    {
        return Value;
    }
}