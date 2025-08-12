using Microsoft.Extensions.FileSystemGlobbing;

namespace ecl.cli;

public class EclCliUtils
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