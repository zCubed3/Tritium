using System;
using System.Drawing;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;

namespace Tritium
{
    public static class Vector2Extensions
    {
        public static void Parse(this ref Vector2 vector, string data)
        {
            // We try to read this back as best we can
            var matches = Regex.Matches(data, RegexPatterns.DecimalOnly);

            if (matches.Count > 2)
                throw new Exception("More than 2 numbers found in Vector2 field!");

            if (matches.Count != 0)
            {
                float.TryParse(matches[0].Value, out vector.X);

                if (matches.Count > 1)
                    float.TryParse(matches[1].Value, out vector.Y);
            }
        }

        public static PointF ToPointF(this Vector2 vector) => new PointF(vector.X, vector.Y);
        public static SizeF ToSizeF(this Vector2 vector) => new SizeF(vector.X, vector.Y);

        /// <summary>
        /// Reverses the X and the Y of this vector
        /// </summary>
        public static Vector2 FlipAxes(this Vector2 vector) => new Vector2(vector.Y, vector.X);

        public static Vector2 SafeNormalize(this Vector2 vector, float epsilon = 0.00001F)
        {
            if (vector.Length() >= epsilon)
                return Vector2.Normalize(vector);
            else
                return vector;
        }
    }
}
