using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Tritium.Assets
{
    public abstract class ContentRef<T> : GenericRef<T> where T : class
    {
        public ContentRef() : base() 
        {
            softContainer = CargoBay.ActivePackage.id;
        }

        public ContentRef(string path) : base(path) 
        {
            ChangePath(path);
        }


        public static implicit operator T(ContentRef<T> content) => content.Resolve();
    }

    public class Texture2DCRef : ContentRef<Texture2D>
    {
        public Texture2DCRef() : base() { }
        public Texture2DCRef(string path) : base(path) { }

        protected override Texture2D FetchReference()
        {
            ResolveContainer();

            try
            {
                return container.packageContent.cache[softRef] as Texture2D;
            }
            catch
            {
                var texture = Texture2D.FromFile(TritiumGame.Instance.GraphicsDevice, GetPath());
                container.packageContent.cache.Add(softRef, texture);
                return texture;
            }
        }
    }
}
