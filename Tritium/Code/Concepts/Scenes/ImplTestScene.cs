using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Tritium.Drawing;
using Tritium.Input;
using Tritium.Concepts.Grid;
using Tritium.Concepts.UI;
using Tritium.Concepts.Pawns;
using Tritium.Assets;
using Tritium.Concepts.Chem;
using Tritium.VFX;

namespace Tritium.Concepts.Scenes
{
    public sealed class ImplTestScene : Scene
    {
        private DrawingLayer testingLayer = new DrawingLayer();
        private Camera camera;

        private UIButton returnButton;

        private SpaceVFX spaceVFX = null;

        private List<ChemicalContainerEntity> containers = new List<ChemicalContainerEntity>();

        public override void SetupScene()
        {
            camera = new Camera();
            AddEntities(camera);

            returnButton = new UIButton("Exit");
            returnButton.onClick += () => { NextScene = new MainMenuScene(); };
            AddEntities(returnButton);

            spaceVFX = new SpaceVFX();
            
            //
            // Various tests
            //
            foreach (Impl impl in CargoBay.GetAllImpls())
            {
                //
                // Chem system
                //
                if (impl is ChemicalContainerImpl chemContainerImpl)
                {
                    ChemicalContainerEntity containerEntity = new ChemicalContainerEntity();
                    containerEntity.containerImpl = chemContainerImpl;

                    containerEntity.Scale = Vector2.One * 100;
                    containerEntity.Position = new Vector2(110 * containers.Count, 0);

                    containers.Add(containerEntity);
                    
                    AddEntities(containerEntity);
                }

                if (impl is ChemicalImpl chemImpl)
                {
                    
                }
            }
        }
        
        public override void UpdateScene()
        {
            base.UpdateScene();

            returnButton.Scale = new Vector2(100, 40);
            returnButton.transform.origin = Vector2.One / 2F;
            returnButton.Position = new Vector2(-Camera.ReferenceResolutionX / 2F, -Camera.ReferenceResolutionY / 2F);
            returnButton.Position += returnButton.Scale / 2F;

            camera.interpolateAngle = false;
            camera.interpolatePos = false;
            camera.interpolateZoom = false;
            camera.Zoom = 10;
            camera.CameraUpdate(TritiumGame.Instance.GraphicsDevice.Viewport);

            UpdateSceneUI(camera.UIScreenMatrix);
        }

        public override void RenderScene(SpriteBatch spriteBatch)
        {
            spaceVFX.Render(spriteBatch, camera.SpriteViewMatrix, camera.Position);

            using (testingLayer.BeginScoped(spriteBatch, camera.UIScreenMatrix))
            {
                base.RenderScene(spriteBatch);
            }

            ScalableFont.RenderAll(spriteBatch, camera.UIViewProjectionMatrix);
        }
    }
}
