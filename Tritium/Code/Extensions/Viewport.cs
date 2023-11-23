using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium
{
    public static class ViewportExtensions
    {
        public static Vector2 GetSize(this Viewport viewport) => new Vector2(viewport.Width, viewport.Height);
    }
}
