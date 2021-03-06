﻿using System;

namespace TwistedLogik.Ultraviolet
{
    /// <summary>
    /// Contains commonly-used boxed values of the Ultraviolet Framework's value types.
    /// </summary>
    public static class UltravioletBoxedValues
    {
        /// <summary>
        /// Contains boxed <see cref="TwistedLogik.Ultraviolet.Color"/> values.
        /// </summary>
        public static class Color
        {
            /// <summary>
            /// The cached box for the value <see cref="TwistedLogik.Ultraviolet.Color.White"/>.
            /// </summary>
            public static readonly Object White = TwistedLogik.Ultraviolet.Color.White;

            /// <summary>
            /// The cached box for the value <see cref="TwistedLogik.Ultraviolet.Color.Black"/>.
            /// </summary>
            public static readonly Object Black = TwistedLogik.Ultraviolet.Color.Black;

            /// <summary>
            /// The cached box for the value <see cref="TwistedLogik.Ultraviolet.Color.Transparent"/>.
            /// </summary>
            public static readonly Object Transparent = TwistedLogik.Ultraviolet.Color.Transparent;
        }

        /// <summary>
        /// Contains boxed <see cref="TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle"/> values.
        /// </summary>
        public static class SpriteFontStyle
        {
            /// <summary>
            /// The cached box for the value <see cref="TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.Regular"/>.
            /// </summary>
            public static readonly Object Regular =
                TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.Regular;

            /// <summary>
            /// The cached box for the value <see cref="TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.Bold"/>.
            /// </summary>
            public static readonly Object Bold =
                TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.Bold;

            /// <summary>
            /// The cached box for the value <see cref="TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.Italic"/>.
            /// </summary>
            public static readonly Object Italic =
                TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.Italic;

            /// <summary>
            /// The cached box for the value <see cref="TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.BoldItalic"/>.
            /// </summary>
            public static readonly Object BoldItalic =
                TwistedLogik.Ultraviolet.Graphics.Graphics2D.SpriteFontStyle.BoldItalic;
        }
    }
}
