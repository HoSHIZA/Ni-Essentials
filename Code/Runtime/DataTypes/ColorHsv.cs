using System;
using JetBrains.Annotations;
using UnityEngine;

namespace NiGames.Essentials
{
    [PublicAPI]
    public struct ColorHsv : IEquatable<ColorHsv>
    {
        public float H;
        public float S;
        public float V;
        public float A;
        
        public ColorHsv(float h, float s, float v, float a = 1f)
        {
            H = Mathf.Clamp01(h);
            S = Mathf.Clamp01(s);
            V = Mathf.Clamp01(v);
            A = Mathf.Clamp01(a);
        }

        public ColorHsv(Color color)
        {
            Color.RGBToHSV(color, out H, out S, out V);
            A = color.a;
        }
        
        public ColorHsv(Color32 color)
        {
            Color.RGBToHSV(color, out H, out S, out V);
            A = (float) color.a / 255;
        }
        
        public override string ToString() => $"HSVA({H:F3}, {S:F3}, {V:F3}, {A:F3})";

        #region With

        public ColorHsv WithHue(float hue) => new(hue, S, V, A);
        public ColorHsv WithSaturation(float saturation) => new(H, saturation, V, A);
        public ColorHsv WithValue(float value) => new(H, S, value, A);
        public ColorHsv WithAlpha(float alpha) => new(H, S, V, alpha);

        #endregion
        
        #region IEquatable Impl
        
        public bool Equals(ColorHsv other)
        {
            return H.Equals(other.H) && S.Equals(other.S) && V.Equals(other.V) && A.Equals(other.A);
        }

        public override bool Equals(object obj)
        {
            return obj is ColorHsv other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(H, S, V, A);
        }

        public static bool operator ==(ColorHsv left, ColorHsv right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ColorHsv left, ColorHsv right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Implicit Operators
        
        public static implicit operator ColorHsv(Color color)
        {
            return new ColorHsv(color);
        }

        public static implicit operator ColorHsv(Color32 color)
        {
            return new ColorHsv(color);
        }

        public static implicit operator Color(ColorHsv color)
        {
            var c = Color.HSVToRGB(color.H, color.S, color.V);
            c.a = color.A;
            return c;
        }

        public static implicit operator Color32(ColorHsv color)
        {
            var c = Color.HSVToRGB(color.H, color.S, color.V);
            c.a = color.A;
            return c;
        }
        
        #endregion
        
        #region Static Methods
        
        public static ColorHsv Lerp(ColorHsv a, ColorHsv b, float t)
        {
            t = Mathf.Clamp01(t);
            return new ColorHsv(
                Mathf.LerpAngle(a.H * 360f, b.H * 360f, t) / 360f,
                Mathf.Lerp(a.S, b.S, t),
                Mathf.Lerp(a.V, b.V, t),
                Mathf.Lerp(a.A, b.A, t)
            );
        }
        
        #endregion
    }
}