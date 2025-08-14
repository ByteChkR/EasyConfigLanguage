namespace ecl.lang
{
    public struct EclSourceSpan
    {
        public EclSourceSpan(EclSource source, int index, int length)
        {
            Source = source;
            Index = index;
            Length = length;
        }

        public int Index { get; }
        public int Length { get; }
        public EclSource Source { get; }
    }
}