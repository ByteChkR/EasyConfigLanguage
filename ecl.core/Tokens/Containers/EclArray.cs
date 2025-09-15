using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using ecl.core.Tokens.Literals;

namespace ecl.core.Tokens.Containers;

public class EclArray : EclContainer
{
    public EclProperty<EclArray, int> this[int index]
    {
        get
        {
            if (index < 0 || index >= _properties.Count)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range for array with {_properties.Count} elements.");
            }
            return _properties[index];
        }
    }
    private class EclArrayProperty : EclProperty<EclArray, int>, IEclProperty<int>
    {
        public EclArrayProperty(EclArray parent, int index) : base(parent, index)
        {
        }

        public override EclToken GetValue(EclToken? initialValue)
        {
            if (initialValue != null)
                throw new ArgumentException("Initial value is not supported for array properties.");
            if (Key < 0 || Key >= Parent._values.Count)
            {
                throw new IndexOutOfRangeException($"Index {Key} is out of range for array with {Parent._values.Count} elements.");
            }
            return Parent._values[Key];
        }

        public override void SetValue(EclToken value)
        {
            if (Key < 0 || Key >= Parent._values.Count)
            {
                throw new IndexOutOfRangeException($"Index {Key} is out of range for array with {Parent._values.Count} elements.");
            }
            var oldValue = Parent._values[Key];

            if (oldValue is EclContainer oldContainer)
            {
                oldContainer.OnChanged -= Parent.RaiseChanged;
            }
            
            Parent._values[Key] = value;
            
            Parent.RaiseChanged(this, oldValue);

            if (value is EclContainer newContainer)
            {
                newContainer.OnChanged += Parent.RaiseChanged;
            }
        }

        public override string GetDebugString()
        {
            return $"{Key}";
        }

        public override void GetDebugString(IndentedTextWriter writer)
        {
            GetValue(null).GetDebugString(writer);
        }
    }
        
    private readonly List<EclToken> _values;
    private readonly List<EclArrayProperty> _properties;
    public IEnumerable<EclProperty<EclArray, int>> Properties => _properties;
    public EclArray(int size) : this(Enumerable.Repeat(EclLiteral.Null, size)){}
    public EclArray(params IEnumerable<EclToken> values)
    {
        var vals = values.ToList();
        _values = new List<EclToken>(Enumerable.Repeat<EclToken>(EclLiteral.Null, vals.Count));
        _properties = Enumerable.Range(0, vals.Count).Select(x=> new EclArrayProperty(this, x)).ToList();
        for (int i = 0; i < vals.Count; i++)
        {
            _properties[i].SetValue(vals[i]);
        }
    }

    public void Add(EclToken token)
    {
        _values.Add(token);
        _properties.Add(new EclArrayProperty(this, _values.Count - 1));
        if (token is EclContainer container)
        {
            container.OnChanged += RaiseChanged;
        }
    }

    public override IEnumerator<EclProperty> GetEnumerator()
    {
        return _properties.GetEnumerator();
    }

    public override EclToken Clone()
    {
        return new EclArray(_values.Select(x => x.Clone()));
    }

    public override void GetDebugString(IndentedTextWriter writer)
    {
        //Write in ECL format:
        if (_properties.Count == 0)
        {
            writer.Write("[]");
            return;
        }
        writer.WriteLine("[");
        writer.Indent++;
        foreach (var property in this)
        {
            property.GetDebugString(writer);
            writer.WriteLine();
        }
        writer.Indent--;
        writer.Write("]");
    }
}