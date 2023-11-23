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

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.MonoGame.DebugView;

namespace Tritium.Concepts.Scenes
{
    public class PhysicsTestScene : Scene
    {
        DrawingLayer drawingLayer = new DrawingLayer() { samplerState = SamplerState.PointWrap };

        Camera camera;

        // TODO: Move prototype physics code out of here

        World world = null;
        DebugView debugView = null;

        UIButton returnButton;

        Entity worldRoot;

        protected class TilePair
        {
            public TileEntity tile;
            public Body body;

            public TilePair(TileEntity tile, Body body)
            {
                this.tile = tile;
                this.body = body;
            }

            public void Update()
            {
                tile.Position = body.Position;
                tile.Angle = body.Rotation;
            }
        }

        protected TilePair playerBody;
        protected List<TilePair> pairs = new List<TilePair>();

        GridEntity basicShip;

        VFX.SpaceVFX spaceVFX;
        float scroll;

        IPawnController controller = new DesktopPawnController();
        Body shipBody;

        public override void SetupScene()
        {
            camera = new Camera();
            AddEntities(camera);

            returnButton = new UIButton("Exit");
            returnButton.onClick += () => { NextScene = new MainMenuScene(); };
            AddEntities(returnButton);

            world = new World(Vector2.Zero); // SPACE!
            debugView = new DebugView(world);
            debugView.LoadContent(TritiumGame.Instance.GraphicsDevice, TritiumGame.Instance.Content);

            CargoBay.TryGetImpl("tritium.core.content", "TiledFloorTile", out TileImpl tileImpl);
            CargoBay.TryGetImpl("tritium.core.content", "BaseCeramicMaterial", out MaterialImpl materialImpl);

            worldRoot = new Entity();

            ConvertUnits.SetDisplayUnitToSimUnitRatio(Camera.PixelToWorldRatio);

            TileEntity playerTile = new TileEntity(tileImpl, materialImpl);
            playerTile.transform.depth -= 0.1F;
            playerBody = new TilePair(playerTile, BodyFactory.CreateRectangle(world, 1F, 1F, 1F, default, 0, BodyType.Dynamic));

            BodyFactory.CreateBody(world, Vector2.Zero);

            basicShip = new GridEntity();
            worldRoot.Parent(basicShip);

            //basicShip.Parent(testThing);

            /*
            // We need to start at 0 and create from there
            for (int x = 0; x <= 5; x++)
            {
                for (int y = 0; y <= 5; y++)
                {
                    basicShip.TryAddTile(new TileEntity(tileImpl, materialImpl), x, y, false);
                }
            }

            for (int x = -1; x <= 5; x++)
            {
                basicShip.TryAddTile(new TileEntity(tileImpl, materialImpl), x, -1, false);
            }

            for (int y = -1; y <= 5; y++)
            {
                basicShip.TryAddTile(new TileEntity(tileImpl, materialImpl), -1, y, false);
            }

            CargoBay.TryGetImpl("tritium.core.content", "MetalWallTile", out TileImpl tileImpl2);
            CargoBay.TryGetImpl("tritium.core.content", "BaseMetalMaterial", out MaterialImpl materialImpl2);

            for (int x = -1; x <= 5; x++)
            {
                for (int y = -1; y <= 5; y++)
                {
                    if (x != -1 && x != 5 && y != -1 && y != 5)
                        continue;

                    basicShip.TryAddTile(new TileEntity(tileImpl2, materialImpl2), x, y, false);
                }
            }

            basicShip.CalculateBounds();
            basicShip.CalculateEnclosures();
            basicShip.ConnectAllTiles();

            int sY = basicShip.rowShift;
            shipBody = BodyFactory.CreateBody(world, Vector2.Zero, 0, BodyType.Dynamic);

            foreach (GridEntity.GridRow row in basicShip.rows)
            {
                int x = row.min;
                foreach (var dict in row.rowTiles)
                {
                    if (dict.TryGetValue(TileImpl.TileLayer.Wall, out TileEntity entity))
                    {
                        //Body body = BodyFactory.CreateRectangle(world, 1, 1, 1, new Vector2(x, sY), 0, BodyType.Dynamic);
                        //TilePair pair = new TilePair(entity, body);

                        FixtureFactory.AttachRectangle(1, 1, 1, new Vector2(x, sY), shipBody);
                    }

                    x++;
                }

                sY++;
            }
            */

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var testTile = new TileEntity(tileImpl, materialImpl);
                    pairs.Add(new TilePair(testTile, BodyFactory.CreateRectangle(world, 1, 1, 1, new Vector2(x, y) + Vector2.One, 0, BodyType.Dynamic)));

                    worldRoot.Parent(testTile);
                }
            }

            worldRoot.Parent(playerTile);
            //AddEntities(testTile);

            spaceVFX = new VFX.SpaceVFX();
        }

        public override void UpdateScene()
        {
            world.Step(Timing.deltaTime);

            //basicShip.Position = shipBody.Position;
            //basicShip.Angle = shipBody.Rotation;

            base.UpdateScene();

            playerBody.Update();

            foreach (var p in pairs)
                p.Update();


            Vector2 heading = controller.GetHeading() * 5;
            playerBody.body.ApplyForce(heading);

            worldRoot.Update();

            camera.Position = playerBody.tile.Position;
            camera.Angle = 0;
            camera.Zoom = 1 + scroll;
            camera.angleInterpolationRate = 10;
            camera.posInterpolationRate = 10;

            scroll = TritMath.Clamp(scroll + InputSystem.mouseInfo.scrollDelta.Y, -5F, 5F);

            returnButton.Scale = new Vector2(100, 40);
            returnButton.transform.origin = Vector2.One / 2F;
            returnButton.Position = new Vector2(-Camera.ReferenceResolutionX / 2F, -Camera.ReferenceResolutionY / 2F);
            returnButton.Position += returnButton.Scale / 2F;

            camera.CameraUpdate(TritiumGame.Instance.GraphicsDevice.Viewport);
            UpdateSceneUI(camera.UIScreenMatrix);
        }

        public override void RenderScene(SpriteBatch spriteBatch)
        {
            spaceVFX.Render(spriteBatch, camera.SpriteViewMatrix, camera.Position);

            using (drawingLayer.BeginScoped(spriteBatch, camera.SpriteViewMatrix))
            {
                worldRoot.Render(spriteBatch);
            }

            using (drawingLayer.BeginScoped(spriteBatch, camera.UIScreenMatrix))
            {
                base.RenderScene(spriteBatch);
            }

            debugView.RenderDebugData(camera.ProjectionMatrix, camera.ViewMatrix);

            ScalableFont.RenderAll(spriteBatch, camera.UIViewProjectionMatrix);
        }
    }
}
