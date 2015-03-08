﻿using System;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Elements
{
    /// <summary>
    /// Represents an element which is used to indicate the position of child content within a component template.
    /// </summary>
    [UvmlKnownType]
    public class ContentPresenter : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenter"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="id">The element's unique identifier within its view.</param>
        public ContentPresenter(UltravioletContext uv, String id)
            : base(uv, id)
        {

        }

        /// <inheritdoc/>
        protected override void DrawOverride(UltravioletTime time, DrawingContext dc)
        {
            if (Control != null)
            {
                Control.OnContentPresenterDraw(time, dc);
            }
            base.DrawOverride(time, dc);
        }

        /// <inheritdoc/>
        protected override void UpdateOverride(UltravioletTime time)
        {
            if (Control != null)
            {
                Control.OnContentPresenterUpdate(time);
            }
            base.UpdateOverride(time);
        }

        /// <inheritdoc/>
        protected override Size2D MeasureOverride(Size2D availableSize)
        {
            if (Control == null)
                return Size2D.Zero;

            return Control.OnContentPresenterMeasure(availableSize);
        }

        /// <inheritdoc/>
        protected override Size2D ArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {
            if (Control == null)
                return Size2D.Zero;

            return Control.OnContentPresenterArrange(finalSize, options);
        }

        /// <inheritdoc/>
        protected override void PositionOverride(Point2D position)
        {
            if (Control != null)
            {
                Control.OnContentPresenterPosition(AbsolutePosition);
            }
            base.PositionOverride(position);
        }
    }
}
