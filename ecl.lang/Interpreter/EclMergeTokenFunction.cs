using System;
using ecl.core.Tokens;
using ecl.merge;

namespace ecl.lang.Interpreter;

public class EclMergeTokenFunction : EclInterpreterFunction
{
    public override EclToken Invoke(EclInterpreterContext caller, EclToken[] args)
    {
        if(args.Length == 0)
            throw new ArgumentException("merge function requires at least one argument (objects to merge).", nameof(args));
        var first = args[0];
        if (args.Length == 1)
        {
            //Merge first with the caller
            
        }
        for (int i = 1; i < args.Length; i++)
        {
            first = EclMerge.Merge(first, args[i]);
        }

        return first;
    }
}