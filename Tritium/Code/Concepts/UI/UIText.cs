using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Assets;

namespace Tritium.Concepts.UI
{
    public class UIText : UIEntity
    {
        public string text;
        public Color color = Color.White;
        public float textScale = 24.0F;
        public float halign = 0;
        public float valign = 0;
        
        public ScalableFont font = DefaultAssets.DefaultMSDFFont;

        // The spritefont's point size, forces sharper texture sampling
        // We oversample the spritefont to help produce a sharper texture in the end
        // Although we have to compensate and reduce the size down to what we need
        // TODO: Not make this constant?

        public bool ignoreScale = true;

        public UIText() { }

        public UIText(string text, float halign, float valign) 
        {
            this.text = text;
            this.halign = halign;
            this.valign = valign;
        }

        public UIText(string text, ScalableFont.HAlignment halign = ScalableFont.HAlignment.Left, ScalableFont.VAlignment valign = ScalableFont.VAlignment.Top)
        {
            this.text = text;
            this.halign = ScalableFont.GetHAlignWeight(halign);
            this.valign = ScalableFont.GetVAlignWeight(valign);
        }

        public virtual void FitToBounds(Vector2 size)
        {
            Vector2 stringSize = font.MeasureString(text, textScale);
            Scale = size / stringSize;
        }

        public override void Render(SpriteBatch batch, Entity parent)
        {
            if (font != null && !string.IsNullOrEmpty(text))
            {
                float scale = textScale * Scale.X;
                font.Draw(text, LocalPosition, Depth, LocalAngle, Vector2.One, halign, valign, scale, color);
            }

            base.Render(batch, this);
        }
    }
}
