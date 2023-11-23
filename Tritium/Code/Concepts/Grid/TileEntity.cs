using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using Tritium.Assets;

using Tritium.Drawing;

namespace Tritium.Concepts.Grid
{
    // TODO: Is there a better way to make tiles align to a grid?
    public class TileEntity : Entity
    {
        public TileImpl tileImpl;
        public MaterialImpl materialImpl;

        protected int activeSection = 0;
        protected bool mirrorX = false;
        protected bool mirrorY = false;

        public TileEntity(TileImpl tile, MaterialImpl material)
        {
            tileImpl = tile;
            materialImpl = material;
        }

        // TODO: Is this bad or as best as it can be done?
        // TODO: Perspective wall support?
        public virtual void ChangeAppearance(bool north, bool east, bool south, bool west)
        {
            TileImpl.TileShape shape = TileImpl.TileShape.Single;

            mirrorX = false;
            mirrorY = false;

            if (west || east)
            {
                shape = TileImpl.TileShape.HorizontalCap;
                mirrorX = west;
            }

            if (north || south)
            {
                shape = TileImpl.TileShape.VerticalCap;
                mirrorY = south;
            }


            if (west && east)
                shape = TileImpl.TileShape.Horizontal;

            if (north && south)
                shape = TileImpl.TileShape.Vertical;


            if ((north || south) && (west || east))
            {
                shape = TileImpl.TileShape.Corner;
                mirrorX = west;
                mirrorY = south;
            }


            if (east && west && (north || south))
            {
                shape = TileImpl.TileShape.HorizontalT;
                mirrorY = south;
            }

            if (north && south && (east || west))
            {
                shape = TileImpl.TileShape.VerticalT;
                mirrorX = west;
            }



            if (south && north && west && east)
                shape = TileImpl.TileShape.All;


            activeSection = tileImpl.tileSections.FindIndex(0, (section) => section.shape == shape);

            if (activeSection == -1)
                activeSection = 0;
        }

        public override void Render(SpriteBatch batch, Entity parent = null)
        {
            // We offset depth a bit based on layer
            SpriteDrawing.Draw(
                tileImpl.tileSections[activeSection].texture, 
                localTransform, 
                tileImpl.tileSections[activeSection].rectangle, 
                materialImpl.color,
                LocalDepth - ((float)tileImpl.layer / 100F),
                mirrorX,
                mirrorY
            );

            base.Render(batch, this);
        }
    }
}
