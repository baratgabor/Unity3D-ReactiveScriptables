using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public static class MyMath
    {
        public enum Easing
        {
            None,
            SmoothStart2,
            SmoothStart3,
            SmoothStart4,
            SmoothStop2,
            SmoothStop3,
            SmoothStop4
        }

        private static Dictionary<Easing, Func<float, float>> _easingMap = new Dictionary<Easing, Func<float, float>>()
        {
            [Easing.None] = (x) => x,
            [Easing.SmoothStart2] = (x) => x * x,
            [Easing.SmoothStart3] = (x) => x * x * x,
            [Easing.SmoothStart4] = (x) => x * x * x * x,
            [Easing.SmoothStop2] = (x) => 1 - ((1 - x) * (1 - x)),
            [Easing.SmoothStop3] = (x) => 1 - ((1 - x) * (1 - x) * (1 - x)),
            [Easing.SmoothStop4] = (x) => 1 - ((1 - x) * (1 - x) * (1 - x) * (1 - x)),
        };

        public static float Map(this float x, float xMin, float xMax, float outputMin, float outputMax)
            => x.MapTo01(xMin, xMax)
                .Map01To(outputMin, outputMax);

        public static float Map(this float x, float xMin, float xMax, float outputMin, float outputMax, Easing easingType)
            => x.MapTo01(xMin, xMax)
                .Ease01(easingType)
                .Map01To(outputMin, outputMax);

        public static float Ease01(this float x, Easing easingType)
            => _easingMap[easingType].Invoke(x);

        /// <summary>
        /// Converts a float to its 0-1 position in the given range.
        /// </summary>
        public static float MapTo01(this float x, float xMin, float xMax)
            => Mathf.Clamp01(MapTo(x, xMin, xMax));

        /// <summary>
        /// Converts a float to 0-1 its position in the given range.
        /// If the float is outside the range, returns less than 0 or greater than 1.
        /// </summary>
        public static float MapTo(this float x, float xMin, float xMax)
            => (x - xMin) / (xMax - xMin);

        /// <summary>
        /// Converts float in 0-1 range to another range.
        /// </summary>
        public static float Map01To(this float x, float outMin, float outMax)
            => (Mathf.Clamp01(x) * (outMax - outMin)) + outMin;

        public static float Invert01(this float x)
        => Math.Abs(x - 1);

        // Output stuff at given stage while chaining, without having to break the chain.
        public static float OutputThis(this float x, out float output)
            => output = x;

        public static bool FastApproximately(float a, float b, float threshold)
        {
            return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;

            // return Math.Abs(a-b) <= threshold;
            // return ((a < b) ? (b - a) : (a - b)) <= threshold;
        }

        /*
        // (1-(1-x)^2) + (0.2 * ((1-(1-x)^3) - (1-(1-x)^2)))
        // x^2 + (0.2 * (x^3 - x^2))
        // smoothstep: x^2 + (-2 * (x^3 - x^2))
        public static float Mix(float a, float b, float weightB, float t)
        {
            return a + (weightB * (b - a));
        }

        public static float Crossfade(float a, float b, float t)
        {
            return a + t * (b - a);
            // (1-t)*a + (t)*b
        }

        public static float Scale(float x, float t)
        {
            return t * x;
        }

        public static float ReverseScale(float x, float t)
        {
            return (1 - t) * x;
        }

        public static float Arch(float t)
        {
            // Scale(Flip(t))
            return t * (1 - t);
        }

        public static float SmoothStartArch(float x)
        {
            // Scale(Arch2, t)
            return x * x * (1 - x);
        }

        public static float NormalizedBezier3(float b, float c, float t)
        {
            float s = 1 - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            return (3 * b * s2 * t) + (3 * c * s * t2) + (t3);
        }
        */
    }
}