using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Tritium
{
    public struct Vector2Int
    {
        public int X = 0, Y = 0;

        public Vector2Int() { }
        public Vector2Int(int X, int Y) 
        {
            this.X = X; 
            this.Y = Y; 
        }

        public Vector2Int(Vector2Int other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }

        public Vector2Int(Vector2 other)
        {
            this.X = (int)other.X;
            this.Y = (int)other.Y;
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}
