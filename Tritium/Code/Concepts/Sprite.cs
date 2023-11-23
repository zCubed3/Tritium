using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Assets;

namespace Tritium.Concepts
{
    [Impl.SubData]
    public class Sprite
    {
        public Texture2DCRef texture;
        public Rectangle rect;
    }
}
