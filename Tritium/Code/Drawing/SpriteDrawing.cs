using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Concepts;


namespace Tritium.Drawing
{
    public static class SpriteDrawing
    {
        public static void Draw(Texture2D texture, Transform2D transform, Rectangle rectangle, Color color, float? forceDepth = null, bool mirrorX = false, bool mirrorY = false)
        {
            DrawingLayer.ActiveLayer.batch.Draw(
                texture,
                transform.Position,
                rectangle,
                color,
                transform.Angle,
                transform.GetRectOrigin(rectangle),
                rectangle.NormalizeScale(transform.Scale),
                (mirrorX ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (mirrorY ? SpriteEffects.FlipVertically : SpriteEffects.None),
                forceDepth ?? transform.depth
            );
        }
    }
}
