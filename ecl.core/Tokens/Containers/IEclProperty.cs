namespace ecl.core.Tokens.Containers;

public interface IEclProperty<TKey>
{
    TKey Key { get; }
    EclToken GetValue(EclToken? initialValue);
    void SetValue(EclToken value);
}