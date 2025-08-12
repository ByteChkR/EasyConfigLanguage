using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ecl.core.Tokens.Literals;

public class EclString : EclLiteral
{
    public string Value { get; }

    internal EclString(string value)
    {
        Value = value;
    }

    
    private static readonly char[] UnsafeChars = new char[] {'\\', '\n', '\r', '"', '\t', '\0', '\a', '\b', '\f', '\v', '\x1B', '\x1C', '\x1D', '\x1E', '\x1F'};

    public static bool IsUnsafe(string str)
    {
        return str.IndexOfAny(UnsafeChars) >= 0;
    }
    public static string Escape(string str)
    {
        var key = str;
        foreach (var c in UnsafeChars)
        {
            key = key.Replace(c.ToString(), $"\\{c}");
        }
        return key;
    }
    
    public override string GetDebugString()
    {
        return $"\"{Escape(Value)}\"";
    }
}