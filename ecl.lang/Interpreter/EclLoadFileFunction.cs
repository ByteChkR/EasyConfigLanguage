using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ecl.core.Tokens;
using ecl.core.Tokens.Literals;

namespace ecl.lang.Interpreter;

public class EclLoadFileFunction : EclInterpreterFunction
{
    public override EclToken Invoke(EclInterpreterContext caller, EclToken[] args)
    {
        if(args.Length == 0)throw new ArgumentException("load function requires at least one argument (file path).", nameof(args));
        var sources = new List<EclSource>();
        foreach (var file in EclUtils.ExpandPatterns(args.Select(x=>((EclString)x).Value).ToArray()))
        {
            sources.Add(EclSource.FromFile(file));
        }

        return new EclLoader(caller.Functions).Load(sources.ToArray());
    }
}