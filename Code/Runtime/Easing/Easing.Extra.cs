using System;
using UnityEngine;

namespace NiGames.Essentials
{
    public static partial class Easing
    {
        public static class Extra
        {
            #region Shorthands

            /// <summary>Simulates a heartbeat pattern with pulsating effect.</summary>
            public static readonly Func<float, float> Heartbeat = HeartbeatEase();

            /// <summary>Creates a stretching and contracting effect like a rubber band.</summary>
            public static readonly Func<float, float> RubberBand = RubberBandEase();

            /// <summary>Produces a ripple effect similar to waves on water.</summary>
            public static readonly Func<float, float> Ripple = RippleEase();

            /// <summary>Generates a wobbling or oscillating motion.</summary>
            public static readonly Func<float, float> Wobble = WobbleEase();

            /// <summary>Mimics the movement of a gelatinous substance.</summary>
            public static readonly Func<float, float> Gelatinous = GelatinousEase();

            /// <summary>Combines elastic motion with an overshoot effect.</summary>
            public static readonly Func<float, float> ElasticOvershoot = ElasticOvershootEase();

            /// <summary>Simulates a spring-like motion with bounce and settle.</summary>
            public static readonly Func<float, float> Spring = SpringEase();

            /// <summary>Creates a bouncing effect that gradually settles.</summary>
            public static readonly Func<float, float> BounceAndSettle = BounceAndSettleEase();

            /// <summary>Imitates the effect of gravity pulling on an object.</summary>
            public static readonly Func<float, float> GravityPull = GravityPullEase();

            /// <summary>Produces an exponential wave pattern.</summary>
            public static readonly Func<float, float> ExponentialWave = ExponentialWaveEase();

            /// <summary>Creates a stepped or staircase-like progression.</summary>
            public static readonly Func<float, float> Staircase = StaircaseEase();

            /// <summary>Generates a pulsating effect like a heartbeat.</summary>
            public static readonly Func<float, float> Pulse = PulseEase();

            /// <summary>Produces a zigzag pattern of motion.</summary>
            public static readonly Func<float, float> Zigzag = ZigzagEase();

            /// <summary>Creates a logarithmic progression curve.</summary>
            public static readonly Func<float, float> Logarithmic = LogarithmicEase();

            /// <summary>Combines sinusoidal motion with a bouncing effect.</summary>
            public static readonly Func<float, float> SinusoidalBounce = SinusoidalBounceEase();

            /// <summary>Produces an exponential rebound effect.</summary>
            public static readonly Func<float, float> ExponentialRebound = ExponentialReboundEase();

            /// <summary>Simulates an elastic snapping motion.</summary>
            public static readonly Func<float, float> ElasticSnap = ElasticSnapEase();

            /// <summary>Creates an exponential decay effect.</summary>
            public static readonly Func<float, float> ExponentialDecay = ExponentialDecayEase();

            /// <summary>Produces a circular arc motion.</summary>
            public static readonly Func<float, float> CircularArc = CircularArcEase();

            /// <summary>Combines bouncy and overshoot effects.</summary>
            public static readonly Func<float, float> BouncyOvershoot = BouncyOvershootEase();

            /// <summary>Creates an elastic punching effect.</summary>
            public static readonly Func<float, float> ElasticPunch = ElasticPunchEase();

            /// <summary>Produces a smooth step transition.</summary>
            public static readonly Func<float, float> SmoothStep = SmoothStepEase();

            /// <summary>Generates an arch-like motion.</summary>
            public static readonly Func<float, float> Arch = ArchEase();

            /// <summary>Combines bouncy and sinusoidal effects.</summary>
            public static readonly Func<float, float> BouncySine = BouncySineEase();

            /// <summary>Creates a quadratic Bézier curve effect.</summary>
            public static readonly Func<float, float> QuadraticBezier = QuadraticBezierEase();

            /// <summary>Produces a cubic Bézier curve effect.</summary>
            public static readonly Func<float, float> CubicBezier = CubicBezierEase();

            /// <summary>Generates an elastic double bouncing effect.</summary>
            public static readonly Func<float, float> ElasticDoubleBounce = ElasticDoubleBounceEase();

            /// <summary>Creates an exponential approach effect.</summary>
            public static readonly Func<float, float> ExponentialApproach = ExponentialApproachEase();

            /// <summary>Produces a parabolic arc motion.</summary>
            public static readonly Func<float, float> ParabolicArc = ParabolicArcEase();

            /// <summary>Generates a sine wave pattern.</summary>
            public static readonly Func<float, float> SineWave = SineWaveEase();

            /// <summary>Combines bouncy and elastic effects.</summary>
            public static readonly Func<float, float> BouncyElastic = BouncyElasticEase();

            /// <summary>Creates a sigmoid or S-shaped curve.</summary>
            public static readonly Func<float, float> Sigmoid = SigmoidEase();

            /// <summary>Produces a rational function curve.</summary>
            public static readonly Func<float, float> Rational = RationalEase();

