using System;
using System.Linq;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;

namespace ecl.merge;

public static class EclMerge
{
    private static void MergeInplace(EclObject first, EclToken second)
    {
        if (second is not EclObject secondObject)
            throw new ArgumentException("Second token must be an EclObject", nameof(second));

        foreach (var prop in secondObject.Properties)
        {
            var newValue = prop.GetValue(null);
            EclToken? oldValue = null;
            if (first.Properties.Any(x => x.Key == prop.Key))
            {
                oldValue = first[prop.Key].GetValue(null);
            }
            if (oldValue is EclContainer)
            {
                MergeInplace(oldValue, newValue);
            }
            else
            {
                first[prop.Key].SetValue(prop.GetValue(null));
            }
        }
    }
    private static void MergeInplace(EclArray first, EclToken second)
    {
        if (second is not EclArray secondArray)
            throw new ArgumentException("Second token must be an EclArray", nameof(second));

        foreach (var item in secondArray)
        {
            first.Add(item.GetValue(null));
        }
    }
    private static EclToken Merge(EclObject first, EclToken second)
    {
        if (second is not EclObject secondObject)
            throw new ArgumentException("Second token must be an EclObject", nameof(second));

        var result = new EclObject();
        foreach (var prop in first.Properties)
        {
            result[prop.Key].SetValue(prop.GetValue(null));
        }

        MergeInplace(result, secondObject);

        return result;
    }

    private static EclToken Merge(EclArray first, EclToken second)
    {
        if (second is not EclArray secondArray)
            throw new ArgumentException("Second token must be an EclArray", nameof(second));
        return new EclArray(
            first.Concat(secondArray)
                .Select(x => x.GetValue(null))
        );
    }

    public static EclToken Merge(EclToken first, EclToken second)
    {
        //Switch on the type of the first token
        switch (first)
        {
            case EclArray array:
                return Merge(array, second);
            case EclObject obj:
                return Merge(obj, second);
            default:
                throw new NotSupportedException($"Merging is not supported for type {first.GetType().Name}");
        }
    }
    
    public static void MergeInplace(EclToken first, EclToken second)
    {
        //Switch on the type of the first token
        switch (first)
        {
            case EclArray array:
                MergeInplace(array, second);
                break;
            case EclObject obj:
                MergeInplace(obj, second);
                break;
            default:
                throw new NotSupportedException($"Merging in place is not supported for type {first.GetType().Name}");
        }
    }
}