using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Tritium.Assets;

namespace Tritium.Concepts.Pawns
{
    // A feature is something that resides at a certain point on a pawns body
    // Some features are removable, others are not!
    // Features are singular objects, pawns setting up features will connect them however their anatomy permits
    // TODO: Sprites actually connected based on the parent point location
    public class PawnFeatureImpl : Impl
    {
        public PawnFeatureImpl() : base() { }
        public PawnFeatureImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }


        public string featureName;
        public bool removable = true;

        public Sprite sprite;

        public Vector2 position = Vector2.Zero;
        public Vector2 origin = Vector2.One / 2F;
        public Vector2 scale = Vector2.One;
        public float depth = 0.5F; // TODO: How far forward is too far?

        public List<PawnEntity.PawnStatMod> statMods = new List<PawnEntity.PawnStatMod>();
    }
}
