using System;
using UnityEngine;

namespace NiGames.Essentials
{
    public static partial class Easing
    {
        #region Lambdas
        
        public static readonly Func<float, float> Linear = t => t;
        
        public static readonly Func<float, float> InQuad = t => t * t;
        
        public static readonly Func<float, float> OutQuad = t => t * (2 - t);
        
        public static readonly Func<float, float> InOutQuad = t => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        
        public static readonly Func<float, float> InCubic = t => t * t * t;
        
        public static readonly Func<float, float> OutCubic = t => --t * t * t + 1;
        
        public static readonly Func<float, float> InOutCubic = t => t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
        
        public static readonly Func<float, float> InQuart = t => t * t * t * t;
        
        public static readonly Func<float, float> OutQuart = t => 1 - --t * t * t * t;
        
        public static readonly Func<float, float> InOutQuart = t => t < 0.5f ? 8 * t * t * t * t : 1 - 8 * --t * t * t * t;
        
        public static readonly Func<float, float> InQuint = t => t * t * t * t * t;
        
        public static readonly Func<float, float> OutQuint = t => 1 + --t * t * t * t * t;
        
        public static readonly Func<float, float> InOutQuint = t => t < 0.5f ? 16 * t * t * t * t * t : 1 + 16 * --t * t * t * t * t;
        
        public static readonly Func<float, float> InSine = t => 1 - Mathf.Cos(t * Mathf.PI / 2);
        
        public static readonly Func<float, float> OutSine = t => Mathf.Sin(t * Mathf.PI / 2);
        
        public static readonly Func<float, float> InOutSine = t => -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
        
        public static readonly Func<float, float> InExpo = t => Mathf.Approximately(t, 0f) ? 0f : Mathf.Pow(2, 10 * (t - 1));
        
        public static readonly Func<float, float> OutExpo = t => Mathf.Approximately(t, 1f) ? 1f : 1 - Mathf.Pow(2, -10 * t);

        public static readonly Func<float, float> InOutExpo = t =>
        {
            if (Mathf.Approximately(t, 0f)) return 0f;
            if (Mathf.Approximately(t, 1f)) return 1f;
            return t < 0.5f
                ? Mathf.Pow(2, 20 * t - 10) / 2
                : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
        };

        public static readonly Func<float, float> InCirc = t => 1 - Mathf.Sqrt(1 - t * t);
        
        public static readonly Func<float, float> OutCirc = t => Mathf.Sqrt(1 - --t * t);

        public static readonly Func<float, float> InOutCirc = t => t < 0.5f
            ? (1 - Mathf.Sqrt(1 - 4 * t * t)) / 2
            : (Mathf.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2;

        public static readonly Func<float, float> InBack = InBackEase();
        
        public static readonly Func<float, float> OutBack = OutBackEase();
        
        public static readonly Func<float, float> InOutBack = InOutBackEase();

        public static readonly Func<float, float> InElastic = InElasticEase();

        public static readonly Func<float, float> OutElastic = OutElasticEase();
        
        public static readonly Func<float, float> InOutElastic = InOutElasticEase();
        
        public static readonly Func<float, float> InBounce = t => 1 - OutBounce(1 - t);

        public static readonly Func<float, float> OutBounce = t =>
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            switch (t)
            {
                case < 1 / d1: return n1 * t * t;
                case < 2 / d1: return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }

            if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }

            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        };

        public static readonly Func<float, float> InOutBounce = t => t < 0.5f 
            ? (1 - OutBounce(1 - 2 * t)) / 2 
            : (1 + OutBounce(2 * t - 1)) / 2;
        
        #endregion
        
        #region Functions
        
        public static Func<float, float> InBackEase(float s = 1.70158f)
        {
            return t => t * t * ((s + 1) * t - s);
        }
        
        public static Func<float, float> OutBackEase(float s = 1.70158f)
        {
            return t => --t * t * ((s + 1) * t + s) + 1;
        }
        
        public static Func<float, float> InElasticEase(float amplitude = 1f, float period = 0.3f)
        {
            return t =>
            {
                if (Mathf.Approximately(t, 0f)) return 0f;
                if (Mathf.Approximately(t, 1f)) return 1f;
                return -amplitude * Mathf.Pow(2, 10 * (t - 1)) * Mathf.Sin((t - 1.1f) * 2 * Mathf.PI / period);
            };
        }
        
        public static Func<float, float> InOutBackEase(float s = 1.70158f)
        {
            return t =>
            {
                s *= 1.525f;
                return t < 0.5f
                    ? t * 2 * t * t * ((s + 1) * t * 2 - s) / 2
                    : ((t * 2 - 2) * (t * 2 - 2) * ((s + 1) * (t * 2 - 2) + s) + 2) / 2;
            };
        }
        
        public static Func<float, float> OutElasticEase(float amplitude = 1f, float period = 0.3f)
        {
            return t =>
            {
                if (Mathf.Approximately(t, 0f)) return 0f;
                if (Mathf.Approximately(t, 1f)) return 1f;
                return amplitude * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.1f) * 2 * Mathf.PI / period) + 1;
            };
        }
        
        public static Func<float, float> InOutElasticEase(float amplitude = 1f, float period = 0.45f)
        {
            return t =>
            {
                if (Mathf.Approximately(t, 0f)) return 0f;
                if (Mathf.Approximately(t, 1f)) return 1f;
                return t < 0.5f
                    ? -(amplitude * Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * 2 * Mathf.PI / period)) / 2
                    : amplitude * Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * 2 * Mathf.PI / period) / 2 + 1;
            };
        }
        
        #endregion
    }
}
