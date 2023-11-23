using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Concepts.UI
{
    public class UIConsole : UITitleBar
    {
        public UIConsole() : base("Console") { }

        public override void UIUpdate(Matrix? worldToScreenMatrix)
        {
            if (enabled)
                base.UIUpdate(worldToScreenMatrix);
        }

        public override void Render(SpriteBatch batch, Entity parent)
        {
            if (visible)
                base.Render(batch, parent);
        }
    }
}
