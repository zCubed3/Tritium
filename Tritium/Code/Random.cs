using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;


namespace Tritium
{
    // Wraps System.Random to provide simple stuff
    public static class TritRandom
    {
        public static float RandomRange(float min = 0, float max = 1) => TritMath.Lerp(min, max, Random.Shared.NextSingle());

        public static Vector2 RandomDirection() => Vector2.Normalize(new Vector2(RandomRange(-1, 1), RandomRange(-1, 1)));

        public static float RandomAngle() => RandomRange(-MathF.PI, MathF.PI);
    }
}
