using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tritium.Concepts;
using Tritium.Drawing;
using Tritium.Assets;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium.VFX
{
    public class SpaceVFX
    {
        public static DrawingLayer SpaceDrawingLayer = new DrawingLayer() { blendState = BlendState.Additive };

        public class SpaceVisualLayer
        {
            public SpaceVisualLayer(SpaceVisualLayerImpl layerImpl)
            {
                this.layerImpl = layerImpl;

                offset = TritRandom.RandomDirection() * TritRandom.RandomRange(100, 1000);
                depth = TritRandom.RandomRange(layerImpl.minDepth, layerImpl.maxDepth);
                angle = TritRandom.RandomAngle();
                idleMovement = TritRandom.RandomDirection();
            }
            
            public SpaceVisualLayerImpl layerImpl = null;
            public Vector2 offset = Vector2.Zero;
            public Vector2 idleMovement = Vector2.UnitX;
            public float depth = 0;
            public float angle = 0;
        }

        public List<SpaceVisualLayer> decidedLayers = new List<SpaceVisualLayer>();
        public Vector2 origin = Vector2.Zero;

        public static int LayerCount => 32;

        public SpaceVFX()
        {
            var layers = CargoBay.GetAllImpls().Where(impl => impl is SpaceVisualLayerImpl).ToList();

            // TODO: Frequency
            for (int l = 0; l < LayerCount; l++)
            {
                var impl = layers.PickRandom() as SpaceVisualLayerImpl;
                decidedLayers.Add(new SpaceVisualLayer(impl));
            }

            origin = TritRandom.RandomDirection() * 100;
        }

        public void Render(SpriteBatch batch, Matrix viewMatrix, Vector2 view)
        {
            using (SpaceDrawingLayer.BeginScoped(batch, viewMatrix))
            {
                Transform2D layerTransform = new Transform2D() { Position = view, Scale = Vector2.One * 1000 };
                foreach (var layer in decidedLayers)
                {
                    Rectangle rect = layer.layerImpl.textureRect;
                    rect = rect.Scale(Vector2.One * layer.layerImpl.repetition);

                    Color color = Color.Lerp(Color.Black, Color.White, layer.layerImpl.brightness);
                    layerTransform.Position = Vector2.Lerp(view, origin + layer.offset, layer.depth);
                    layerTransform.Position += (layer.idleMovement * Timing.elapsedTime * 0.01F);
                    layerTransform.Angle = layer.angle;

                    SpriteDrawing.Draw(layer.layerImpl.texture, layerTransform, rect, color);
                }
            }
        }
    }
}
