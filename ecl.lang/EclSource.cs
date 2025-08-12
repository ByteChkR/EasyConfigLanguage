using System;

namespace ecl.lang
{
    internal class EclSource
    {
        public string FileName { get; }
        public string Content { get; }
        public EclSourceSpan Span { get; }
        public EclSource(string fileName, string content)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Span = new EclSourceSpan(this, 0, content.Length);
        }
    }
}