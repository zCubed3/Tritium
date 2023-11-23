using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Tritium.Assets;

namespace Tritium.Concepts
{
    public class SpaceVisualLayerImpl : Impl
    {
        public SpaceVisualLayerImpl() : base() { }
        public SpaceVisualLayerImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }

        public Texture2DCRef texture;
        public Rectangle textureRect;

        /// <summary>
        /// How common is this layer when space is being populated?
        /// </summary>
        public float frequency = 0.5F;

        /// <summary>
        /// How much do we scale the texture by?
        /// </summary>
        public float repetition = 1F;

        /// <summary>
        /// How bright is this layer? Lerps color from Black to White based on value
        /// </summary>
        public float brightness = 1F;

        public float minDepth = 0;
        public float maxDepth = 1;
    }
}