            /// <summary>Generates a hyperbolic tangent curve.</summary>
            public static readonly Func<float, float> HyperbolicTangent = HyperbolicTangentEase();

            /// <summary>Combines bouncy and exponential effects.</summary>
            public static readonly Func<float, float> BouncyExponential = BouncyExponentialEase();

            /// <summary>Creates a punching effect with overshoot and settle.</summary>
            public static readonly Func<float, float> Punch = PunchEase();

            /// <summary>Generates a shaking effect that gradually diminishes.</summary>
            public static readonly Func<float, float> Shake = ShakeEase();

            #endregion

            #region Functions

            public static Func<float, float> HeartbeatEase(float frequency = 8f, float amplitude = 0.2f)
            {
                return t =>
                {
                    var x = Mathf.Sin(t * Mathf.PI * frequency) * (1 - t);
                    return t + x * amplitude;
                };
            }

            public static Func<float, float> RubberBandEase(float strength = 1f)
            {
                return t => t < 0.5f
                    ? 0.5f * (1 - Mathf.Pow(1 - 2 * t, 3 * strength))
                    : 0.5f * (Mathf.Pow(2 * t - 2, 3 * strength) + 2);
            }

            public static Func<float, float> RippleEase(float frequency = 10f)
            {
                return t =>
                {
                    var x = t * frequency - 5;
                    return 1 - (Mathf.Sin(x) / (x + 0.01f) + 1) / 2;
                };
            }

            public static Func<float, float> WobbleEase(float frequency = 2f)
            {
                return t => t - 0.5f * Mathf.Sin(t * frequency * Mathf.PI);
            }

