using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.core.Tokens.Literals;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ecl.json;

public static class EclJsonExtensions
{
    private static EclArray ToEclArray(JArray array)
    {
        return new EclArray(array.Select(x=>x.ToEclToken()));
    }

    private static EclObject ToEclObject(JObject obj)
    {
        return new EclObject(obj.Properties().ToDictionary(x=>x.Name, x=>x.Value.ToEclToken()));
    }
        
    private static EclLiteral ToEclLiteral(JValue value)
    {
        return value.Type switch
        {
            JTokenType.Null => EclLiteral.Null,
            JTokenType.Boolean => value.Value<bool>() ? EclLiteral.True : EclLiteral.False,
            JTokenType.Integer => EclLiteral.CreateNumber(value.ToString(CultureInfo.InvariantCulture)),
            JTokenType.Float => EclLiteral.CreateNumber(value.ToString(CultureInfo.InvariantCulture)),
            _ => EclLiteral.CreateString(value.Value<string>()!),
        };
    }
        
        
    private static JObject ToJObject(EclObject eclObject)
    {
        var jObject = new JObject();
        foreach (var kvp in eclObject.Properties)
        {
            jObject[kvp.Key] = ToJToken(kvp.GetValue(null));
        }
        return jObject;
    }
        
    private static JArray ToJArray(EclArray eclArray)
    {
        var jArray = new JArray();
        foreach (var item in eclArray)
        {
            jArray.Add(ToJToken(item.GetValue(null)));
        }
        return jArray;
    }

    public static T? ToObject<T>(this EclToken token) => token.ToJToken().ToObject<T>();
    private static JValue ToJValue(EclLiteral literal)
    {
        return literal switch
        {
            EclString str => new JValue(str.Value),
            EclNumber num => new JValue(decimal.Parse(num.Value, CultureInfo.InvariantCulture)),
            EclBoolean boolean => new JValue(boolean.Value),
            EclNull _ => JValue.CreateNull(),
            _ => throw new NotSupportedException($"Unsupported EclLiteral type: {literal.GetType()}")
        };
    }
    public static EclToken ToEclToken(this JToken token)
    {
        return token switch
        {
            JArray array => ToEclArray(array),
            JObject obj => ToEclObject(obj),
            JValue value => ToEclLiteral(value),
            _ => throw new NotSupportedException($"Unsupported JToken type: {token.Type}")
        };
    }
    
    public static EclObject ToEclToken(this JObject obj)
    {
        return ToEclObject(obj);
    }
    
    public static EclArray ToEclToken(this JArray array)
    {
        return ToEclArray(array);
    }
    
    public static EclLiteral ToEclToken(this JValue value)
    {
        return ToEclLiteral(value);
    }
    
    public static JToken ToJToken(this EclToken token)
    {
        return token switch
        {
            EclObject eclObject => ToJObject(eclObject),
            EclArray eclArray => ToJArray(eclArray),
            EclLiteral literal => ToJValue(literal),
            _ => throw new NotSupportedException($"Unsupported EclToken type: {token.GetType()}")
        };
    }
    
    public static JObject ToJToken(this EclObject eclObject)
    {
        return ToJObject(eclObject);
    }
    
    public static JArray ToJToken(this EclArray eclArray)
    {
        return ToJArray(eclArray);
    }
    
    public static JValue ToJToken(this EclLiteral literal)
    {
        return ToJValue(literal);
    }

    public static string ToYamlString(this JToken token)
    {
        var expConverter = new ExpandoObjectConverter();
        dynamic deserializedObject;
        if(token is JArray)
        {
            deserializedObject = JsonConvert.DeserializeObject<List<object>>(token.ToString(Formatting.Indented), expConverter)!;
        }
        else
        {
            deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(token.ToString(Formatting.Indented), expConverter)!;
        }

        var serializer = new YamlDotNet.Serialization.Serializer();
        string yaml = serializer.Serialize(deserializedObject);
        return yaml;
    }
}