﻿using System;
using TwistedLogik.Ultraviolet.Platform;

namespace TwistedLogik.Ultraviolet.Desktop.Platform
{
    /// <summary>
    /// Represents an implementation of the <see cref="ScreenDensityService"/> class for desktop platforms.
    /// </summary>
    public sealed partial class DesktopScreenDensityService : ScreenDensityService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopScreenDensityService"/> class.
        /// </summary>
        /// <param name="display">The <see cref="IUltravioletDisplay"/> for which to retrieve density information.</param>
        public DesktopScreenDensityService(UltravioletContext uv, IUltravioletDisplay display)
            : base(display)
        {
            if (InitWindows8_1(uv, display))
                return;

            if (InitOSX(uv, display))
                return;

            InitFallback(uv, display);
        }

        /// <summary>
        /// Retrieves DPI information when running on Windows 8.1 and higher.
        /// </summary>
        private Boolean InitWindows8_1(UltravioletContext uv, IUltravioletDisplay display)
        {
            if (uv.Platform != UltravioletPlatform.Windows || Environment.OSVersion.Version < new Version(6, 3))
                return false;

            var rect = new Win32.RECT
            {
                left   = display.Bounds.Left,
                top    = display.Bounds.Top,
                right  = display.Bounds.Right,
                bottom = display.Bounds.Bottom 
            };

            var hmonitor = IntPtr.Zero;

            unsafe
            {
                Win32.EnumDisplayMonitors(IntPtr.Zero, &rect, (hdc, lprcClip, lprcMonitor, dwData) =>
                {
                    hmonitor = hdc;
                    return false;
                }, IntPtr.Zero);
            }

            if (hmonitor == IntPtr.Zero)
                return false;

            UInt32 x, y;
            Win32.GetDpiForMonitor(hmonitor, 0, out x, out y);

            this.densityX      = x;
            this.densityY      = y;
            this.densityScale  = x / 96f;
            this.densityBucket = GuessBucketFromDensityScale(densityScale);

            return true;
        }

        /// <summary>
        /// Retrieves DPI information on OS X when the Mac-specific compatibility shim is missing.
        /// </summary>
        private Boolean InitOSX(UltravioletContext uv, IUltravioletDisplay display)
        {
            if (uv.Platform != UltravioletPlatform.OSX)
                return false;

            this.densityX     = 72f;
            this.densityY     = 72f;
            this.densityScale = 1.0f;

            return true;
        }

        /// <summary>
        /// Retrieves DPI information in the general case.
        /// </summary>
        private Boolean InitFallback(UltravioletContext uv, IUltravioletDisplay display)
        {
            using (var graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                this.densityX      = graphics.DpiX;
                this.densityY      = graphics.DpiY;
                this.densityScale  = graphics.DpiX / 96f;
                this.densityBucket = GuessBucketFromDensityScale(densityScale);
            }
            return true;
        }

        /// <inheritdoc/>
        public override Single DensityScale
        {
            get { return densityScale; }
        }

        /// <inheritdoc/>
        public override Single DensityX
        {
            get { return densityX; }
        }

        /// <inheritdoc/>
        public override Single DensityY
        {
            get { return densityY; }
        }

        /// <inheritdoc/>
        public override ScreenDensityBucket DensityBucket
        {
            get { return densityBucket; }
        }

        /// <summary>
        /// Attempts to guess at the appropriate <see cref="ScreenDensityBucket"/> for the specified density scale.
        /// </summary>
        private static ScreenDensityBucket GuessBucketFromDensityScale(Single scale)
        {
            if (scale >= 6f)
                return ScreenDensityBucket.ExtraExtraExtraHigh;

            if (scale >= 5f)
                return ScreenDensityBucket.ExtraExtraHigh;

            if (scale >= 3f)
                return ScreenDensityBucket.ExtraHigh;

            if (scale >= 2.5f)
                return ScreenDensityBucket.High;

            if (scale >= 1.5f)
                return ScreenDensityBucket.Medium;

            if (scale >= 1.25f)
                return ScreenDensityBucket.Low;

            return ScreenDensityBucket.Desktop;
        }

        // State values.
        private Single densityX;
        private Single densityY;
        private Single densityScale;
        private ScreenDensityBucket densityBucket;
    }
}
