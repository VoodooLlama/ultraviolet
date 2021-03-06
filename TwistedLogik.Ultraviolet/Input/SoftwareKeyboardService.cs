﻿using System;

namespace TwistedLogik.Ultraviolet.Input
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="SoftwareKeyboardService"/> class.
    /// </summary>
    /// <returns>The instance of <see cref="SoftwareKeyboardService"/> that was created.</returns>
    public delegate SoftwareKeyboardService SoftwareKeyboardServiceFactory();

    /// <summary>
    /// Represents a service which controls the software keyboard, if one is available on the current platform.
    /// </summary>
    public abstract class SoftwareKeyboardService
    {
        /// <summary>
        /// Shows the software keyboard, if one is available.
        /// </summary>
        /// <param name="mode">The display mode of the software keyboard.</param>
        /// <returns><see langword="true"/> if the software keyboard was shown; otherwise, <see langword="false"/>.</returns>
        public abstract Boolean ShowSoftwareKeyboard(KeyboardMode mode);

        /// <summary>
        /// Hides the software keyboard.
        /// </summary>
        /// <returns><see langword="true"/> if the software keyboard was hidden; otherwise, false.</returns>
        public abstract Boolean HideSoftwareKeyboard();
    }
}
