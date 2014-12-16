﻿using System;
using System.Collections.Generic;
using System.Linq;
using TwistedLogik.Nucleus;
using TwistedLogik.Nucleus.Data;
using TwistedLogik.Ultraviolet.Layout.Animation;
using TwistedLogik.Ultraviolet.Layout.Elements;

namespace TwistedLogik.Ultraviolet.Layout.Stylesheets
{
    /// <summary>
    /// Contains methods for reifying instances of <see cref="UvssStoryboard"/> into instances of <see cref="Storyboard"/>.
    /// </summary>
    internal static class UvssStoryboardReifier
    {
        /// <summary>
        /// Initializes the <see cref="UvssStoryboardReifier"/> type.
        /// </summary>
        static UvssStoryboardReifier()
        {
            standardEasingFunctions["ease-in-linear"]          = Easings.EaseInLinear;
            standardEasingFunctions["ease-out-linear"]         = Easings.EaseOutLinear;
            standardEasingFunctions["ease-in-cubic"]           = Easings.EaseInCubic;
            standardEasingFunctions["ease-out-cubic"]          = Easings.EaseOutCubic;
            standardEasingFunctions["ease-in-quadratic"]       = Easings.EaseInQuadratic;
            standardEasingFunctions["ease-out-quadratic"]      = Easings.EaseOutQuadratic;
            standardEasingFunctions["ease-in-out-quadratic"]   = Easings.EaseInOutQuadratic;
            standardEasingFunctions["ease-in-quartic"]         = Easings.EaseInQuartic;
            standardEasingFunctions["ease-out-quartic"]        = Easings.EaseOutQuartic;
            standardEasingFunctions["ease-in-out-quartic"]     = Easings.EaseInOutQuartic;
            standardEasingFunctions["ease-in-quintic"]         = Easings.EaseInQuintic;
            standardEasingFunctions["ease-out-quintic"]        = Easings.EaseInQuintic;
            standardEasingFunctions["ease-in-out-quintic"]     = Easings.EaseInOutQuintic;
            standardEasingFunctions["ease-in-sin"]             = Easings.EaseInSin;
            standardEasingFunctions["ease-out-sin"]            = Easings.EaseOutSin;
            standardEasingFunctions["ease-in-out-sin"]         = Easings.EaseInOutSin;
            standardEasingFunctions["ease-in-exponential"]     = Easings.EaseInExponential;
            standardEasingFunctions["ease-out-exponential"]    = Easings.EaseOutExponential;
            standardEasingFunctions["ease-in-out-exponential"] = Easings.EaseInOutExponential;
            standardEasingFunctions["ease-in-circular"]        = Easings.EaseInCircular;
            standardEasingFunctions["ease-out-circular"]       = Easings.EaseOutCircular;
            standardEasingFunctions["ease-in-out-circular"]    = Easings.EaseInOutCircular;
            standardEasingFunctions["ease-in-back"]            = Easings.EaseInBack;
            standardEasingFunctions["ease-out-back"]           = Easings.EaseOutBack;
            standardEasingFunctions["ease-in-out-back"]        = Easings.EaseInOutBack;
            standardEasingFunctions["ease-in-elastic"]         = Easings.EaseInElastic;
            standardEasingFunctions["ease-out-elastic"]        = Easings.EaseOutElastic;
            standardEasingFunctions["ease-in-out-elastic"]     = Easings.EaseInOutElastic;
            standardEasingFunctions["ease-in-bounce"]          = Easings.EaseInBounce;
            standardEasingFunctions["ease-out-bounce"]         = Easings.EaseOutBounce;
            standardEasingFunctions["ease-in-out-bounce"]      = Easings.EaseInOutBounce;
        }

        /// <summary>
        /// Reifies an instance of the <see cref="UvssStoryboard"/> class into a new instance of the <see cref="Storyboard"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="storyboardDefinition">The storyboard definition to reify.</param>
        /// <returns>The new instance of <see cref="Storyboard"/> that was created.</returns>
        public static Storyboard ReifyStoryboard(UltravioletContext uv, UvssStoryboard storyboardDefinition)
        {
            Contract.Require(uv, "uv");
            Contract.Require(storyboardDefinition, "storyboardDefinition");

            var storyboard = new Storyboard(uv);

            foreach (var targetDefinition in storyboardDefinition.Targets)
            {
                var target = ReifyStoryboardTarget(targetDefinition);
                storyboard.Targets.Add(target);
            }

            return storyboard;
        }

        /// <summary>
        /// Creates a new <see cref="StoryboardTarget"/> instance based on the specified <see cref="UvssStoryboardTarget"/>.
        /// </summary>
        /// <param name="targetDefinition">The storyboard target definition.</param>
        /// <returns>The reified storyboard target.</returns>
        public static StoryboardTarget ReifyStoryboardTarget(UvssStoryboardTarget targetDefinition)
        {
            Contract.Require(targetDefinition, "targetDefinition");

            var target = new StoryboardTarget(targetDefinition.Selector);

            foreach (var animationDefinition in targetDefinition.Animations)
            {
                var animation = ReifyStoryboardAnimation(targetDefinition.Filter, animationDefinition);
                if (animation != null)
                {
                    target.Animations.Add(animationDefinition.AnimatedProperty, animation);
                }
            }

            return target;
        }

