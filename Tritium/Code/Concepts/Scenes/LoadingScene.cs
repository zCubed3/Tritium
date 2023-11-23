using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Drawing;
using Tritium.Concepts.Grid;
using Tritium.Concepts.UI;
using Tritium.Assets;

namespace Tritium.Concepts.Scenes
{
    // Called when the XDepot is working alongside the CargoBay
    // Has very basic visuals
    public class LoadingScene : Scene
    {
        public DrawingLayer sceneLayer = new DrawingLayer() { samplerState = SamplerState.LinearWrap };

        protected Camera camera;
        protected UIText headerText, subText;
         
        protected Thread loadThread;
         
        protected string section = "N/A";
         
        protected float lastTick = 0F;
        protected string throbber = "";
        protected string subState = "";

        protected bool finished = false;
        protected float finishDelay = 0;

        public override void SetupScene()
        {
            camera = new Camera();
            AddEntities(camera);

            headerText = new UIText("HEADER", ScalableFont.HAlignment.Center, ScalableFont.VAlignment.Middle);
            headerText.textScale = 32F;
            AddEntities(headerText);

            subText = new UIText("SUBTEXT", ScalableFont.HAlignment.Center, ScalableFont.VAlignment.Middle);
            subText.Position += new Vector2(0, 200F);
            AddEntities(subText);

            loadThread = new Thread(() => 
            {
                section = "Loading Packages";
                CargoBay.LoadPackages(ref subState);

#if DEBUG
                section = "Dumping Debug Info";
                CargoBay.DumpDebugInfo();
#endif

                section = "Done";
                subState = "";

                finishDelay = Timing.elapsedTime + 0.6F;
                finished = true;
            });

            loadThread.Start();
        }

        public override void UpdateScene()
        {
            base.UpdateScene();

            if (Timing.elapsedTime > lastTick)
            {
                throbber += '.';
                lastTick = Timing.elapsedTime + 0.3F;
            }

            if (throbber.Length > 3)
                throbber = "";

            if (finished)
                throbber = "!";

            if (loadThread.ThreadState == ThreadState.Stopped && finished && Timing.elapsedTime > finishDelay)
                NextScene = new MainMenuScene();

            headerText.text = $"{section}{throbber}";
            subText.text = subState;

            camera.CameraUpdate(TritiumGame.Instance.GraphicsDevice.Viewport);
        }

        public override void RenderScene(SpriteBatch spriteBatch)
        {
            using (new ScopedLayer(spriteBatch, sceneLayer, camera.SpriteViewMatrix))
            {
                base.RenderScene(spriteBatch);
            }

            ScalableFont.RenderAll(spriteBatch, camera.UIViewProjectionMatrix);
        }
    }
}
