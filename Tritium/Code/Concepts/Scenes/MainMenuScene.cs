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

namespace Tritium.Concepts.Scenes
{
    public class MainMenuScene : Scene
    {
        protected Camera camera;

        protected UIButton playButton, uiTestButton, physicsTestButton, implTestButton;

        protected DrawingLayer drawingLayer = new DrawingLayer();

        protected VFX.SpaceVFX spaceVFX;

        protected UIButton logoButton;
        protected bool rollButton;
        protected float rollAnimate;

        public override void SetupScene()
        {
            camera = new Camera();
            AddEntities(camera);

            playButton = new UIButton("Load GameScene");
            playButton.Scale = new Vector2(250, 60);
            playButton.onClick += () => { NextScene = new GameScene(); };

            uiTestButton = new UIButton("Load UITestScene");
            uiTestButton.Scale = new Vector2(250, 60);
            uiTestButton.onClick += () => { NextScene = new UITestScene(); };

            physicsTestButton = new UIButton("Load PhysicsTestScene");
            physicsTestButton.Scale = new Vector2(250, 60);
            physicsTestButton.onClick += () => { NextScene = new PhysicsTestScene(); };

            implTestButton = new UIButton("Load ImplTestScene");
            implTestButton.Scale = new Vector2(250, 60);
            implTestButton.onClick += () => { NextScene = new ImplTestScene(); };

            logoButton = new UIButton(Color.White);
            logoButton.texture = DefaultAssets.TritiumLogo;
            logoButton.Scale = new Vector2(80, 80);
            logoButton.Position = new Vector2(0, -Camera.ReferenceResolutionY / 2F + 100F);
            logoButton.baseColor = logoButton.hoverColor = logoButton.clickColor = Color.White;
            logoButton.onClick += () => { rollButton = !rollButton; };

            spaceVFX = new VFX.SpaceVFX();

            AddEntities(playButton, uiTestButton, physicsTestButton, implTestButton, logoButton);
        }

        public override void UpdateScene()
        {
            base.UpdateScene();

            Viewport viewport = TritiumGame.Instance.GraphicsDevice.Viewport;

            // TODO: Make UIEntities have anchors relative to the viewport?

            float length = 240F;
            float per = length / 3.0F;

            playButton.Position = new Vector2(0, per * -1);
            uiTestButton.Position = new Vector2(0, per * 0);
            physicsTestButton.Position = new Vector2(0, per * 1);
            implTestButton.Position = new Vector2(0, per * 2);

            //uiTestButton.Scale = new Vector2(MathF.Abs(Timing.sinTime.X) * 320, 60);

            rollAnimate = TritMath.Clamp(rollAnimate + (Timing.deltaTime * (rollButton ? 1 : -1)));

            DefaultAssets.DefaultMSDFFont.Draw(" ritium", logoButton.Position, 0F, 0F, new Vector2(rollAnimate, 1), 0F, 0.8F, 64F, new Color(0, 227, 143), false);
            logoButton.Angle = TritMath.Lerp(0, MathF.PI * 2F, rollAnimate);

            camera.Zoom = 5;
            camera.zoomInterpolationRate = 1F;
            camera.CameraUpdate(TritiumGame.Instance.GraphicsDevice.Viewport);

            UpdateSceneUI(camera.UIScreenMatrix);
        }

        public override void RenderScene(SpriteBatch spriteBatch)
        {
            spaceVFX.Render(spriteBatch, camera.SpriteViewMatrix, camera.Position);

            using (new ScopedLayer(spriteBatch, drawingLayer, camera.UIScreenMatrix))
            {
                base.RenderScene(spriteBatch);
            }

            ScalableFont.RenderAll(spriteBatch, camera.UIViewProjectionMatrix);
        }
    }
}
