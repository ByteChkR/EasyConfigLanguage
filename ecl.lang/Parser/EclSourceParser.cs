using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ecl.core.Tokens;
using ecl.core.Tokens.Literals;
using ecl.lang.Expressions;
using ecl.lang.Expressions.Access;
using ecl.lang.Interpreter;

namespace ecl.lang.Parser;

internal class EclSourceParser
{
    public EclSourceParser(EclSourceReader reader, EclInterpreterFunctions functions)
    {
        Reader = reader;
        Functions = functions;
    }
    // ECL Syntax:
    // Boolean: true, false
    // Null: null
    // Numbers: 123, 45.67, -89, 0.001, 1e10, -2.5e-3, 0xFF, 0b1010
    // Strings: "Hello, World!", "Line1\nLine2", "Escape: \"quotes\" and \\backslashes\\"
    // Identifiers: myVariable, _tempVar, 1stName, $myHiddenVar(when assigning), myHiddenVar(when accessing)
    // Access: myObject.property, myArray[0]
    // Comments: // This is a comment, /* This is a block comment */
    // Special Methods(at statement level): @import
    // Special Methods(at expression level): @import, @concat, @len, @append, @prepend, ...
    // Method Calls: @<identifier>(arg1, arg2, ...)

    public EclSourceReader Reader { get; }
    public EclInterpreterFunctions Functions { get; }


    private EclExpression ParseMethodCall()
    {
        var start = Reader.CurrentIndex;
        Reader.Eat('@');
        var methodName = ParseLiteralIdentifier();
        Reader.SkipWhitespace();
        var function = Functions.HasFunction(methodName)
            ? Functions.GetFunction(methodName)
            : throw new InvalidOperationException($"Function '{methodName}' not found.");
        Reader.SkipWhitespace();
        Reader.Eat('(');
        Reader.SkipWhitespace();
        var args = new List<EclExpression>();
        while (!Reader.IsEof() && !Reader.Is(')'))
        {
            var arg = ParseValueExpression();
            args.Add(arg);
            Reader.SkipWhitespace();
        }
        
        Reader.Eat(')');
        var pos = Reader.GetPosition(start);
        return new EclFunctionCallExpression(pos, function, args.ToArray(), methodName);
    }

    private EclExpression ParseFormatString()
    {
        //Parse string with this format: $"Hello {name}, you have {count} new messages."
        // Name and count can be any valid expression
        var start = Reader.CurrentIndex;
        Reader.Eat('$');
        Reader.Eat('"');
        var parts = new List<EclExpression>();
        var sb = new StringBuilder();
        while (!Reader.IsEof() && !Reader.Is('"'))
        {
            if (Reader.Is('\\'))
            {
                Reader.Move(); // Skip the backslash
                if (Reader.IsEof())
                    throw new InvalidOperationException("Unterminated string literal.");
                sb.Append(Regex.Unescape($"\\{Reader.Peek()}"));
            }
            else if (Reader.Is('{'))
            {
                if (Reader.Is("{{"))
                {
                    sb.Append(Reader.Peek());
                    Reader.Move();
                    sb.Append(Reader.Peek());
                    Reader.Move();
                    continue;
                }
                // Flush current string part
                if (sb.Length > 0)
                {
                    var strPos = Reader.GetPosition(start);
                    parts.Add(new EclConstantExpression(strPos, EclLiteral.CreateString(sb.ToString())));
                    sb.Clear();
                }

                Reader.Move(); // Skip '{'
                Reader.SkipWhitespace();
                var expr = ParseValueExpression();
                parts.Add(expr);
                Reader.SkipWhitespace();
                if (Reader.IsEof() || !Reader.Is('}'))
                    throw new InvalidOperationException("Unterminated expression in format string.");
                Reader.Move(); // Skip '}'
            }
            else
            {
                sb.Append(Reader.Peek());
                Reader.Move();
            }
            
        }
        
        if (Reader.IsEof() || !Reader.Is('"'))
            throw new InvalidOperationException("Unterminated string literal.");
        Reader.Move(); // Skip the closing quote
        if (sb.Length > 0)
        {
            var strPos = Reader.GetPosition(start);
            parts.Add(new EclConstantExpression(strPos, EclLiteral.CreateString(sb.ToString())));
        }
        var pos = Reader.GetPosition(start);
        return new EclFormatStringExpression(pos, parts);
    }
    