        /// <summary>
        /// Creates a new <see cref="AnimationBase"/> instance based on the specified <see cref="UvssStoryboardAnimation"/>.
        /// </summary>
        /// <param name="filter">The type filter on the storyboard target.</param>
        /// <param name="animationDefinition">The storyboard animation definition.</param>
        /// <returns>The reified storyboard animation.</returns>
        public static AnimationBase ReifyStoryboardAnimation(UvssStoryboardTargetFilter filter, UvssStoryboardAnimation animationDefinition)
        {
            Contract.Require(filter, "filter");
            Contract.Require(animationDefinition, "animationDefinition");

            var propertyType = GetDependencyPropertyType(filter, animationDefinition.AnimatedProperty);
            if (propertyType == null)
                return null;

            var animationType = GetAnimationType(propertyType);
            if (animationType == null)
                return null;

            var animation = (AnimationBase)Activator.CreateInstance(animationType);

            foreach (var keyframeDefinition in animationDefinition.Keyframes)
            {
                var keyframe = ReifyStoryboardAnimationKeyframe(keyframeDefinition, propertyType);
                animation.AddKeyframe(keyframe);
            }

            return animation;
        }

        /// <summary>
        /// Creates a new <see cref="AnimationKeyframeBase"/> instance based on the specified <see cref="UvssStoryboardKeyframe"/>.
        /// </summary>
        /// <param name="keyframeDefinition">The storyboard keyframe definition.</param>
        /// <param name="animatedType">The type of value which is being animated.</param>
        /// <returns>The reified storyboard animation keyframe.</returns>
        public static AnimationKeyframeBase ReifyStoryboardAnimationKeyframe(UvssStoryboardKeyframe keyframeDefinition, Type animatedType)
        {
            Contract.Require(keyframeDefinition, "keyframeDefinition");
            Contract.Require(animatedType, "animatedType");

            var keyframeType = typeof(AnimationKeyframe<>).MakeGenericType(animatedType);

            var time   = TimeSpan.FromMilliseconds(keyframeDefinition.Time);
            var easing = ParseEasingFunction(keyframeDefinition.Easing);
            var value  = String.IsNullOrWhiteSpace(keyframeDefinition.Value) ? null : 
                    ObjectResolver.FromString(keyframeDefinition.Value, animatedType);

            var keyframe = (value == null) ?
                    (AnimationKeyframeBase)Activator.CreateInstance(keyframeType, time, easing) :
                    (AnimationKeyframeBase)Activator.CreateInstance(keyframeType, time, value, easing);

            return keyframe;
        }

        /// <summary>
        /// Parses the name of an easing function into an instance of the <see cref="EasingFunction"/> delegate.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The <see cref="EasingFunction"/> delegate instance which was created.</returns>
        private static EasingFunction ParseEasingFunction(String str)
        {
            if (String.IsNullOrEmpty(str))
                return null;

            EasingFunction function;
            standardEasingFunctions.TryGetValue(str, out function);
            return function;
        }

        /// <summary>
        /// Resolves the specified element type name into a <see cref="Type"/> object.
        /// </summary>
        /// <param name="name">The element type name to resolve.</param>
        /// <returns>The resolved <see cref="Type"/> object.</returns>
        private static Type ResolveElementType(String name)
        {
            return UIViewLoader.GetElementTypeFromName(name, false);
        }

        /// <summary>
        /// Gets the type of the specified dependency property.
        /// </summary>
        /// <param name="filter">The type filter on the storyboard target.</param>
        /// <param name="property">The name of the dependency property being animated.</param>
        /// <returns>The type of the specified dependency property.</returns>
        private static Type GetDependencyPropertyType(UvssStoryboardTargetFilter filter, String property)
        {
            var possiblePropertyTypes =
                (from f in filter
                 let elementType = ResolveElementType(f)
                 let propertyID = UIElement.FindStyledDependencyProperty(property, elementType)
                 let propertyType = (propertyID == null) ? null : Type.GetTypeFromHandle(propertyID.PropertyType)
                 where propertyType != null
                 select propertyType).Distinct();

            if (possiblePropertyTypes.Count() > 1)
                throw new InvalidOperationException(LayoutStrings.AmbiguousDependencyPropertyType.Format(property));

            return possiblePropertyTypes.SingleOrDefault();
        }

        /// <summary>
        /// Gets the animation type which corresponds to the specified type of value.
        /// </summary>
        /// <param name="type">The type of value being animated.</param>
        /// <returns>The animation type which corresponds to the specified type of value.</returns>
        private static Type GetAnimationType(Type type)
        {
            var nullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (nullable)
                type = type.GetGenericArguments()[0];

            if (type.IsPrimitive || type == typeof(Decimal))
            {
                var animationTypeName = nullable ? String.Format("Nullable{0}Animation", type.Name) : String.Format("{0}Animation", type.Name);
                return Type.GetType(animationTypeName);
            }

            var interpolatableInterface = typeof(IInterpolatable<>).MakeGenericType(type);
            if (type.GetInterfaces().Contains(interpolatableInterface))
            {
                return nullable ? typeof(NullableInterpolatableAnimation<>).MakeGenericType(type) :
                    typeof(InterpolatableAnimation<>).MakeGenericType(type);
            }

            return typeof(ObjectAnimation<>).MakeGenericType(type);
        }

        // The standard easing functions which can be specified in an animation.
        private static readonly Dictionary<String, EasingFunction> standardEasingFunctions = 
            new Dictionary<String, EasingFunction>();
    }
}
