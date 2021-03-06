﻿using System;
using TwistedLogik.Ultraviolet.Input;
using TwistedLogik.Ultraviolet.UI.Presentation.Input;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Controls.Primitives
{
    /// <summary>
    /// Represents the track for a scroll bar or slider.
    /// </summary>
    [UvmlKnownType]
    public class Track : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public Track(UltravioletContext uv, String name)
            : base(uv, name)
        {
            this.Thumb = new Button(uv, null) 
            { 
                HorizontalAlignment = HorizontalAlignment.Stretch, 
                VerticalAlignment   = VerticalAlignment.Stretch,
                Focusable           = false,
            };
            this.Thumb.Classes.Add("track-thumb");
            this.Thumb.ChangeLogicalAndVisualParents(this, this);
            KeyboardNavigation.SetIsTabStop(this.Thumb, false);

            this.IncreaseButton = new RepeatButton(uv, null) 
            { 
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Stretch,
                Opacity             = 0,
                Focusable           = false,
            };
            this.IncreaseButton.Classes.Add("track-increase");
            this.IncreaseButton.Click += HandleIncreaseButtonClick;
            this.IncreaseButton.ChangeLogicalAndVisualParents(this, this);
            KeyboardNavigation.SetIsTabStop(this.IncreaseButton, false);

            this.DecreaseButton = new RepeatButton(uv, null) 
            { 
                HorizontalAlignment = HorizontalAlignment.Stretch, 
                VerticalAlignment   = VerticalAlignment.Stretch,
                Opacity             = 0,
                Focusable           = false,
            };
            this.DecreaseButton.Classes.Add("track-decrease");
            this.DecreaseButton.Click += HandleDecreaseButtonClick;
            this.DecreaseButton.ChangeLogicalAndVisualParents(this, this);
            KeyboardNavigation.SetIsTabStop(this.DecreaseButton, false);

            Mouse.AddPreviewMouseMoveHandler(this.Thumb, HandleThumbPreviewMouseMove);
            Mouse.AddPreviewMouseDownHandler(this.Thumb, HandleThumbPreviewMouseDown);
            Mouse.AddPreviewMouseUpHandler(this.Thumb, HandleThumbPreviewMouseUp);
        }

        /// <summary>
        /// Gets or sets the track's orientation.
        /// </summary>
        /// <value>A <see cref="Orientation"/> value which indicates whether the track is oriented vertically
        /// or horizontally. The default value is <see cref="Orientation.Horizontal"/>.</value>
        /// <remarks>
        /// <dprop>
        ///		<dpropField><see cref="OrientationProperty"/></dpropField>
        ///		<dpropStylingName>orientation</dpropStylingName>
        ///		<dpropMetadata><see cref="PropertyMetadataOptions.AffectsMeasure"/></dpropMetadata>
        /// </dprop>
        /// </remarks>
        public Orientation Orientation
        {
            get { return GetValue<Orientation>(OrientationProperty); }
            set { SetValue<Orientation>(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the track's associated viewport.
        /// </summary>
        /// <value>A <see cref="Double"/> that represents the size in device independent pixels of the 
        /// track's associated viewport.</value>
        public Double ViewportSize
        {
            get
            {
                var owner = TemplatedParent as ScrollBarBase;
                return (owner == null) ? Double.NaN : owner.ViewportSize;
            }
            set
            {
                var owner = TemplatedParent as ScrollBarBase;
                if (owner != null)
                {
                    owner.ViewportSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the track's minimum value.
        /// </summary>
        /// <value>A <see cref="Double"/> that represents the minimum value of the <see cref="Value"/> property.</value>
        public Double Minimum
        {
            get
            {
                var owner = TemplatedParent as RangeBase;
                return (owner == null) ? 0 : owner.Minimum;
            }
            set
            {
                var owner = TemplatedParent as RangeBase;
                if (owner != null)
                {
                    owner.Minimum = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the track's maximum value.
        /// </summary>
        /// <value>A <see cref="Double"/> that represents the maximum value of the <see cref="Value"/> property.</value>
        public Double Maximum
        {
            get
            {
                var owner = TemplatedParent as RangeBase;
                return (owner == null) ? 0 : owner.Maximum;
            }
            set
            {
                var owner = TemplatedParent as RangeBase;
                if (owner != null)
                {
                    owner.Maximum = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the track's current value.
        /// </summary>
        /// <value>A <see cref="Double"/> that represents the track's current value.</value>
        public Double Value
        {
            get
            {
                var owner = TemplatedParent as RangeBase;
                return (owner == null) ? 0 : owner.Value;
            }
            set
            {
                var owner = TemplatedParent as RangeBase;
                if (owner != null)
                {
                    owner.Value = value;
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="Orientation"/> dependency property.</value>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Track),
            new PropertyMetadata<Orientation>(null, PropertyMetadataOptions.AffectsMeasure));

        /// <inheritdoc/>
        protected internal override Int32 LogicalChildrenCount
        {
            get { return 3; }
        }

        /// <inheritdoc/>
        protected internal override Int32 VisualChildrenCount
        {
            get { return 3; }
        }

        /// <inheritdoc/>
        protected internal override UIElement GetLogicalChild(Int32 childIndex)
        {
            switch (childIndex)
            {
                case 0: return DecreaseButton;
                case 1: return Thumb;
                case 2: return IncreaseButton;
            }
            throw new ArgumentOutOfRangeException("childIndex");
        }

        /// <inheritdoc/>
        protected internal override UIElement GetVisualChild(Int32 childIndex)
        {
            switch (childIndex)
            {
                case 0: return DecreaseButton;
                case 1: return Thumb;
                case 2: return IncreaseButton;
            }
            throw new ArgumentOutOfRangeException("childIndex");
        }

        /// <inheritdoc/>
        protected override Size2D MeasureOverride(Size2D availableSize)
        {
            var desiredSize = Size2D.Zero;

            Thumb.Measure(availableSize);
            desiredSize = Thumb.DesiredSize;

            return desiredSize;
        }

        /// <inheritdoc/>
        protected override Size2D ArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {
            // Calculate the sizes of the track's components.
            var thumbSize          = CalculateThumbSize(finalSize);
            var decreaseButtonSize = CalculateDecreaseButtonSize(finalSize, thumbSize);
            var increaseButtonSize = CalculateIncreaseButtonSize(finalSize, decreaseButtonSize, thumbSize);

            // Arrange the track's components.
            var position    = new Point2D(0, 0);
            var orientation = this.Orientation;

            DecreaseButton.Arrange(new RectangleD(position, decreaseButtonSize));
            position += (orientation == Orientation.Horizontal) ? new Point2D(decreaseButtonSize.Width, 0) : new Point2D(0, decreaseButtonSize.Height);

            Thumb.Arrange(new RectangleD(position, thumbSize));
            position += (orientation == Orientation.Horizontal) ? new Point2D(thumbSize.Width, 0) : new Point2D(0, thumbSize.Height);

            IncreaseButton.Arrange(new RectangleD(position, increaseButtonSize));

            return finalSize;
        }

        /// <summary>
        /// Converts a range value to a pixel offset into the scroll bar's track.
        /// </summary>
        /// <param name="value">The range value to convert.</param>
        /// <param name="thumbSize">The size of the track.</param>
        /// <param name="trackSize">The size of the thumb.</param>
        /// <returns>The converted pixel value.</returns>
        protected Double ValueToOffset(Double value, Double trackSize, Double thumbSize)
        {
            var available = trackSize - thumbSize;

            var min = Minimum;
            var max = Maximum;

            if (max == min)
                return 0;

            var percent = (value - min) / (max - min);
            var used    = available * percent;

            return used;
        }

        /// <summary>
        /// Converts a pixel offset into the scroll bar's track to a range value.
        /// </summary>
        /// <param name="pixels">The pixel value to convert.</param>
        /// <param name="thumbSize">The size of the track.</param>
        /// <param name="trackSize">The size of the thumb.</param>
        /// <returns>The converted range value.</returns>
        protected Double OffsetToValue(Double pixels, Double trackSize, Double thumbSize)
        {
            var available = trackSize - thumbSize;

            var min = Minimum;
            var max = Maximum;

            if (max == min)
                return 0;

            var percent = pixels / available;
            var value   = (percent * (Maximum - Minimum)) + Minimum;

            return value;
        }

        /// <summary>
        /// Calculates the size of the track's thumb.
        /// </summary>
        /// <param name="trackSize">The size of the track.</param>
        /// <returns>The size of the track's thumb.</returns>
        protected Size2D CalculateThumbSize(Size2D trackSize)
        {
            if (Double.IsNaN(ViewportSize))
                return Thumb.DesiredSize;

            var orientation = this.Orientation;
            var max         = Maximum;
            var min         = Minimum;
            var vps         = ViewportSize;

            if (max - min + vps == 0)
            {
                return (orientation == Orientation.Horizontal) ? 
                    new Size2D(0, trackSize.Height) :
                    new Size2D(trackSize.Width, 0);
            }
            else
            {
                var trackLength = (orientation == Orientation.Horizontal) ? trackSize.Width : trackSize.Height;
                var thumbLength = ((vps / (max - min + vps)) * trackLength);

                return (orientation == Orientation.Horizontal) ?
                    new Size2D(Math.Ceiling(thumbLength), trackSize.Height) :
                    new Size2D(trackSize.Width, Math.Ceiling(thumbLength));                    
            }
        }

        /// <summary>
        /// Calculates the size of the track's decrease button.
        /// </summary>
        /// <param name="trackSize">The size of the track.</param>
        /// <param name="thumbSize">The size of the track's thumb.</param>
        /// <returns>The size of the track's decrease button.</returns>
        protected Size2D CalculateDecreaseButtonSize(Size2D trackSize, Size2D thumbSize)
        {
            var orientation = this.Orientation;
            var val         = Value;
            var min         = Minimum;
            var max         = Maximum;

            if (min == max)
            {
                return (orientation == Orientation.Horizontal) ? 
                    new Size2D(0, trackSize.Height) :
                    new Size2D(trackSize.Width, 0);
            }

            var trackLength = (orientation == Orientation.Horizontal) ? trackSize.Width : trackSize.Height;
            var thumbLength = (orientation == Orientation.Horizontal) ? thumbSize.Width : thumbSize.Height;
            var available   = trackLength - thumbLength;
            var percent     = (val - min) / (max - min);
            var used        = available * percent;

            return (orientation == Orientation.Horizontal) ?
                    new Size2D(Math.Max(0, Math.Floor(used)), trackSize.Height) :
                    new Size2D(trackSize.Width, Math.Max(0, Math.Floor(used)));  
        }

        /// <summary>
        /// Calculates the size of the track's increase button.
        /// </summary>
        /// <param name="trackSize">The size of the track.</param>
        /// <param name="decreaseButtonSize">The size of the track's decrease button.</param>
        /// <param name="thumbSize">The size of the track's thumb.</param>
        /// <returns>The size of the track's increase button.</returns>
        protected Size2D CalculateIncreaseButtonSize(Size2D trackSize, Size2D decreaseButtonSize, Size2D thumbSize)
        {
            return (Orientation == Orientation.Horizontal) ? 
               new Size2D(Math.Max(0, trackSize.Width - (decreaseButtonSize.Width + thumbSize.Width)), trackSize.Height) :
               new Size2D(trackSize.Width, Math.Max(0, trackSize.Height - (decreaseButtonSize.Height + thumbSize.Height)));
        }

        /// <summary>
        /// Handles the <see cref="ButtonBase.Click"/> event for the decrease button.
        /// </summary>
        private void HandleDecreaseButtonClick(DependencyObject element, ref RoutedEventData data)
        {
            var owner = TemplatedParent as RangeBase;
            if (owner != null)
            {
                owner.DecreaseLarge();

                var scrollbar = owner as ScrollBarBase;
                if (scrollbar != null)
                {
                    scrollbar.RaiseScrollEvent(ScrollEventType.LargeDecrement);
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="ButtonBase.Click"/> event for the increase button.
        /// </summary>
        private void HandleIncreaseButtonClick(DependencyObject element, ref RoutedEventData data)
        {
            var owner = TemplatedParent as RangeBase;
            if (owner != null)
            {
                owner.IncreaseLarge();

                var scrollbar = owner as ScrollBarBase;
                if (scrollbar != null)
                {
                    scrollbar.RaiseScrollEvent(ScrollEventType.LargeIncrement);
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="Mouse.PreviewMouseMoveEvent"/> routed event for the Thumb button.
        /// </summary>
        private void HandleThumbPreviewMouseMove(DependencyObject element, MouseDevice device, Double x, Double y, Double dx, Double dy, ref RoutedEventData data)
        {
            var button = element as Button;
            if (button != null && button.IsMouseCaptured)
            {
                var relativeMousePosition = Mouse.GetPosition(this);

                var oldValue = Value;
                if (Orientation == Orientation.Vertical)
                {
                    var relY = relativeMousePosition.Y - thumbDragOffset;
                    Value = OffsetToValue(relY, RenderSize.Height, Thumb.RenderSize.Height);
                }
                else
                {
                    var relX = relativeMousePosition.X - thumbDragOffset;
                    Value = OffsetToValue(relX, RenderSize.Width, Thumb.RenderSize.Width);
                }

                if (Value != oldValue)
                {
                    var scrollbar = TemplatedParent as ScrollBarBase;
                    if (scrollbar != null)
                    {
                        scrollbar.RaiseScrollEvent(ScrollEventType.ThumbTrack);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="Mouse.PreviewMouseDownEvent"/> routed event for the Thumb button.
        /// </summary>
        private void HandleThumbPreviewMouseDown(DependencyObject element, MouseDevice device, MouseButton pressed, ref RoutedEventData data)
        {
            var relativeMousePosition = Mouse.GetPosition(Thumb);

            if (Orientation == Orientation.Vertical)
            {
                thumbDragging   = true;
                thumbDragOffset = relativeMousePosition.Y;
            }
            else
            {
                thumbDragging   = true;
                thumbDragOffset = relativeMousePosition.X;
            }
        }
        
        /// <summary>
        /// Handles the <see cref="Mouse.PreviewMouseUpEvent"/> routed event for the Thumb button.
        /// </summary>
        private void HandleThumbPreviewMouseUp(DependencyObject element, MouseDevice device, MouseButton pressed, ref RoutedEventData data)
        {
            if (!thumbDragging)
                return;

            var scrollbar = TemplatedParent as ScrollBarBase;
            if (scrollbar != null)
            {
                scrollbar.RaiseScrollEvent(ScrollEventType.EndScroll);
            }

            thumbDragging = false;
        }

        // Component element references.
        private readonly Button Thumb = null;
        private readonly RepeatButton DecreaseButton = null;
        private readonly RepeatButton IncreaseButton = null;

        // State values.
        private Boolean thumbDragging;
        private Double thumbDragOffset;
    }
}
