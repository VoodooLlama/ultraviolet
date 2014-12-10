﻿using System;
using TwistedLogik.Nucleus.Data;

namespace TwistedLogik.Ultraviolet.Layout
{
    /// <summary>
    /// Represents a value which is bound to a dependency property and which requires
    /// special conversion logic.
    /// </summary>
    /// <typeparam name="TDependency">The type of the dependency property.</typeparam>
    /// <typeparam name="TBound">The type of the bound property.</typeparam>
    internal sealed class DependencyBoundValueConverting<TDependency, TBound> : DependencyBoundValue<TBound>, IDependencyBoundValue<TDependency>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyBoundValueConverting{TDependency, TBound}"/> class.
        /// </summary>
        /// <param name="value">The dependency property value which created this object.</param>
        /// <param name="expressionType">The type of the bound expression.</param>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <param name="expression">The binding expression.</param>
        public DependencyBoundValueConverting(IDependencyPropertyValue value, Type expressionType, Type viewModelType, String expression)
            : base(value, expressionType, viewModelType, expression)
        {

        }

        /// <inheritdoc/>
        public TDependency Get()
        {
            return cachedConvertedValue;
        }

        /// <inheritdoc/>
        public TDependency GetFresh()
        {
            var value = GetUnderlyingValue();
            return ConvertBoundValue(value);
        }

        /// <inheritdoc/>
        public void Set(TDependency value)
        {
            var converted = (TBound)ConvertValue(value, typeof(TDependency), typeof(TBound));
            SetUnderlyingValue(converted);
        }

        /// <inheritdoc/>
        protected override void OnCachedValueChanged(TBound value)
        {
            cachedConvertedValue = ConvertBoundValue(value);
        }

        /// <summary>
        /// Converts a value to the specified type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="originalType">The value's original type.</param>
        /// <param name="conversionType">The type to which to convert the value.</param>
        /// <returns>The converted value.</returns>
        private static Object ConvertValue(Object value, Type originalType, Type conversionType)
        {
            if (conversionType == typeof(String))
            {
                return (value == null) ? null : value.ToString();
            }
            if (originalType == typeof(String))
            {
                try
                {
                    return ObjectResolver.FromString((String)value, conversionType);
                }
                catch (FormatException) { return conversionType.IsClass ? null : Activator.CreateInstance(conversionType); }
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// Converts a bound value to the type of the dependency property.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        private TDependency ConvertBoundValue(TBound value)
        {
            return (TDependency)ConvertValue(value, typeof(TBound), typeof(TDependency));
        }

        // State values.
        private TDependency cachedConvertedValue;
    }
}
