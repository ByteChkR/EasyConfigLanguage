using System;
using System.Collections.Generic;
using System.IO;
using ecl.core.Tokens;
using ecl.core.Tokens.Literals;

namespace ecl.lang.Interpreter;

public class EclLoadFileFunction : EclInterpreterFunction
{
    public override EclToken Invoke(EclInterpreterContext caller, EclToken[] args)
    {
        if(args.Length == 0)throw new ArgumentException("load function requires at least one argument (file path).", nameof(args));
        var sources = new List<EclSource>();
        foreach (var arg in args)
        {
            if(arg is not EclString file)
                throw new ArgumentException("load function requires string arguments (file paths).", nameof(args));
            if (!File.Exists(file.Value))
            {
                throw new FileNotFoundException($"File not found: {file.Value}", file.Value);
            }
            sources.Add(EclSource.FromFile(file.Value));
        }

        return new EclLoader(caller.Functions).Load(sources.ToArray());
    }
}