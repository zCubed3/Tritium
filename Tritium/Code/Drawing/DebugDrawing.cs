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
    public static class DebugDrawing
    {
        public delegate void DrawCallDelegate();
        private static List<DrawCallDelegate> drawCalls = new List<DrawCallDelegate>();

        private static DrawingLayer debugLayer = new DrawingLayer();

        public static void Draw(Texture2D texture, Transform2D transform, Rectangle rectangle, Color color, float? forceDepth = null, bool mirrorX = false, bool mirrorY = false)
        {
            drawCalls.Add(() =>
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
            });
        }

        public static void RenderDebug(SpriteBatch spriteBatch, Matrix? view)
        {
            using (new ScopedLayer(spriteBatch, debugLayer, view))
            {
                foreach (var drawCall in drawCalls)
                {
                    drawCall?.Invoke();
                }
            }

            drawCalls.Clear();
        }
    }
}
