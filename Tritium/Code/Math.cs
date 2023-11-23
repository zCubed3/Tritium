using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;


namespace Tritium
{
    // Wraps a bunch of base MathF methods with XNA compatibility in mind
    public static class TritMath
    {
        // Trig functions
        public static float Atan2(Vector2 direction) => MathF.Atan2(direction.Y, direction.X);

        // To make things compatible with our coordinate system, we invert X
        public static Vector2 DirectionFromAngle(float direction, float xmult = 1, float ymult = 1) => 
            new Vector2(MathF.Cos(direction * xmult), MathF.Sin(direction * ymult));

        // Same system Unity uses
        public const float RadianToDegree = 57.2957795131f;
        public const float DegreeToRadians = 0.01745329251f;

        // Common Vector2 functions
        public static Vector2 Abs(Vector2 a) => new Vector2(MathF.Abs(a.X), MathF.Abs(a.Y));
        public static Vector2 Sqrt(Vector2 a) => new Vector2(MathF.Sqrt(a.X), MathF.Sqrt(a.Y));
        public static float Clamp(float x, float min = 0, float max = 1) => MathF.Max(min, MathF.Min(x, max));
        
        public static float Wrap(float x, float min = 0, float max = 1)
        {
            return x - (max - min) * MathF.Floor(x / (max - min));
        }

        // TODO: Faster angle lerp?
        public static float LerpAngle(float a, float b, float alpha)
        {
            Vector2 dA = DirectionFromAngle(a);
            Vector2 dB = DirectionFromAngle(b);

            return Atan2(Lerp(dA, dB, alpha));
        }

        // Hold on, we have a reason to repeat code here. Dynamic is too slow to be useful and generics lack the ability to be static!
        // Clamped interpolations
        public static float Lerp(float a, float b, float alpha) => UnclampedLerp(a, b, Clamp(alpha, 0.0f, 1.0f));
        public static Vector2 Lerp(Vector2 a, Vector2 b, float alpha) => UnclampedLerp(a, b, Clamp(alpha, 0.0f, 1.0f));

        // Same case here!
        // Unclamped interpolations
        public static float UnclampedLerp(float a, float b, float alpha) => a + (b - a) * alpha;
        public static Vector2 UnclampedLerp(Vector2 a, Vector2 b, float alpha) => a + (b - a) * alpha;

        // Common powers
        public static float Sqr(float a) => a * a;
        public static float Cube(float a) => a * a * a;

        // IK related
        public static float LawOfCosinesSSS(float a, float b, float c)
        {
            return MathF.Acos(Sqr(a) + Sqr(b) - Sqr(c) / 2 * a * b);
        }

        public static float LawOfCosinesSAS(float a, float b, float cos)
        {
            return MathF.Sqrt(Sqr(a) + Sqr(b) - 2 * a * b * MathF.Cos(cos));
        }

        // Averaging
        public static Vector2 Average(params Vector2[] vectors)
        {
            Vector2 average = Vector2.Zero;
            foreach (Vector2 vector in vectors)
                average += vector;

            return average / vectors.Length;
        }
    }
}