    private EclExpression ParseString()
    {
        //Parse String Literal
        var start = Reader.CurrentIndex;
        Reader.Eat('"');
        var sb = new StringBuilder();
        while (!Reader.IsEof() && !Reader.Is('"'))
        {
            if (Reader.Is('\\'))
            {
                Reader.Move(); // Skip the backslash
                if (Reader.IsEof())
                    throw new InvalidOperationException("Unterminated string literal.");
                sb.Append(Regex.Unescape($"\\{Reader.Peek()}"));
            }
            else
            {
                sb.Append(Reader.Peek());
            }

            Reader.Move();
        }

        if (Reader.IsEof() || !Reader.Is('"'))
            throw new InvalidOperationException("Unterminated string literal.");
        Reader.Move(); // Skip the closing quote
        var pos = Reader.GetPosition(start);
        return new EclConstantExpression(pos, EclLiteral.CreateString(sb.ToString()));
    }

    private EclExpression? ParseLiteral()
    {
        Reader.SkipWhitespace();
        var reset = Reader.CurrentIndex;
        
        if (Reader.Is('[') || Reader.Is('{'))
        {
            return null; // Array or Object start, not a literal
        }

        if (Reader.Is('@'))
        {
            return ParseMethodCall();
        }
        if (Reader.Is("true"))
        {
            var start = Reader.CurrentIndex;
            Reader.Eat("true");
            var pos = Reader.GetPosition(start);
            return new EclConstantExpression(pos, EclLiteral.True);
        }

        if (Reader.Is("false"))
        {
            var start = Reader.CurrentIndex;
            Reader.Eat("false");
            var pos = Reader.GetPosition(start);
            return new EclConstantExpression(pos, EclLiteral.False);
        }

        if (Reader.Is("null"))
        {
            var start = Reader.CurrentIndex;
            Reader.Eat("null");
            var pos = Reader.GetPosition(start);
            return new EclConstantExpression(pos, EclLiteral.Null);
        }

        if (Reader.Is('"')) return ParseString();
        if(Reader.Is($"$\"")) return ParseFormatString();

        // Try to parse a number
        var numberStart = Reader.CurrentIndex;
        var numberBuilder = new StringBuilder();
        // Number Examples: 123, 45.67, -89, 0.001, 1e10, -2.5e-3, 0xFF, 0b1010
        var hasDecimalPoint = false;
        var hasExponent = false;
        var hasHexPrefix = false;
        var hasBinaryPrefix = false;

        while (!Reader.IsEof() && (char.IsDigit(Reader.Peek()) || Reader.Is('.') || Reader.Is('e') ||
                                   Reader.Is('E') || Reader.Is('-') || Reader.Is('+')))
        {
            if (Reader.Is('.'))
            {
                if (hasDecimalPoint)
                {
                    Reader.CurrentIndex = reset;
                    return null;
                }

                hasDecimalPoint = true;
            }
            else if (Reader.Is('e') || Reader.Is('E'))
            {
                if (hasExponent)
                {
                    Reader.CurrentIndex = reset;
                    return null;
                }

                hasExponent = true;
                numberBuilder.Append(Reader.Peek());
                Reader.Move();
                if (Reader.Is('-') || Reader.Is('+'))
                {
                    numberBuilder.Append(Reader.Peek());
                    Reader.Move();
                }
            }
            else if (Reader.Is('x') && !hasHexPrefix && numberBuilder.Length == 0)
            {
                hasHexPrefix = true;
                numberBuilder.Append("0x");
            }
            else if (Reader.Is('b') && !hasBinaryPrefix && numberBuilder.Length == 0)
            {
                hasBinaryPrefix = true;
                numberBuilder.Append("0b");
            }
            else
            {
                numberBuilder.Append(Reader.Peek());
            }

            Reader.Move();
        }

        if (numberBuilder.Length != 0 &&
            (!hasHexPrefix || Regex.IsMatch(numberBuilder.ToString(), @"^0x[0-9A-Fa-f]+$")) &&
            (!hasBinaryPrefix || Regex.IsMatch(numberBuilder.ToString(), @"^0b[01]+$")))
        {
            var num = EclLiteral.CreateNumber(numberBuilder.ToString());
            if(num.IsValid)
                return new EclConstantExpression(Reader.GetPosition(numberStart), num);
        }
        // If we reach here, it means we didn't find a valid number, we will parse a literal identifier instead
        Reader.CurrentIndex = reset;
        
        return ParseIdentifier();
    }

