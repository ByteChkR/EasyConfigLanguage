using System.Linq;
using Microsoft.CodeAnalysis;

namespace ecl.mapper.generator;

public static class CommonExtensions
{
    public static bool Implements(this ITypeSymbol type, string typeName)
    {
        return type.ToDisplayString() == typeName || //If its this type
               (type.BaseType != null && type.BaseType.Implements(typeName)) || //Recurse up the tree and check base types
               type.AllInterfaces.Any(x => x.ToDisplayString() == typeName); //Recurse back down and check the interfaces
    }
}