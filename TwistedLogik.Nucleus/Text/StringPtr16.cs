﻿using System;
using System.Security;

namespace TwistedLogik.Nucleus.Text
{
    /// <summary>
    /// Represent a pointer to an unmanaged string where each character is 16 bits.
    /// </summary>
    public struct StringPtr16 : IEquatable<StringPtr16>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringPtr16"/> structure from the specified <see langword="null"/>-terminated string.
        /// </summary>
        /// <param name="ptr">A pointer to the <see langword="null"/>-terminated string data.</param>
        [SecurityCritical]
        public StringPtr16(IntPtr ptr)
        {
            this.ptr = ptr;

            var length = 0;
            unsafe
            {
                var p = (ushort*)ptr;
                while (*p++ != 0)
                    length++;
            }

            this.length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPtr16"/> structure.
        /// </summary>
        /// <param name="ptr">A pointer to the string data.</param>
        /// <param name="length">The number of characters in the string data.</param>
        [SecurityCritical]
        public StringPtr16(IntPtr ptr, Int32 length)
        {
            this.ptr = ptr;
            this.length = length;
        }

        /// <summary>
        /// Converts the string pointer to an instance of <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="ptr">The string pointer to convert.</param>
        /// <returns>The converted pointer.</returns>
        [SecuritySafeCritical]
        public static explicit operator IntPtr(StringPtr16 ptr)
        {
            return ptr.ptr;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="ptr1">The first object to compare.</param>
        /// <param name="ptr2">The second object to compare.</param>
        /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
        [SecuritySafeCritical]
        public static Boolean operator ==(StringPtr16 ptr1, StringPtr16 ptr2)
        {
            return ptr1.Equals(ptr2);
        }

        /// <summary>
        /// Determines whether the specified objects are unequal.
        /// </summary>
        /// <param name="ptr1">The first object to compare.</param>
        /// <param name="ptr2">The second object to compare.</param>
        /// <returns><see langword="true"/> if the specified objects are unequal; otherwise, <see langword="false"/>.</returns>
        [SecuritySafeCritical]
        public static Boolean operator !=(StringPtr16 ptr1, StringPtr16 ptr2)
        {
            return !ptr1.Equals(ptr2);
        }

        /// <summary>
        /// Gets the instance's hash code.
        /// </summary>
        /// <returns>The instance's hash code.</returns>
        [SecuritySafeCritical]
        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 29 + ptr.GetHashCode();
                hash = hash * 29 + length.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Converts the object to a human-readable string.
        /// </summary>
        /// <returns>A human-readable string that represents the object.</returns>
        [SecuritySafeCritical]
        public override String ToString()
        {
            unsafe { return new String((char*)ptr, 0, length); }
        }

        /// <summary>
        /// Determines whether the specified object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to compare to this object.</param>
        /// <returns><see langword="true"/> ifthis object is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        [SecuritySafeCritical]
        public override Boolean Equals(Object obj)
        {
            return obj is StringPtr16 && Equals((StringPtr16)obj);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to compare to this object.</param>
        /// <returns><see langword="true"/> ifthis object is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        [SecuritySafeCritical]
        public Boolean Equals(StringPtr16 obj)
        {
            return obj.ptr == this.ptr && obj.length == this.length;
        }

        /// <summary>
        /// Converts the string pointer to a pointer to an unspecified type.
        /// </summary>
        /// <returns>A pointer to the string data.</returns>
        [CLSCompliant(false)]
        [SecuritySafeCritical]
        public unsafe void* ToPointer()
        {
            return ptr.ToPointer();
        }

        /// <summary>
        /// Gets a <see langword="null"/> string pointer.
        /// </summary>
        public static StringPtr16 Zero
        {
            get { return new StringPtr16(); }
        }

        /// <summary>
        /// Gets the character at the specified index within the string.
        /// </summary>
        /// <param name="index">The index of the character to retrieve.</param>
        /// <returns>The character at the specified index within the string.</returns>
        public Char this[Int32 index]
        {
            [SecuritySafeCritical]
            get
            {
                Contract.EnsureRange(index >= 0 && index < length, nameof(index));
                unsafe
                {
                    return *((char*)ptr + index);
                }
            }
        }

        /// <summary>
        /// Gets the length of the string in characters.
        /// </summary>
        public Int32 Length
        {
            get { return length; }
        }

        // Property values.
        [SecurityCritical]
        private readonly IntPtr ptr;
        private readonly Int32 length;
    }
}
