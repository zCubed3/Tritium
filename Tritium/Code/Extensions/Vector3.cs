using System;
using System.Drawing;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;

namespace Tritium
{
    public static class Vector3Extensions
    {
        public static void Parse(this ref Vector3 vector, string data)
        {
            // We try to read this back as best we can
            var matches = Regex.Matches(data, RegexPatterns.DecimalOnly);

            if (matches.Count > 3)
                throw new Exception("More than 3 numbers found in Vector2 field!");

            if (matches.Count != 0)
            {
                float.TryParse(matches[0].Value, out vector.X);

                if (matches.Count > 1)
                    float.TryParse(matches[1].Value, out vector.Y);

                if (matches.Count > 2)
                    float.TryParse(matches[2].Value, out vector.Z);
            }
        }
    }
}
