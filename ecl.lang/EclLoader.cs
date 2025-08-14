using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.lang.Expressions;
using ecl.lang.Interpreter;
using ecl.lang.Parser;
using ecl.merge;
using Microsoft.Extensions.FileSystemGlobbing;

namespace ecl.lang
{
    
    public class EclUtils
    {
        public static IEnumerable<string> ExpandPatterns(params string[] patterns)
        {
            var matcher = new Matcher();
            foreach (var pattern in patterns)
            {
                if (pattern.StartsWith("!"))
                {
                    matcher.AddInclude(pattern.Substring(1));
                }
                else
                {
                    matcher.AddInclude(pattern);
                }
            }
        
            var files = matcher.GetResultsInFullPath(Directory.GetCurrentDirectory());
            return files;
        }
    }
    
    public class EclLoader
    {
        private readonly EclInterpreterFunctions _functions;

        public EclLoader() : this(EclInterpreterFunctions.Default)
        {
        }
        public EclLoader(EclInterpreterFunctions functions)
        {
            _functions = functions;
        }

        public static EclToken Load(params IEnumerable<EclSource> sources) => new EclLoader().Load(sources.ToArray());
        
        public EclToken Load(EclSource[] source)
        {
            List<EclToken> mergeList = new List<EclToken>();
            foreach (var s in source)
            {
                var reader = new EclSourceReader(s);
                var parser = new EclSourceParser(reader, _functions);
                var result = parser.Parse().ToList();
                EclInterpreterContext ctx = new EclInterpreterContext(_functions);
                EclExpression expr;
                if (result.All(x => x is EclAssignmentExpression))
                {
                    expr = new EclObjectExpression(s.Span, result);
                }
                else if (result.Count != 1)
                {
                    expr = new EclArrayExpression(s.Span, result);
                }
                else
                {
                    expr = result[0];
                }


                mergeList.Add(expr.Execute(ctx));
            }

            var resultToken = mergeList.First();
            for (int i = 1; i < mergeList.Count; i++)
            {
                resultToken = EclMerge.Merge(resultToken, mergeList[i]);
            }
            
            return resultToken;
        }
    }
}