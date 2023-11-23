using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Drawing;

using Tritium.Assets;

namespace Tritium.Concepts.UI
{
    public class UIPanel : UIEntity
    {
        public Texture2D texture = DefaultAssets.DefaultUITexture;

        public bool customRect = false;
        public Rectangle targetRect;

        public Color color = Color.White;

        protected virtual Color TargetColor => color;

        public Rectangle Rect { get => customRect ? targetRect : texture.Bounds; }

        // TODO: Make this work in world space?
        public virtual bool CursorWithinBounds(Vector2 cursor, Matrix? worldToScreenMatrix)
        {
            Matrix mat = worldToScreenMatrix ?? Matrix.Identity;

            Vector2 corner = (LocalScale * localTransform.origin) + Rect.NormalizeScale(LocalScale);
            Vector2 min = Vector2.Transform(Position - corner, mat);
            Vector2 max = Vector2.Transform(Position + corner, mat);

            return cursor.X > min.X && cursor.Y > min.Y && cursor.X < max.X && cursor.Y < max.Y;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Render(SpriteBatch batch, Entity parent)
        {
            if (visible)
            {
                SpriteDrawing.Draw(texture, localTransform, Rect, TargetColor);
                base.Render(batch, this);
            }
        }
    }
}
