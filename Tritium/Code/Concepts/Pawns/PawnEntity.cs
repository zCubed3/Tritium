using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Tritium.Drawing;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium.Concepts.Pawns
{
    public class PawnEntity : Entity
    {
        // By default all stats are 0! We use stat mods to boost the pawn's stats
        public enum PawnStat
        {
            Movement,
            Manipulation
        }

        [Impl.SubData]
        public class PawnStatMod 
        {
            public PawnStat stat;
            public float statOffset;
        }

        [Impl.SubData]
        public class PawnFeatureLayout
        {
            public string featureLocation;
            public Impl.StrictImplRef<PawnFeatureImpl> feature;
            public bool mirrorX = false;
            public bool mirrorY = false;

            public List<PawnFeatureLayout> childFeatures = new List<PawnFeatureLayout>();

            // TODO: Optimization?
            public void CalculateStats(ref Dictionary<PawnStat, float> stats)
            {
                PawnFeatureImpl featureImpl = feature;

                foreach (PawnStatMod mod in featureImpl.statMods)
                    stats[mod.stat] += mod.statOffset;

                foreach (PawnFeatureLayout childFeature in childFeatures)
                    childFeature.CalculateStats(ref stats);
            }

            public void Render(SpriteBatch batch, Entity parent)
            {
                PawnFeatureImpl featureImpl = feature;

                // TODO: Better calculate feature offsets
                Transform2D featureTransform = new Transform2D();

                featureTransform.origin = featureImpl.origin;
                featureTransform.Position = featureImpl.position;
                featureTransform.Scale = featureImpl.scale;
                featureTransform.ApplyMirror(mirrorX, mirrorY);
                featureTransform = featureTransform.Transform(parent.localTransform);

                SpriteDrawing.Draw(
                    featureImpl.sprite.texture,
                    featureTransform,
                    featureImpl.sprite.rect,
                    Color.White,
                    featureImpl.depth,
                    mirrorX,
                    mirrorY
                );

                foreach (PawnFeatureLayout childFeature in childFeatures)
                    childFeature.Render(batch, parent);
            }
        }

        public PawnEntity(PawnImpl impl) => pawnImpl = impl;


        // TODO: Pawn AI?
        public IPawnController controller = new NullPawnController();
        public Dictionary<PawnStat, float> pawnStats = null;
        public PawnImpl pawnImpl = null;

        // Anatomy is copied from the Impl because some pawns can lose limbs and such
        public PawnFeatureLayout anatomy = null;

        public override void Update()
        {
            if (pawnStats == null)
            {
                pawnStats = new Dictionary<PawnStat, float>();

                foreach (PawnStat val in Enum.GetValues<PawnStat>())
                    pawnStats.Add(val, 0F);
            }

            if (anatomy == null)
                anatomy = pawnImpl.anatomy;

            // TODO: Dirty flag optimization!
            foreach (var key in pawnStats.Keys)
                pawnStats[key] = 0F;

            anatomy.CalculateStats(ref pawnStats);

            Vector2 heading = controller.GetHeading();
            heading = Vector2.TransformNormal(heading, transform.LocalToWorld).SafeNormalize();
            heading *= Timing.deltaTime * pawnStats[PawnStat.Movement];

            transform.Position += heading;

            base.Update();
        }

        public override void Render(SpriteBatch batch, Entity parent = null)
        {
            // We need to render the whole anatomy of this pawn
            // TODO: Make a convenient drawing system?
            anatomy.Render(batch, this);

            base.Render(batch, parent);
        }
    }
}
