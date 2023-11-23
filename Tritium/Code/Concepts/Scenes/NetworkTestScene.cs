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
using Tritium.Networking;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.MonoGame.DebugView;

namespace Tritium.Concepts.Scenes
{
    public class NetworkTestScene : Scene
    {
        DrawingLayer drawingLayer = new DrawingLayer() { samplerState = SamplerState.PointWrap };

        Camera camera;
        UIButton startServer, startClient, returnButton;

        ITransportLayer layer;

        bool server = false;

        public void CreateSocket()
        {
            layer = new UDPLayer();
            layer.Connect("udp://127.0.0.1:9000");
        }

        public override void SetupScene()
        {
            camera = new Camera();
            AddEntities(camera);

            startServer = new UIButton("Start Server");
            startServer.Scale = new Vector2(250, 40);
            startServer.onClick += CreateSocket;


            startClient = new UIButton("Start Client");
            startClient.Scale = new Vector2(250, 40);
            startClient.onClick += CreateSocket;

            returnButton = new UIButton("Exit");
            returnButton.onClick += () => { NextScene = new MainMenuScene(); };

            AddEntities(startServer, startClient, returnButton);
        }

        public override void UpdateScene()
        {
            if (layer != null)
            {
                var packets = layer.ReceivePackets();

                startClient.Text = $"{packets.Count}";

                startServer.visible = false;
                startClient.visible = true;

                using (var packet = layer.CreatePacket())
                {
                    packet.Write(Encoding.ASCII.GetBytes("Hola!"));
                }
            }

            base.UpdateScene();

            returnButton.Scale = new Vector2(100, 40);
            returnButton.transform.origin = Vector2.One / 2F;
            returnButton.Position = new Vector2(-Camera.ReferenceResolutionX / 2F, -Camera.ReferenceResolutionY / 2F);
            returnButton.Position += returnButton.Scale / 2F;

            float length = 240F;
            float per = length / 3.0F;

            startServer.Position = new Vector2(0, per * -1);
            startClient.Position = new Vector2(0, per * 0);

            camera.CameraUpdate(TritiumGame.Instance.GraphicsDevice.Viewport);
            UpdateSceneUI(camera.UIScreenMatrix);
        }

        public override void RenderScene(SpriteBatch spriteBatch)
        {
            using (drawingLayer.BeginScoped(spriteBatch, camera.UIScreenMatrix))
            {
                base.RenderScene(spriteBatch);
            }

            ScalableFont.RenderAll(spriteBatch, camera.UIViewProjectionMatrix);
        }
    }
}