    private EclExpression ParseArray()
    {
        var start = Reader.CurrentIndex;
        Reader.Eat('[');
        Reader.SkipWhitespace();
        var elements = new List<EclExpression>();
        while (!Reader.IsEof() && !Reader.Is(']'))
        {
            var element = ParseValueExpression();
            Reader.SkipWhitespace();
            elements.Add(element);
        }

        Reader.Eat(']');
        var pos = Reader.GetPosition(start);
        return new EclArrayExpression(pos, elements);
    }

    private string ParseLiteralIdentifier()
    {
        string name;
        if (Reader.Is('"'))
        {
            var str = (EclConstantExpression)ParseString();
            name = ((EclString)str.Value).Value;
        }
        else
        {
            // Read until whitespace
            var sb = new StringBuilder();
            while (!Reader.IsEof() && !char.IsWhiteSpace(Reader.Peek()) &&
                   !Reader.Is('.') &&
                   !Reader.Is('[') && !Reader.Is(']') &&
                   !Reader.Is('(') && !Reader.Is(')') &&
                   !Reader.Is('{') && !Reader.Is('}'))
            {
                sb.Append(Reader.Peek());
                Reader.Move();
            }

            name = sb.ToString();
        }

        if (string.IsNullOrEmpty(name))
            throw new InvalidOperationException("Expected an identifier but found none.");
        return name;
    }

    private EclLValueExpression ParseIdentifier()
    {
        var start = Reader.CurrentIndex;
        var name = ParseLiteralIdentifier();
        var pos = Reader.GetPosition(start);
        EclLValueExpression current = new EclIdentifierExpression(pos, name);
        Reader.SkipWhitespace();
        while (Reader.Is('.') || Reader.Is('['))
        {
            start = Reader.CurrentIndex;
            if (Reader.Is('.'))
            {
                Reader.Eat('.');
                Reader.SkipWhitespace();
                var propertyName = ParseLiteralIdentifier();
                var propertyPos = Reader.GetPosition(start);
                current = new EclMemberAccessExpression(propertyPos, propertyName, current);
            }
            else if (Reader.Is('['))
            {
                Reader.Eat('[');
                Reader.SkipWhitespace();
                var indexExpr = ParseValueExpression();
                Reader.SkipWhitespace();
                Reader.Eat(']');
                var indexPos = Reader.GetPosition(start);
                current = new EclArrayAccessExpression(indexPos, current, indexExpr);
            }
            else break;
            Reader.SkipWhitespace();
        }
        
        return current;
    }

    private EclExpression ParseObject()
    {
        var start = Reader.CurrentIndex;
        Reader.Eat('{');
        Reader.SkipWhitespace();
        var properties = Parse().ToList();

        Reader.Eat('}');
        var pos = Reader.GetPosition(start);
        return new EclObjectExpression(pos, properties);
    }

    public IEnumerable<EclExpression> Parse()
    {
        while (!Reader.IsEof() && !Reader.Is('}'))
        {
            Reader.SkipWhitespace();
            var expr= ParseValueExpression();
            Reader.SkipWhitespace();
            if (expr is not EclFunctionCallExpression && Reader.Is('=') && expr is EclLValueExpression lValue)
            {
                yield return ParseAssignmentExpression(lValue);
            }
            else
            {
                yield return expr;
            }
            Reader.SkipWhitespace();
        }
    }

    public EclExpression ParseAssignmentExpression(EclLValueExpression left)
    {
        Reader.Eat("=");
        Reader.SkipWhitespace();
        var right = ParseValueExpression();
        return new EclAssignmentExpression(left.Position, left, right);
    }
    
    public EclExpression ParseValueExpression()
    {
        var literal = ParseLiteral();
        if (literal != null)
            return literal;


        Reader.SkipWhitespace();
        if (Reader.Is('[')) return ParseArray();
        if (Reader.Is('{')) return ParseObject();

        throw new InvalidOperationException("Expected a literal, array, or object but found none.");
    }
}