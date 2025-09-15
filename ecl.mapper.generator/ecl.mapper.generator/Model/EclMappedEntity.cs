using System;
using Microsoft.CodeAnalysis;

namespace ecl.mapper.generator.Model;

public readonly struct EclMappedEntity : IEquatable<EclMappedEntity>
{
	
    public Diagnostic[] Diagnostics { get; }
    public EclMappedEntity(string ns, string className, EclMappedProperty[] properties, Diagnostic[] diagnostics)
    {
        Namespace = ns;
        ClassName = className;
        Properties = properties;
        Diagnostics = diagnostics;
    }

    public string Namespace { get; }
    public string ClassName { get; }
    public EclMappedProperty[] Properties { get; }

    public bool Equals(EclMappedEntity other)
    {
        return Diagnostics.Equals(other.Diagnostics) && Namespace == other.Namespace && ClassName == other.ClassName && Properties.Equals(other.Properties);
    }

    public override bool Equals(object? obj)
    {
        return obj is EclMappedEntity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Diagnostics, Namespace, ClassName, Properties);
    }

    public static bool operator ==(EclMappedEntity left, EclMappedEntity right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EclMappedEntity left, EclMappedEntity right)
    {
        return !left.Equals(right);
    }
}