            public static Func<float, float> GelatinousEase(float elasticity = 2.5f)
            {
                return t => (Mathf.Sin(t * Mathf.PI * (0.2f + elasticity * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t) * (1f + 1.2f * (1f - t));
            }

            public static Func<float, float> ElasticOvershootEase(float amplitude = 1f, float period = 0.3f)
            {
                return t => t < 0.5f
                    ? 0.5f * Mathf.Sin(13 * Mathf.PI / 2 * t) * Mathf.Pow(2, 10 * (t - 0.5f)) * amplitude
                    : 0.5f * (2 - Mathf.Sin(13 * Mathf.PI / 2 * (t * 0.5f + 0.5f)) * Mathf.Pow(2, -10 * (t - 0.5f))) * amplitude;
            }

            public static Func<float, float> SpringEase(float frequency = 4f, float damping = 6f)
            {
                return t => 1f - Mathf.Cos(t * Mathf.PI * frequency) * Mathf.Exp(-t * damping);
            }

            public static Func<float, float> BounceAndSettleEase(float bounceFrequency = 8f, float bounceAmplitude = 0.2f)
            {
                return t => 1 - (Mathf.Pow(1 - t, 4) + Mathf.Sin(t * Mathf.PI * bounceFrequency) * Mathf.Pow(1 - t, 3) * bounceAmplitude);
            }

            public static Func<float, float> GravityPullEase(float strength = 2f)
            {
                return t => Mathf.Pow(t, strength) * (3 - 2 * t);
            }

            public static Func<float, float> ExponentialWaveEase(float frequency = 2f, float amplitude = 0.1f, float decay = 4f)
            {
                return t => t + amplitude * Mathf.Sin(frequency * Mathf.PI * t) * Mathf.Exp(-decay * t);
            }

            public static Func<float, float> StaircaseEase(int steps = 5)
            {
                return t => Mathf.Floor(t * steps) / (steps - 1);
            }

            public static Func<float, float> PulseEase(float frequency = 5f)
            {
                return t => 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * (t * frequency - 0.25f));
            }

            public static Func<float, float> ZigzagEase(float frequency = 1f)
            {
                return t => t * frequency % 1 < 0.5f 
                    ? 2 * (t * frequency % 1) 
                    : 2 * (1 - t * frequency % 1);
            }

            public static Func<float, float> LogarithmicEase(float base1 = 10f)
            {
                return t => Mathf.Log(1 + t * (base1 - 1)) / Mathf.Log(base1);
            }

            public static Func<float, float> SinusoidalBounceEase(float frequency = 4f, float amplitude = 0.4f)
            {
                return t =>
                {
                    var x = Mathf.Sin(t * Mathf.PI * frequency);
                    return t + amplitude * x * Mathf.Pow(1 - t, 2);
                };
            }

            public static Func<float, float> ExponentialReboundEase()
            {
                return t => t < 0.5f 
                    ? 0.5f * (1 - Mathf.Sqrt(1 - 4 * t * t)) 
                    : 0.5f * (Mathf.Sqrt((3 - 2 * t) * (2 * t - 1)) + 1);
            }

            public static Func<float, float> ElasticSnapEase(float amplitude = 0.1f)
            {
                return t => t * t * (3 - 2 * t) + Mathf.Sin(t * Mathf.PI * 4) * Mathf.Pow(1 - t, 2) * amplitude;
            }

            public static Func<float, float> ExponentialDecayEase(float rate = 4f)
            {
                return t => 1 - Mathf.Exp(-t * rate);
            }

            public static Func<float, float> CircularArcEase()
            {
                return t => 1 - Mathf.Sqrt(1 - t * t);
            }

            public static Func<float, float> BouncyOvershootEase(float overshoot = 1.70158f, float bounceFrequency = 7f, float bounceAmplitude = 0.05f)
            {
                return t =>
                {
                    var x = t * 2;
                    if (x < 1) return (1 - Mathf.Sqrt(1 - x * x)) / 2;
                    x -= 2;
                    return (Mathf.Sqrt(1 - x * x) + 1) / 2 + Mathf.Sin(t * Mathf.PI * bounceFrequency) * Mathf.Pow(1 - t, 2) * bounceAmplitude;
                };
            }

            public static Func<float, float> ElasticPunchEase(float amplitude = 1f, float period = 0.3f)
            {
                return t => Mathf.Pow(2, -10 * t) * Mathf.Sin((t - period / 4) * (2 * Mathf.PI) / period) * amplitude + 1;
            }

            public static Func<float, float> SmoothStepEase()
            {
                return t => t * t * (3 - 2 * t);
            }

            public static Func<float, float> ArchEase()
            {
                return t => t * (1 - t) * 4;
            }

            public static Func<float, float> BouncySineEase(float frequency = 2f, float decay = 3f, float amplitude = 0.3f)
            {
                return t => Mathf.Sin(t * Mathf.PI * frequency) * Mathf.Exp(-t * decay) * amplitude + t;
            }

            public static Func<float, float> QuadraticBezierEase()
            {
                return t => t * t * (3 - 2 * t);
            }

            public static Func<float, float> CubicBezierEase()
            {
                return t => t * t * t * (10 + t * (-15 + t * 6));
            }

            public static Func<float, float> ElasticDoubleBounceEase(float frequency = 5f, float amplitude = 0.1f)
            {
                return t => t < 0.5f
                    ? 0.5f - Mathf.Pow(1 - 2 * t, 4) / 2 + Mathf.Sin(t * Mathf.PI * frequency) * amplitude
                    : 0.5f + Mathf.Pow(2 * t - 2, 4) / 2 + Mathf.Sin(t * Mathf.PI * frequency) * amplitude;
            }

            public static Func<float, float> ExponentialApproachEase(float rate = 10f)
            {
                return t => 1 - Mathf.Pow(2, -rate * t);
            }

            public static Func<float, float> ParabolicArcEase()
            {
                return t => 1 - (2 * t - 1) * (2 * t - 1);
            }

            public static Func<float, float> SineWaveEase(float frequency = 1f)
            {
                return t => (Mathf.Sin(2 * Mathf.PI * t * frequency) + 1) / 2;
            }

            public static Func<float, float> BouncyElasticEase(float frequency = 13f, float decay = 10f)
            {
                return t => Mathf.Sin(frequency * Mathf.PI / 2 * t) * Mathf.Pow(2, -decay * t) + t;
            }

            public static Func<float, float> SigmoidEase(float steepness = 10f)
            {
                return t => 1 / (1 + Mathf.Exp(-steepness * (t - 0.5f)));
            }

            public static Func<float, float> RationalEase()
            {
                return t => t / (2 - t);
            }

            public static Func<float, float> HyperbolicTangentEase(float steepness = 2f)
            {
                return t => (Mathf.Exp(steepness * t) - 1) / (Mathf.Exp(steepness * t) + 1);
            }

            public static Func<float, float> BouncyExponentialEase(float bounceFrequency = 5f, float bounceAmplitude = 0.1f)
            {
                return t => t < 0.5f
                    ? 0.5f * (1 - Mathf.Sqrt(1 - 4 * t * t) - Mathf.Sin(t * Mathf.PI * bounceFrequency) * bounceAmplitude)
                    : 0.5f * (Mathf.Sqrt(-((2 * t) - 3) * ((2 * t) - 1)) + 1 - Mathf.Sin(t * Mathf.PI * bounceFrequency) * bounceAmplitude);
            }

            public static Func<float, float> PunchEase(float amplitude = 1f, float frequency = 3f)
            {
                return t =>
                {
                    var s = 9f / (2f * Mathf.PI) * frequency;
                    return t switch
                    {
                        0f => 0f,
                        1f => 0f,
                        _ => amplitude * Mathf.Exp(-t * s) * Mathf.Sin(2 * Mathf.PI * frequency * t)
                    };
                };
            }

            public static Func<float, float> ShakeEase(float amplitude = 1f, float frequency = 4f)
            {
                return t => amplitude * Mathf.Sin(frequency * t * Mathf.PI * 2) * (1 - t);
            }
            
            #endregion
        }
    }
}