using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ecl.core.Tokens.Literals;

namespace ecl.core.Tokens.Containers;

public class EclObject : EclContainer
{
    //Overload [string] to access properties by key
    public IEnumerable<EclProperty<EclObject, string>> Properties => _propertyCache.Values;
    public EclProperty<EclObject, string> this[string key]
    {
        get
        {
            if (!_propertyCache.TryGetValue(key, out var property))
            {
                property = new EclObjectProperty(this, key);
            }
            return property;
        }
    }
    private class EclObjectProperty : EclProperty<EclObject, string>, IEclProperty<string>
    {

        public EclObjectProperty(EclObject parent, string key) : base(parent, key)
        {
        }

        public override EclToken GetValue(EclToken? initialValue)
        {
            if (Parent._properties.TryGetValue(Key, out var value))
            {
                return value;
            }
            if (initialValue != null)
            {
                Parent._properties[Key] = initialValue;
                Parent._propertyCache[Key] = this;
                return initialValue;
            }
            throw new KeyNotFoundException($"Key '{Key}' not found in object.");
        }

        public override void SetValue(EclToken value)
        {
            if (!Parent._propertyCache.TryGetValue(Key, out var property))
            {
                Parent._propertyCache[Key] = this;
            }
            Parent._properties.TryGetValue(Key, out EclToken? oldValue);
            
            if (oldValue is EclContainer oldContainer)
            {
                oldContainer.OnChanged -= Parent.RaiseChanged;
            }
            
            Parent._properties[Key] = value;
            Parent.RaiseChanged(this, oldValue ?? EclLiteral.Null);
            
            if (value is EclContainer newContainer)
            {
                newContainer.OnChanged += Parent.RaiseChanged;
            }
        }

        public override void GetDebugString(IndentedTextWriter writer)
        {
            writer.Write($"{GetSaveKey()} = ");
            GetValue(null).GetDebugString(writer);
        }
        public override string GetDebugString()
        {
            return $"{GetSaveKey()}";
        }

        
        private string GetSaveKey()
        {
            //If the key contains any whitespace, control, or special characters, we need to escape it and put it in quotes
            if (EclString.IsUnsafe(Key))
            {
                return $"\"{EclString.Escape(Key)}\""; // Escape
            }

            if (Key.Contains('.') || Key.Any(char.IsWhiteSpace))
            {
                return $"\"{Key}\"";
            }
            return Key; // No need to escape
        }

    }
    private readonly Dictionary<string, EclToken> _properties;
    private readonly Dictionary<string, EclProperty<EclObject, string>> _propertyCache;

    public EclObject(): this(new Dictionary<string, EclToken>())
    {
    }
    public EclObject(Dictionary<string, EclToken> properties)
    {
        _properties = new Dictionary<string, EclToken>();
        _propertyCache = new Dictionary<string, EclProperty<EclObject, string>>();
        foreach (var kvp in properties)
        {
            var prop = _propertyCache[kvp.Key] = new EclObjectProperty(this, kvp.Key);
            prop.SetValue(kvp.Value);
        }
    }

    public override IEnumerator<EclProperty> GetEnumerator()
    {
        return _propertyCache.Values.GetEnumerator();
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        writer.WriteLine("{");
        writer.Indent++;
        foreach (var property in this)
        {
            property.GetDebugString(writer);
            writer.WriteLine();
        }
        writer.Indent--;
        writer.Write("}");
    }

    public override EclToken Clone()
    {
        return new EclObject(_properties.ToDictionary(x => x.Key, x => x.Value.Clone()));
    }
}