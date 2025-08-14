using System;
using System.IO;

namespace ecl.lang
{
    public class EclSource
    {
        public string FileName { get; }
        public string Content { get; }
        public EclSourceSpan Span { get; }
        private EclSource(string fileName, string content)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Span = new EclSourceSpan(this, 0, content.Length);
        }
        
        

        public static EclSource FromSource(string name, string src)
        {
            return new EclSource(name, src);
        }

        public static EclSource FromFile(string file)
        {
            return FromSource(file, File.ReadAllText(file));
        }

        public static EclSource[] FromFiles(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                throw new ArgumentException("No files provided.", nameof(files));
            }

            var sources = new EclSource[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                sources[i] = FromFile(files[i]);
            }

            return sources;
        }
    }
}