﻿using System;

namespace TwistedLogik.Ultraviolet.Graphics
{
    /// <summary>
    /// Encapsulates the capabilities of a graphics device.
    /// </summary>
    public abstract class GraphicsCapabilities
    {
        /// <summary>
        /// Gets a value indicating whether the device supports texture which have both depth and stencil components.
        /// </summary>
        public abstract Boolean SupportsDepthStencilTextures { get; }

        /// <summary>
        /// Gets a value indicating whether the device supports instanced rendering.
        /// </summary>
        public abstract Boolean SupportsInstancedRendering { get; }

        /// <summary>
        /// Gets a value indicating whether the device supports using a non-zero base instance
        /// when performing instanced rendering.
        /// </summary>
        public abstract Boolean SupportsNonZeroBaseInstance { get; }

        /// <summary>
        /// Gets a value indicating whether the current platform has in-hardware support for
        /// preserving the content of a render target after it has been unbound for rendering.
        /// </summary>
        public abstract Boolean SupportsPreservingRenderTargetContentInHardware { get; }
        
        /// <summary>
        /// Gets a value indicating whether the current device provides support for sampler state
        /// which is independent from texture state. If this value is false, every sampler to which
        /// a particular texture is bound must have the same sampler state.
        /// </summary>
        public abstract Boolean SupportsIndependentSamplerState { get; }

        /// <summary>
        /// Gets the maximum texture size supported by the device.
        /// </summary>
        public abstract Int32 MaximumTextureSize { get; }

        /// <summary>
        /// Gets the maximum supported viewport width.
        /// </summary>
        public abstract Int32 MaximumViewportWidth { get; }

        /// <summary>
        /// Gets the maximum supported viewport height.
        /// </summary>
        public abstract Int32 MaximumViewportHeight { get; }
    }
}
