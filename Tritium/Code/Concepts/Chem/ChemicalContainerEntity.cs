using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

using Tritium.Drawing;

namespace Tritium.Concepts.Chem
{
    public class ChemicalContainerEntity : Entity
    {
        public ChemicalContainerImpl containerImpl = null;
        public List<Chemical> containedChemicals = new List<Chemical>();

        public override void Render(SpriteBatch batch, Entity parent = null)
        {
            if (containerImpl != null)
            {
                SpriteDrawing.Draw(
                    containerImpl.sprite.texture,
                    localTransform,
                    containerImpl.sprite.rect,
                    Color.White,
                    Depth,
                    false,
                    false
                );
            }
            
            base.Render(batch, parent);
        }
    }
}