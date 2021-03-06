﻿using System;
using System.Text;

namespace TwistedLogik.Nucleus.Text
{
    /// <summary>
    /// Represents a segment of a string.
    /// </summary>
    public struct StringSegment : IEquatable<StringSegment>, IEquatable<String>, IEquatable<StringBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSegment"/> structure.
        /// </summary>
        /// <param name="source">The <see cref="SourceString"/> that represents the segment.</param>
        public StringSegment(String source)
        {
            Contract.Require(source, nameof(source));

            this.sourceString  = source;
            this.sourceBuilder = null;
            this.start         = 0;
            this.length        = source.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSegment"/> structure.
        /// </summary>
        /// <param name="source">The <see cref="SourceStringBuilder"/> that represents the segment.</param>
        public StringSegment(StringBuilder source)
        {
            Contract.Require(source, nameof(source));

            this.sourceString  = null;
            this.sourceBuilder = source;
            this.start         = 0;
            this.length        = source.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSegment"/> structure.
        /// </summary>
        /// <param name="source">The source <see cref="String"/> that contains this segment.</param>
        /// <param name="start">The index of the string segment's first character within its parent string.</param>
        /// <param name="length">The number of characters in the string segment.</param>
        public StringSegment(String source, Int32 start, Int32 length)
        {
            Contract.Require(source, nameof(source));
            Contract.EnsureRange(start >= 0 && start <= source.Length, nameof(start));
            Contract.EnsureRange(length >= 0 && start + length <= source.Length, nameof(length));

            this.sourceString  = source;
            this.sourceBuilder = null;
            this.start         = start;
            this.length        = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSegment"/> structure.
        /// </summary>
        /// <param name="source">The source <see cref="StringBuilder"/> that contains this segment.</param>
        /// <param name="start">The index of the string segment's first character within its parent string.</param>
        /// <param name="length">The number of characters in the string segment.</param>
        public StringSegment(StringBuilder source, Int32 start, Int32 length)
        {
            Contract.Require(source, nameof(source));
            Contract.EnsureRange(start >= 0 && start <= source.Length, nameof(start));
            Contract.EnsureRange(length >= 0 && start + length <= source.Length, nameof(length));

            this.sourceString  = null;
            this.sourceBuilder = source;
            this.start         = start;
            this.length        = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSegment"/> structure.
        /// </summary>
        /// <param name="source">The source <see cref="StringSegment"/> that contains this segment.</param>
        /// <param name="start">The index of the string segment's first character within its parent string.</param>
        /// <param name="length">The number of characters in the string segment.</param>
        public StringSegment(StringSegment source, Int32 start, Int32 length)
        {
            Contract.EnsureRange(start >= 0 && start <= source.Length, nameof(start));
            Contract.EnsureRange(length >= 0 && start + length <= source.Length, nameof(length));

            this.sourceString = source.sourceString;
            this.sourceBuilder = source.sourceBuilder;
            this.start = source.start + start;
            this.length = length;
        }

        /// <summary>
        /// Implicitly converts a <see cref="String"/> to a string segment.
        /// </summary>
        /// <param name="s">The <see cref="String"/> to convert.</param>
        /// <returns>The converted <see cref="StringSegment"/>.</returns>
        public static implicit operator StringSegment(String s)
        {
            return (s == null) ? Empty : new StringSegment(s);
        }
        
        /// <summary>
        /// Explicitly converts a <see cref="StringBuilder"/> to a string segment.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to convert.</param>
        /// <returns>The converted <see cref="StringSegment"/>.</returns>
        public static explicit operator StringSegment(StringBuilder sb)
        {
            return (sb == null) ? Empty : new StringSegment(sb);
        }

        /// <summary>
        /// Compares two string segments for equality.
        /// </summary>
        /// <param name="s1">The first <see cref="StringSegment"/>.</param>
        /// <param name="s2">The second <see cref="StringSegment"/>.</param>
        /// <returns><see langword="true"/> if the two string segments are equal; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator ==(StringSegment s1, StringSegment s2)
        {
            return s1.Equals(s2);
        }

        /// <summary>
        /// Compares two string segments for inequality.
        /// </summary>
        /// <param name="s1">The first <see cref="StringSegment"/>.</param>
        /// <param name="s2">The second <see cref="StringSegment"/>.</param>
        /// <returns><see langword="true"/> if the two string segments are unequal; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator !=(StringSegment s1, StringSegment s2)
        {
            return !s1.Equals(s2);
        }

        /// <summary>
        /// Compares a string segment and a string for equality.
        /// </summary>
        /// <param name="s1">The <see cref="StringSegment"/> to compare.</param>
        /// <param name="s2">The <see cref="SourceString"/> to compare.</param>
        /// <returns><see langword="true"/> if the string segment is equal to the string; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator ==(StringSegment s1, String s2)
        {
            return s1.Equals(s2);
        }

        /// <summary>
        /// Compares a string segment and a string for inequality.
        /// </summary>
        /// <param name="s1">The <see cref="StringSegment"/> to compare.</param>
        /// <param name="s2">The <see cref="SourceString"/> to compare.</param>
        /// <returns><see langword="true"/> if the string segment is not equal to the string; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator !=(StringSegment s1, String s2)
        {
            return !s1.Equals(s2);
        }

        /// <summary>
        /// Compares a string segment and a string builder for equality.
        /// </summary>
        /// <param name="s1">The <see cref="StringSegment"/> to compare.</param>
        /// <param name="s2">The <see cref="SourceStringBuilder"/> to compare.</param>
        /// <returns><see langword="true"/> if the string segment is equal to the string builder; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator ==(StringSegment s1, StringBuilder s2)
        {
            return s1.Equals(s2);
        }

        /// <summary>
        /// Compares a string segment and a string builder for inequality.
        /// </summary>
        /// <param name="s1">The <see cref="StringSegment"/> to compare.</param>
        /// <param name="s2">The <see cref="SourceStringBuilder"/> to compare.</param>
        /// <returns><see langword="true"/> if the string segment is not equal to the string builder; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator !=(StringSegment s1, StringBuilder s2)
        {
            return !s1.Equals(s2);
        }

        /// <summary>
        /// Gets a value indicating whether the specified string segments are contiguous.
        /// </summary>
        /// <param name="s1">The first <see cref="StringSegment"/>.</param>
        /// <param name="s2">The second <see cref="StringSegment"/>.</param>
        /// <returns><see langword="true"/> if the string segments are contiguous; otherwise, <see langword="false"/>.</returns>
        public static Boolean AreSegmentsContiguous(StringSegment s1, StringSegment s2)
        {
            if (s1.sourceString != null)
            {
                return (s1.sourceString == s2.sourceString) &&
                    s1.Start + s1.Length == s2.Start ||
                    s2.Start + s2.Length == s1.Start;

            }
            return (s1.sourceBuilder == s2.sourceBuilder) &&
                s1.Start + s1.Length == s2.Start ||
                s2.Start + s2.Length == s1.Start;
        }

        /// <summary>
        /// Combines two contiguous string segments.
        /// </summary>
        /// <param name="s1">The first <see cref="StringSegment"/> to combine.</param>
        /// <param name="s2">The second <see cref="StringSegment"/> to combine.</param>
        /// <returns>The combined string segment.</returns>
        public static StringSegment CombineSegments(StringSegment s1, StringSegment s2)
        {
            if (!AreSegmentsContiguous(s1, s2))
            {
                throw new InvalidOperationException(NucleusStrings.SegmentsAreNotContiguous);
            }

            return (s1.sourceString != null) ?
                new StringSegment(s1.sourceString, s1.Start, s1.Length + s2.Length) :                
                new StringSegment(s1.sourceBuilder, s1.Start, s1.Length + s2.Length);
        }

        /// <summary>
        /// Creates a new <see cref="StringSegment"/> from the same source as the specified segment.
        /// </summary>
        /// <param name="segment">The segment from which to retrieve a string source.</param>
        /// <param name="start">The index of the string segment's first character within its parent string.</param>
        /// <param name="length">The number of characters in the string segment.</param>
        /// <returns>The string segment that was created.</returns>
        public static StringSegment FromSource(StringSegment segment, Int32 start, Int32 length)
        {
            if (segment.sourceString != null)
                return new StringSegment(segment.sourceString, start, length);

            if (segment.sourceBuilder != null)
                return new StringSegment(segment.sourceBuilder, start, length);

            if (start != 0)
                throw new ArgumentOutOfRangeException("start");

            if (length != 0)
                throw new ArgumentOutOfRangeException("length");

            return Empty;
        }

        /// <summary>
        /// Converts the object to a human-readable string.
        /// </summary>
        /// <returns>A human-readable string that represents the object.</returns>
        public override String ToString()
        {
            if (sourceBuilder != null)
            {
                return sourceBuilder.ToString(Start, Length);
            }
            if (sourceString != null)
            {
                return sourceString.Substring(Start, Length);
            }
            return null;
        }

        /// <summary>
        /// Gets the object's hash code.
        /// </summary>
        /// <returns>The object's hash code.</returns>
        public override Int32 GetHashCode()
        {
            if ((sourceString == null && sourceBuilder == null) || length == 0) 
                return 0;

            unchecked
            {
                var hash = 17;
                for (int i = 0; i < length; i++)
                {
                    hash = hash * 31 + this[i].GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this string segment is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this segment.</param>
        /// <returns><see langword="true"/> ifthis string segment is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        public override Boolean Equals(Object obj)
        {
            if (obj is StringSegment) return Equals((StringSegment)obj);
            if (obj is String) return Equals((String)obj);
            if (obj is StringBuilder) return Equals((StringBuilder)obj);
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the content of this string segment equals
        /// the content of the specified string segment.
        /// </summary>
        /// <param name="other">The <see cref="StringSegment"/> to compare to this segment.</param>
        /// <returns><see langword="true"/> if the content of this segment equals the content of the 
        /// specified string segment; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(StringSegment other)
        {
            if (IsEmpty)
                return other.IsEmpty;
            if (other.IsEmpty)
                return IsEmpty;

            if (other.length != length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the content of this string segment equals
        /// the content of the specified string.
        /// </summary>
        /// <param name="other">The <see cref="SourceString"/> to compare to this segment.</param>
        /// <returns><see langword="true"/> if the content of this segment equals the content of the 
        /// specified string; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(String other)
        {
            if (IsEmpty && other == String.Empty)
                return true;

            if (other == null || other.Length != length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the content of this string segment equals
        /// the content of the specified string builder.
        /// </summary>
        /// <param name="other">The <see cref="SourceStringBuilder"/> to compare to this segment.</param>
        /// <returns><see langword="true"/> if the content of this segment equals the content of the 
        /// specified string builder; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(StringBuilder other)
        {
            if (IsEmpty && other.Length == 0)
                return true;

            if (other == null || other.Length != length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a string segment which is a substring of this string segment.
        /// </summary>
        /// <param name="start">The starting character of the substring within this string segment.</param>
        /// <returns>A <see cref="StringSegment"/> which is a substring of this string segment.</returns>
        public StringSegment Substring(Int32 start)
        {
            Contract.EnsureRange(start >= 0 && start < this.length, nameof(start));
            Contract.EnsureNot(IsEmpty, NucleusStrings.SegmentIsEmpty);

            var substringLength = (length - start);

            return (sourceString == null) ? 
                new StringSegment(sourceBuilder, this.start + start, substringLength) :
                new StringSegment(sourceString, this.start + start, substringLength);
        }

        /// <summary>
        /// Creates a string segment which is a substring of this string segment.
        /// </summary>
        /// <param name="start">The starting character of the substring within this string segment.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <returns>A <see cref="StringSegment"/> which is a substring of this string segment.</returns>
        public StringSegment Substring(Int32 start, Int32 length)
        {
            Contract.EnsureRange(start >= 0 && start < this.length, nameof(start));
            Contract.EnsureRange(length > 0 && start + length <= this.length, nameof(length));
            Contract.EnsureNot(IsEmpty, NucleusStrings.SegmentIsEmpty);

            return (sourceString == null) ? 
                new StringSegment(sourceBuilder, this.start + start, length) :
                new StringSegment(sourceString, this.start + start, length);
        }

        /// <summary>
        /// Gets the index of the first occurrence of the specified character within the string segment.
        /// </summary>
        /// <param name="value">The character for which to search.</param>
        /// <returns>The index of the first occurrence of the specified character, or -1 if the string segment does not contain the character.</returns>
        public Int32 IndexOf(Char value)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this[i] == value)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the first occurrence of the specified string within the string segment.
        /// </summary>
        /// <param name="value">The string for which to search.</param>
        /// <returns>The index of the first occurrence of the specified string, or -1 if the string segment does not contain the string.</returns>
        public Int32 IndexOf(String value)
        {
            Contract.Require(value, nameof(value));

            for (int i = 0; i < Length; i++)
            {
                var matches = 0;

                for (int j = 0; j < value.Length; j++)
                {
                    if (this[i + j] != value[j])
                        break;

                    matches++;
                }

                if (matches == value.Length)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Gets the character at the specified index within the string segment.
        /// </summary>
        /// <param name="ix">The index of the character to retrieve.</param>
        /// <returns>The character at the specified index.</returns>
        public Char this[Int32 ix]
        {
            get 
            {
                Contract.EnsureRange(ix >= 0 && ix < length, nameof(ix));

                return (sourceString == null) ? sourceBuilder[start + ix] : sourceString[start + ix];
            }
        }

        /// <summary>
        /// Gets the <see cref="SourceString"/> that contains this string segment.
        /// </summary>
        public String SourceString
        {
            get { return sourceString; }
        }

        /// <summary>
        /// Gets the <see cref="SourceStringBuilder"/> that contains this string segment.
        /// </summary>
        public StringBuilder SourceStringBuilder
        {
            get { return sourceBuilder; }
        }

        /// <summary>
        /// Gets the index of the string segment's first character within its parent string.
        /// </summary>
        public Int32 Start
        {
            get { return start; }
        }

        /// <summary>
        /// Gets the number of characters in the string segment.
        /// </summary>
        public Int32 Length
        {
            get { return length; }
        }

        /// <summary>
        /// Gets the number of characters in the segment's source string.
        /// </summary>
        public Int32 SourceLength
        {
            get
            {
                if (sourceString != null)
                {
                    return sourceString.Length;
                }
                if (sourceBuilder != null)
                {
                    return sourceBuilder.Length;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is an empty string segment.
        /// </summary>
        public Boolean IsEmpty
        {
            get { return length == 0; }
        }

        /// <summary>
        /// Represents an empty string segment.
        /// </summary>
        public static readonly StringSegment Empty = new StringSegment();

        // Property values.
        private readonly String sourceString;
        private readonly StringBuilder sourceBuilder;
        private readonly Int32 start;
        private readonly Int32 length;
    }
}
