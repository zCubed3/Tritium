using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium
{
    public static class RectangleExtensions
    {
        public static Vector2 GetSize(this Rectangle rect) => new Vector2(rect.Width, rect.Height);

        public static Vector2 NormalizeScale(this Rectangle rect, Vector2 scale) => scale / rect.GetSize();

        public static Rectangle Scale(this Rectangle rect, Vector2 scale) => new Rectangle(rect.X, rect.Y, rect.Width * (int)scale.X, rect.Height * (int)scale.Y);

        public static void Parse(this ref Rectangle rect, string data)
        {
            // We try to read this back as best we can

            // Rect regex is non permissive, Rects take ints, we can't read anything other than integers
            var matches = Regex.Matches(data, RegexPatterns.IntegerOnly);

            if (matches.Count > 4)
                throw new Exception("More than 4 numbers found in Rectangle field!");

            if (matches.Count != 0)
            {
                int.TryParse(matches[0].Value, out rect.X);

                if (matches.Count > 1)
                    int.TryParse(matches[1].Value, out rect.Y);

                if (matches.Count > 2)
                    int.TryParse(matches[2].Value, out rect.Width);

                if (matches.Count > 3)
                    int.TryParse(matches[3].Value, out rect.Height);
            }
        }
    }
}
