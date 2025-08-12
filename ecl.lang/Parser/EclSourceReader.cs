using System;

namespace ecl.lang.Parser
{
    internal class EclSourceReader
    {
        public EclSourceReader(EclSource source)
        {
            Source = source;
        }

        public EclSourceSpan GetPosition(int start)
        {
            return GetPosition(start, CurrentIndex - start);
        }
        public EclSourceSpan GetPosition(int start, int length)
        {
            if (start < 0 || start >= Source.Content.Length)
                throw new ArgumentOutOfRangeException(nameof(start), "Start index is out of range.");
            if (length < 0 || start + length > Source.Content.Length)
                throw new ArgumentOutOfRangeException(nameof(length), "Length is out of range.");
            return new EclSourceSpan(Source, start, length);
        }
        public EclSource Source { get; }
        public int CurrentIndex { get; set; } = 0;
        public string DebugString => IsEof() ? "EOF" : Source.Content.Substring(CurrentIndex);
        public bool IsEof(int offset = 0) => Source.Content.Length <= offset || CurrentIndex + offset >= Source.Content.Length;
        public char Peek(int offset = 0)
        {
            if (IsEof(offset))
                return '\0';
            return Source.Content[CurrentIndex + offset];
        }
        public void Move(int offset = 1)
        {
            CurrentIndex += offset;
        }
        
        public void Eat(char c)
        {
            if (Is(c))
            {
                Move();
            }
            else
            {
                throw new InvalidOperationException($"Expected '{c}' at index {CurrentIndex}, but found '{Peek()}' instead.");
            }
        }
        
        public void Eat(string str)
        {
            if (Is(str))
            {
                Move(str.Length);
            }
            else
            {
                throw new InvalidOperationException($"Expected '{str}' at index {CurrentIndex}, but found '{Peek()}' instead.");
            }
        }
        
        public bool Is(char c, int offset = 0)
        {
            return Peek(offset) == c;
        }
        public bool Is(string str, int offset = 0)
        {
            if (IsEof(str.Length + offset))
                return false;
            for (int i = 0; i < str.Length; i++)
            {
                if (Peek(offset + i) != str[i])
                    return false;
            }
            return true;
        }

        public EclSourceSpan ReadComment()
        {
            int start;
            Eat('/');
            if (Is('/'))
            {
                //Read until end of line
                Move();
                start = CurrentIndex;
                while(!Is('\r') && !Is('\n') && !IsEof())
                {
                    Move();
                }
                return GetPosition(start);
            }
            
            Eat('*');
            start = CurrentIndex;
            while (!IsEof())
            {
                if (Is('*') && Is('/', 1))
                {
                    var result = GetPosition(start);
                    Move(2); // Skip '*/'
                    return result;
                }
                Move();
            }
            
            throw new InvalidOperationException($"Unterminated comment starting at index {start}.");
        }

        public void SkipWhitespace()
        {
            while(!IsEof() && (char.IsWhiteSpace(Peek()) || Is('\r') || Is('\n') || Is("//") || Is("/*")))
            {
                if(Is("//") || Is("/*"))
                {
                    ReadComment();
                }
                else
                {
                    Move();
                }
            }
        }
    }
}