using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.json;

namespace ecl.cli;

internal class ProgramSettings
{
    private EclToken _token;

    public ProgramSettings(EclToken token)
    {
        _token = token;
    }
    
    public EclToken Token => _token;
    public ProgramSettings GetToken(string key)
    {
        if (_token is not EclObject obj)
        {
            throw new InvalidOperationException("ProgramSettings token is not an EclObject.");
        }
        return new ProgramSettings(obj[key].GetValue(null));
    }

    public T? ToObject<T>(string key)
    {
        return GetToken(key).Token.ToObject<T>();
    }

    public T? ToObject<T>()
    {
        return Token.ToObject<T>();
    }
}