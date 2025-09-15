using System;

namespace ecl.mapper.generator.Model;

public readonly struct EclMappedProperty : IEquatable<EclMappedProperty>
{
    public EclMappedProperty(string propertyName, string propertyType, bool isNested)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
        IsNested = isNested;
    }

    public string PropertyName { get; }
    public string PropertyType { get; }
    public bool IsNested { get; }

    public bool Equals(EclMappedProperty other)
    {
        return PropertyName == other.PropertyName && PropertyType == other.PropertyType && IsNested == other.IsNested;
    }

    public override bool Equals(object? obj)
    {
        return obj is EclMappedProperty other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PropertyName, PropertyType, IsNested);
    }

    public static bool operator ==(EclMappedProperty left, EclMappedProperty right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EclMappedProperty left, EclMappedProperty right)
    {
        return !left.Equals(right);
    }
}