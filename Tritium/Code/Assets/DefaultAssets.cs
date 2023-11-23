using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium.Assets
{
    public static class DefaultAssets
    {
        public static SpriteFont DefaultFont;
        public static ScalableFont DefaultMSDFFont;

        public static Texture2D DefaultTexture;
        public static Texture2D DefaultUITexture;
        public static Texture2D TritiumLogo;

        internal static void LoadDefaultAssets(ContentManager content)
        {
            DefaultFont = content.Load<SpriteFont>("DefaultFont");
            DefaultTexture = content.Load<Texture2D>("DefaultTexture");
            DefaultUITexture = content.Load<Texture2D>("DefaultUITexture");
            TritiumLogo = content.Load<Texture2D>("TritiumLogo");

            DefaultMSDFFont = ScalableFont.Load("Roboto");
        }
    }
}
