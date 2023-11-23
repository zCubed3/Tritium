using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium
{
    public struct AABB
    {
        public Vector2 min, max;

        public AABB(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;

            if (min.X > max.X && min.Y > max.Y)
            {
                this.min = max;
                this.max = min;
            }
        }

        public bool ContainsPoint(Vector2 point)
        {
            return point.X >= min.X && point.Y >= min.Y && point.X <= max.X && point.Y <= max.Y;
        }

        public bool OverlapsAABB(AABB box)
        {
            return ContainsPoint(box.min) 
                || ContainsPoint(box.max) 
                || ContainsPoint(new Vector2(box.max.X, box.min.Y)) 
                || ContainsPoint(new Vector2(box.min.X, box.max.Y));
        }

        public bool ContainsAABB(AABB box)
        {
            return ContainsPoint(box.min)
                && ContainsPoint(box.max)
                && ContainsPoint(new Vector2(box.max.X, box.min.Y))
                && ContainsPoint(new Vector2(box.min.X, box.max.Y));
        }
    }
}
