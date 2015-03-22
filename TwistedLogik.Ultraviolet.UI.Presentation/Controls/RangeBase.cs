﻿using System;
using System.ComponentModel;
using TwistedLogik.Nucleus;
using TwistedLogik.Ultraviolet.UI.Presentation.Styles;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Controls
{
    /// <summary>
    /// Represents the base class for controls which represent a value within a specified range.
    /// </summary>
    [DefaultProperty("Value")]
    public abstract class RangeBase : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeBase"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public RangeBase(UltravioletContext uv, String id)
            : base(uv, id)
        {

        }

        /// <summary>
        /// Increases the value of the <see cref="Value"/> property by a small amount.
        /// </summary>
        public void IncreaseSmall()
        {
            Value += SmallChange;
        }

        /// <summary>
        /// Increases the value of the <see cref="Value"/> property by a large amount.
        /// </summary>
        public void IncreaseLarge()
        {
            Value += LargeChange;
        }

        /// <summary>
        /// Decreases the value of the <see cref="Value"/> property by a small amount.
        /// </summary>
        public void DecreaseSmall()
        {
            Value -= SmallChange;
        }

        /// <summary>
        /// Decreases the value of the <see cref="Value"/> property by a large amount.
        /// </summary>
        public void DecreaseLarge()
        {
            Value -= LargeChange;
        }

        /// <summary>
        /// Gets or sets the range control's current value.
        /// </summary>
        public Double Value
        {
            get { return GetValue<Double>(ValueProperty); }
            set { SetValue<Double>(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum allowed value of the <see cref="Value"/> property.
        /// </summary>
        public Double Minimum
        {
            get { return GetValue<Double>(MinimumProperty); }
            set { SetValue<Double>(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum allowed value of the <see cref="Value"/> property.
        /// </summary>
        public Double Maximum
        {
            get { return GetValue<Double>(MaximumProperty); }
            set { SetValue<Double>(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value that is added or removed from the <see cref="Value"/> property
        /// when the range control applies a small change.
        /// </summary>
        public Double SmallChange
        {
            get { return GetValue<Double>(SmallChangeProperty); }
            set { SetValue<Double>(SmallChangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value that is added or removed from the <see cref="Value"/> property
        /// when the range control applies a large change.
        /// </summary>
        public Double LargeChange
        {
            get { return GetValue<Double>(LargeChangeProperty); }
            set { SetValue<Double>(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Value"/> property changes.
        /// </summary>
        public event UpfEventHandler ValueChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="Minimum"/> property changes.
        /// </summary>
        public event UpfEventHandler MinimumChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="Maximum"/> property changes.
        /// </summary>
        public event UpfEventHandler MaximumChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="SmallChange"/> property changes.
        /// </summary>
        public event UpfEventHandler SmallChangeChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="LargeChange"/> property changes.
        /// </summary>
        public event UpfEventHandler LargeChangeChanged;

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        [Styled("value")]
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(RangeBase),
            new PropertyMetadata(HandleValueChanged, new CoerceValueCallback<Double>(CoerceValue)));

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        [Styled("minimum")]
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(Double), typeof(RangeBase),
            new PropertyMetadata(HandleMinimumChanged));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        [Styled("maximum")]
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(Double), typeof(RangeBase),
            new PropertyMetadata(CommonBoxedValues.Double.One, HandleMaximumChanged));

        /// <summary>
        /// Identifies the <see cref="SmallChange"/> dependency property.
        /// </summary>
        [Styled("small-change")]
        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(Double), typeof(RangeBase),
            new PropertyMetadata(0.1, HandleSmallChangeChanged));

        /// <summary>
        /// Identifies the <see cref="LargeChange"/> dependency property.
        /// </summary>
        [Styled("large-change")]
        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(Double), typeof(RangeBase),
            new PropertyMetadata(CommonBoxedValues.Double.One, HandleLargeChangeChanged));

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            var temp = ValueChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="MinimumChanged"/> event.
        /// </summary>
        protected virtual void OnMinimumChanged()
        {
            var temp = MinimumChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="MaximumChanged"/> event.
        /// </summary>
        protected virtual void OnMaximumChanged()
        {
            var temp = MaximumChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="SmallChangeChanged"/> event.
        /// </summary>
        protected virtual void OnSmallChangeChanged()
        {
            var temp = SmallChangeChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="LargeChangeChanged"/> event.
        /// </summary>
        protected virtual void OnLargeChangeChanged()
        {
            var temp = LargeChangeChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Value"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The object that raised the event.</param>
        private static void HandleValueChanged(DependencyObject dobj)
        {
            var range = (RangeBase)dobj;
            range.OnValueChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Minimum"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The object that raised the event.</param>
        private static void HandleMinimumChanged(DependencyObject dobj)
        {
            var range = (RangeBase)dobj;
            range.CoerceValue(ValueProperty);
            range.CoerceValue(MaximumProperty);
            range.OnMinimumChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Maximum"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The object that raised the event.</param>
        private static void HandleMaximumChanged(DependencyObject dobj)
        {
            var range = (RangeBase)dobj;
            range.CoerceValue(ValueProperty);
            range.OnMaximumChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="SmallChange"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The object that raised the event.</param>
        private static void HandleSmallChangeChanged(DependencyObject dobj)
        {
            var range = (RangeBase)dobj;
            range.OnSmallChangeChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="LargeChange"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The object that raised the event.</param>
        private static void HandleLargeChangeChanged(DependencyObject dobj)
        {
            var range = (RangeBase)dobj;
            range.OnLargeChangeChanged();
        }

        /// <summary>
        /// Coerces the <see cref="Value"/> property so that it remains within
        /// the range specified by <see cref="Minimum"/> and <see cref="Maximum"/>.
        /// </summary>
        private static Double CoerceValue(DependencyObject dobj, Double value)
        {
            var rangeBase = (RangeBase)dobj;
            
            var min = rangeBase.Minimum;
            if (min > value)
                return min;
            
            var max = rangeBase.Maximum;
            if (max < value)
                return max;

            return value;
        }

        /// <summary>
        /// Coerces the <see cref="Maximum"/> property so that it remains larger
        /// than the <see cref="Minimum"/> property.
        /// </summary>
        private static Double CoerceMaximum(DependencyObject dobj, Double value)
        {
            var rangeBase = (RangeBase)dobj;

            var min = rangeBase.Minimum;
            if (min > value)
                return min;

            return value;
        }
    }
}
