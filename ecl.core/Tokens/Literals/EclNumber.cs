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
    private string IntegerString => Value.Split('.')[0];
    public decimal AsDecimal => decimal.Parse(Value);
    public double AsDouble => double.Parse(Value);
    public long AsLong => long.Parse(IntegerString);
    public int AsInt => int.Parse(IntegerString);
    public float AsFloat => float.Parse(Value);
    public ulong AsULong => ulong.Parse(IntegerString);
    public uint AsUInt => uint.Parse(IntegerString);
    public short AsShort => short.Parse(IntegerString);
    public ushort AsUShort => ushort.Parse(IntegerString);
    public byte AsByte => byte.Parse(IntegerString);
    public sbyte AsSByte => sbyte.Parse(IntegerString);
    public BigInteger AsBigInteger => BigInteger.Parse(IntegerString);
    public bool IsBigInteger => BigInteger.TryParse(IntegerString, out _);
    public BigInteger AsBigIntegerOrDefault => IsBigInteger ? BigInteger.Parse(IntegerString) : BigInteger.Zero;
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