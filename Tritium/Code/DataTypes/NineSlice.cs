using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium
{
    public struct NineSlice
    {
        public Rectangle[] patches = new Rectangle[9];
        public Vector2[] offsets = new Vector2[9];

        // https://gamedev.stackexchange.com/questions/115684/monogame-scale-resizing-texture-without-stretching-the-borders
        public NineSlice(Rectangle rect, int borderPx)
        {
            int x = rect.X;
            int y = rect.Y;
            int w = rect.Width;
            int h = rect.Height;

            int middleWidth = w - borderPx * 2;
            int middleHeight = h - borderPx * 2;

            int bottomY = y + h - borderPx;
            int rightX = x + w - borderPx;
            int leftX = x + borderPx;
            int topY = y + borderPx;

            patches[0] = new Rectangle(x, y, borderPx, borderPx); // Top left corner
            patches[1] = new Rectangle(leftX, y, middleWidth, borderPx); // Top middle
            patches[2] = new Rectangle(rightX, y, borderPx, borderPx); // Top right

            patches[3] = new Rectangle(x, topY, borderPx, middleHeight); // Middle left
            patches[4] = new Rectangle(leftX, topY, middleWidth, middleHeight); // Middle
            patches[5] = new Rectangle(rightX, topY, borderPx, middleHeight); // Middle right

            patches[6] = new Rectangle(x, bottomY, borderPx, borderPx); // Bottom left corner
            patches[7] = new Rectangle(leftX, bottomY, middleWidth, borderPx); // Bottom middle
            patches[8] = new Rectangle(rightX, bottomY, borderPx, borderPx); // Bottom right corner

            // Offsets are calculated automatically
            for (int p = 0; p < 9; p++)
            {
                float xDiff = (patches[p].X - x) / (float)w;
                float yDiff = (patches[p].Y - y) / (float)h;
                offsets[p] = new Vector2(xDiff, yDiff);
            }
        }
    }
}
