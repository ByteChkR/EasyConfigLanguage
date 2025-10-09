namespace ecl.core.Tokens;

public static class EclTokenReferenceExtensions
{
    public static EclToken Dereference(this EclToken token, EclToken? initialValue = null)
    {
        if (token is EclTokenReference r) return r.Property.GetValue(initialValue);
        return token;
    }
}