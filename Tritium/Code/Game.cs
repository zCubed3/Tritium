using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;

using Genbox.VelcroPhysics;

using Tritium.Assets;
using Tritium.Input;
using Tritium.Concepts;
using Tritium.Concepts.UI;
using Tritium.Concepts.Pawns;
using Tritium.Concepts.Grid;
using Tritium.Concepts.Scenes;
using Tritium.Logging;
using Tritium.Drawing;

namespace Tritium
{
    public class TritiumGame : Game
    {
        public static TritiumGame Instance { get; private set; }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static StreamLogWriter LogStreamWriter;
        public static LoggerInstance Logger;

        public TritiumGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Instance = this;

            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            Logger = new LoggerInstance("Tritium");

            if (!File.Exists("Tritium.log"))
                File.Create("Tritium.log").Close();

            LogStreamWriter = new StreamLogWriter(File.Open("Tritium.log", FileMode.Truncate));
            Logger.AddWriter(LogStreamWriter);
            Logger.AddWriter(TritiumConsole.Instance);
        }

        protected override void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Logger.Log("Running Initialize()!");

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected bool justChangedScenes = false;
        protected Scene _activeScene;
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                _activeScene = value;
                value.SetupScene();
                justChangedScenes = true;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultAssets.LoadDefaultAssets(Content);
            InputSystem.Initialize();

            ActiveScene = new LoadingScene();
            //ActiveScene = new UITestScene();
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Make things that can circumvent focus loss?
            if (!IsActive)
            {
                base.Update(gameTime);
                return;
            }

            justChangedScenes = false;

            Timing.Update(gameTime);

            InputSystem.Update();

            ActiveScene.UpdateScene();

            if (ActiveScene?.NextScene != null)
            {
                ActiveScene = ActiveScene.NextScene;
                Timing.Reset();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Make things that can circumvent focus loss
            if (!IsActive)
            {
                base.Draw(gameTime);
                return;
            }

            if (ActiveScene != null)
            {
                GraphicsDevice.Clear(ActiveScene.ClearColor);

                if (!justChangedScenes)
                    ActiveScene.RenderScene(_spriteBatch);

                DebugDrawing.RenderDebug(_spriteBatch, Camera.ActiveCamera?.SpriteViewMatrix);
            }

            base.Draw(gameTime);
        }
    }
}