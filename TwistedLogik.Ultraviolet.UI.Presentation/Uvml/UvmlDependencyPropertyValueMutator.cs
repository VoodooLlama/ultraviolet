﻿using System;
using System.Linq;
using System.Reflection;
using TwistedLogik.Nucleus;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Uvml
{
    /// <summary>
    /// Represents a UVML mutator which sets a dependency property value.
    /// </summary>
    internal sealed class UvmlDependencyPropertyValueMutator : UvmlMutator
    {
        /// <summary>
        /// Initializes the <see cref="UvmlDependencyPropertyValueMutator"/> type.
        /// </summary>
        static UvmlDependencyPropertyValueMutator()
        {
            miSetLocalValue = typeof(DependencyObject).GetMethods()
                .Where(x => String.Equals(x.Name, nameof(DependencyObject.SetLocalValue), StringComparison.Ordinal)).Single();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UvmlDependencyPropertyValueMutator"/> class.
        /// </summary>
        /// <param name="dpropID">The property which is being mutated.</param>
        /// <param name="dpropValue">The value to set on the property.</param>
        public UvmlDependencyPropertyValueMutator(DependencyProperty dpropID, UvmlNode dpropValue)
        {
            Contract.Require(dpropID, nameof(dpropID));
            Contract.Require(dpropValue, nameof(dpropValue));

            this.dpropID = dpropID;
            this.dpropValue = dpropValue;
            this.dpropSetter = miSetLocalValue.MakeGenericMethod(dpropID.PropertyType);
        }

        /// <inheritdoc/>
        public override void Mutate(UltravioletContext uv, Object instance, UvmlInstantiationContext context)
        {
            var dobj = instance as DependencyObject;
            if (dobj == null)
                return;
            
            dpropSetter.Invoke(instance, new Object[] { dpropID, dpropValue.Instantiate(uv, context) });
        }

        // Reflection information for the open generic version of SetLocalValue() on DependencyObject
        private static readonly MethodInfo miSetLocalValue;

        // State values.
        private readonly DependencyProperty dpropID;
        private readonly UvmlNode dpropValue;
        private readonly MethodInfo dpropSetter;
    }
}
