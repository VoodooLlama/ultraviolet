﻿using System;
using TwistedLogik.Nucleus;
using TwistedLogik.Ultraviolet.Graphics;

namespace TwistedLogik.Ultraviolet.UI.Presentation
{
    /// <summary>
    /// Represents one of the render targets used to render an out-of-band element.
    /// </summary>
    public class OutOfBandRenderTarget : UltravioletResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutOfBandRenderTarget"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        internal OutOfBandRenderTarget(UltravioletContext uv)
            : base(uv)
        {
            renderTarget = RenderTarget2D.Create(1, 1);

            colorBuffer = RenderBuffer2D.Create(RenderBufferFormat.Color, 1, 1, RenderBufferOptions.None);
            renderTarget.Attach(colorBuffer);

            if (uv.GetGraphics().Capabilities.SupportsDepthStencilTextures)
            {
                depthBuffer = RenderBuffer2D.Create(RenderBufferFormat.Depth24Stencil8, 1, 1, RenderBufferOptions.WillNotBeSampled);
                renderTarget.Attach(depthBuffer);
            }
            else
            {
                depthBuffer = RenderBuffer2D.Create(RenderBufferFormat.Depth16, 1, 1, RenderBufferOptions.WillNotBeSampled);
                renderTarget.Attach(depthBuffer);

                stencilBuffer = RenderBuffer2D.Create(RenderBufferFormat.Stencil8, 1, 1, RenderBufferOptions.WillNotBeSampled);
                renderTarget.Attach(stencilBuffer);
            }
        }

        /// <summary>
        /// Resizes the render target.
        /// </summary>
        /// <param name="width">The render target's new width in pixels.</param>
        /// <param name="height">The render target's new height in pixels.</param>
        public void Resize(Int32 width, Int32 height)
        {
            Contract.EnsureRange(width >= 1, nameof(width));
            Contract.EnsureRange(height >= 1, nameof(height));
            Contract.EnsureNotDisposed(this, Disposed);
            
            this.renderTarget.Resize(width, height);
        }

        /// <summary>
        /// Resizes the render target to match the specified element.
        /// </summary>
        /// <param name="element">The element for which to resize the render target.</param>
        /// <param name="bounds">The visual bounds of the element in absolute screen coordinates.</param>
        /// <returns><see langword="true"/> if the element was resized; otherwise, <see langword="false"/>.</returns>
        public Boolean ResizeForElement(UIElement element, out RectangleD bounds)
        {
            Contract.Require(element, nameof(element));

            if (element.View == null)
            {
                bounds = RectangleD.Empty;
                return false;
            }

            bounds = element.TransformedVisualBounds;

            var effect = element.Effect;
            if (effect != null)
            {
                effect.ModifyVisualBounds(ref bounds);
            }

            var display = element.View.Display;
            var width   = Math.Max(1, (Int32)Math.Ceiling(display.DipsToPixels(bounds.Width)));
            var height  = Math.Max(1, (Int32)Math.Ceiling(display.DipsToPixels(bounds.Height)));

            if (width == renderTarget.Width && height == renderTarget.Height)
                return false;

            if (width < 1 || height < 1)
            {
                Resize(1, 1);
                bounds = RectangleD.Empty;
            }
            else
            {
                Resize(width, height);
            }

            return true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the render target is ready to be rendered.
        /// </summary>
        public Boolean IsReady
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return isReady;
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                isReady = value;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the render target has a dedicated stencil buffer.
        /// </summary>
        public Boolean HasDedicatedStencilBuffer
        {
            get { return stencilBuffer != null; }
        }

        /// <summary>
        /// Gets the cumulative transform of all ancestors of the rendered element.
        /// </summary>
        public Matrix VisualTransform
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the width of the render target in pixels.
        /// </summary>
        public Int32 Width
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return renderTarget.Width;
            }
        }

        /// <summary>
        /// Gets the height of the render target in pixels.
        /// </summary>
        public Int32 Height
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return renderTarget.Height;
            }
        }

        /// <summary>
        /// Gets the transformed visual bounds of the elements contained by this buffer in absolute screen space.
        /// </summary>
        public RectangleD VisualBounds
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="RenderTarget2D"/> used to render this out-of-band element.
        /// </summary>
        public RenderTarget2D RenderTarget
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return renderTarget;
            }
        }

        /// <summary>
        /// Gets the <see cref="RenderBuffer2D"/> that represents this element's color buffer.
        /// </summary>
        public RenderBuffer2D ColorBuffer
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return colorBuffer;
            }
        }

        /// <summary>
        /// Gets the <see cref="RenderBuffer2D"/> that represents this element's depth buffer (which may have stencil components).
        /// </summary>
        public RenderBuffer2D DepthBuffer
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return depthBuffer;
            }
        }

        /// <summary>
        /// Gets the <see cref="RenderBuffer2D"/> that represents thie element's dedicated stencil buffer, if it has one.
        /// </summary>
        public RenderBuffer2D StencilBuffer
        {
            get
            {
                return stencilBuffer;
            }
        }

        /// <summary>
        /// Gets the next target in the render target chain.
        /// </summary>
        public OutOfBandRenderTarget Next
        {
            get { return NextInternal == null ? null : NextInternal.Value; }
        }

        /// <summary>
        /// Gets the next target in the render target chain.
        /// </summary>
        internal UpfPool<OutOfBandRenderTarget>.PooledObject NextInternal
        {
            get;
            set;
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.Dispose(renderTarget);
                SafeDispose.Dispose(colorBuffer);
                SafeDispose.Dispose(depthBuffer);
                SafeDispose.Dispose(stencilBuffer);
            }
            base.Dispose(disposing);
        }

        // Property values.
        private Boolean isReady;
        private readonly RenderTarget2D renderTarget;
        private readonly RenderBuffer2D colorBuffer;
        private readonly RenderBuffer2D depthBuffer;
        private readonly RenderBuffer2D stencilBuffer;
    }
}