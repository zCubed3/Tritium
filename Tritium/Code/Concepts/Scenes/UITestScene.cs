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
    public class UITestScene : Scene
    {
        DrawingLayer testingLayer = new DrawingLayer();
        Camera camera;

        UIText textTest;
        UIButton returnButton;
        UITitleBar windowHandle;

        public override void SetupScene()
        {
            camera = new Camera();
            AddEntities(camera);

            returnButton = new UIButton("Exit");
            returnButton.onClick += () => { NextScene = new MainMenuScene(); };
            AddEntities(returnButton);

            windowHandle = new UITitleBar("Test Window");
            windowHandle.Scale = new Vector2(200, 40);
            AddEntities(windowHandle);

            textTest = new UIText();
            windowHandle.Parent(textTest);
        }

        char test = '\0';
        float interval = 0;

        public override void UpdateScene()
        {
            base.UpdateScene();

            if (Timing.elapsedTime > interval) 
            {
                test++;
                interval = Timing.elapsedTime + 0.5F;
            }

            if (test > 64)
            {
                test = '\0';
            }

            float pt = MathF.Abs(Timing.sinTime.X) * 128;
            pt = 32;

            string str = "";
            for (char c = 'A'; c <= 'z'; c++)
                str += c;

            textTest.Position = new Vector2(0, 3F);
            textTest.text = "";
            textTest.textScale = pt;

            returnButton.Scale = new Vector2(100, 40);
            returnButton.transform.origin = Vector2.One / 2F;
            returnButton.Position = new Vector2(-Camera.ReferenceResolutionX / 2F, -Camera.ReferenceResolutionY / 2F);
            returnButton.Position += returnButton.Scale / 2F;

            camera.Zoom = 10;
            camera.CameraUpdate(TritiumGame.Instance.GraphicsDevice.Viewport);

            UpdateSceneUI(camera.UIScreenMatrix);

            str = "Hi friend!\nHola amigo!\nBonjour ami!\nПривет друг!";
            ScalableFont.LoadedFonts["Roboto"].Draw(str, new Vector3(textTest.LocalPosition, -0.9F), Quaternion.CreateFromYawPitchRoll(0, 0, Timing.cosTime.X), Vector2.One, (Timing.sinTime.X + 1) / 2F, 0.5F, pt, Color.White);
        }

        public override void RenderScene(SpriteBatch spriteBatch)
        {
            using (testingLayer.BeginScoped(spriteBatch, camera.UIScreenMatrix))
            {
                base.RenderScene(spriteBatch);
            }

            ScalableFont.RenderAll(spriteBatch, camera.UIViewProjectionMatrix);
        }
    }
}
