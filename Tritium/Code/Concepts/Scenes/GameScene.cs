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
    // TODO: Make more scenes?
    // Eg. Editor, Mod Menu, etc?
    public class GameScene : Scene
    {
        UIEntity uiContainer = null;
        UIPanel panel = null, panel2 = null;
        UIText text = null;
        UIText consoleLog = null;

        Entity worldRoot = null;
        Entity tileRoot = null;
        UIButton returnButton = null;
        UIPanel testThing = null;
        PawnEntity pawnEntity = null;

        Camera camera = new Camera();

        // TODO: Better organize drawing code into "screens"
        DrawingLayer spriteLayer = new DrawingLayer() { samplerState = SamplerState.PointWrap };
        DrawingLayer uiLayer = new DrawingLayer();

        GridEntity basicShip;

        VFX.SpaceVFX space;

        bool parentShip = true;

        public override void SetupScene()
        {
            uiContainer = new UIEntity();
            //panel = new UIButton("HOLA");
            panel2 = new UIPanel();
            text = new UIText() { text = "HOLA" };
            consoleLog = new UIText();

            returnButton = new UIButton("Exit");
            returnButton.onClick += () => { NextScene = new MainMenuScene(); };

            AddEntities(text, consoleLog, returnButton);

            worldRoot = new Entity();
            tileRoot = new Entity();

            worldRoot.Parent(panel2);

            CargoBay.TryGetImpl("tritium.core.content", "DevPawn", out PawnImpl pawnImpl);
            pawnEntity = new PawnEntity(pawnImpl);

            worldRoot.Parent(tileRoot);

            // TODO: Pawn selection
            pawnEntity.controller = new DesktopPawnController();

            testThing = new UIPanel();
            testThing.Scale /= 10;
            testThing.Depth = 0.1F;

            worldRoot.Parent(testThing);

            CargoBay.TryGetImpl("tritium.core.content", "TiledFloorTile", out TileImpl tileImpl);
            CargoBay.TryGetImpl("tritium.core.content", "BaseCeramicMaterial", out MaterialImpl materialImpl);

            basicShip = new GridEntity();
            worldRoot.Parent(basicShip);
            basicShip.Parent(pawnEntity);

            //basicShip.Parent(testThing);

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

            space = new VFX.SpaceVFX();
        }

        public override void UpdateScene()
        {
            base.UpdateScene();

            Vector2 scale = Vector2.One * 1;

            tileRoot.Position = new Vector2(MathF.Sin(Timing.elapsedTime), 0);

            panel2.Scale = Vector2.One * 1;

            worldRoot.Update();

            camera.Position = pawnEntity.LocalPosition;
            camera.Angle = -pawnEntity.LocalAngle;
            camera.Zoom = 7;
            camera.angleInterpolationRate = 10;
            camera.zoomInterpolationRate = 10;

            camera.interpolatePos = false;
            camera.interpolateAngle = false;
            camera.interpolateZoom = false;

            panel2.Depth = 0;

            camera.Update();
            camera.CameraUpdate(TritiumGame.Instance.GraphicsDevice.Viewport);

            Vector2 worldClick = camera.ScreenToWorld(InputSystem.mouseInfo.position);

            Matrix localToWorld = basicShip.localTransform.LocalToWorld;
            Matrix worldToLocal = basicShip.localTransform.WorldToLocal;
            //worldClick = Vector2.Transform(worldClick, mat);

            Vector2 localClick = Vector2.Transform(worldClick, worldToLocal);
            localClick = Vector2.Round(localClick);

            worldClick = Vector2.Transform(localClick, localToWorld);

            text.Position = new Vector2(-Camera.ReferenceResolutionX / 2F, Camera.ReferenceResolutionY / 2F);
            text.Position += Vector2.UnitY * text.textScale;

            //text.text = $"{pawnEntity.LocalPosition}\n{camera.LocalPosition}";
            text.text = $"{worldClick}";
            text.valign = 1F;
            text.textScale = 16F;
            text.Depth = 0.1F;

            basicShip.Position += Vector2.One * 0.01F;
            basicShip.Angle += Timing.deltaTime * 0.1F;
            if (InputSystem.mouseInfo.leftButton.justPressed)
            {
                CargoBay.TryGetImpl("tritium.core.content", "MetalWallTile", out TileImpl tileImpl);
                CargoBay.TryGetImpl("tritium.core.content", "BaseCeramicMaterial", out MaterialImpl materialImpl);

                bool placed = basicShip.TryAddTile(new TileEntity(tileImpl, materialImpl), (int)localClick.X, (int)localClick.Y);
                panel2.color = placed ? Color.Green : Color.Red;
                panel2.color.A = 0x1A;

            }

            Vector2 shipLocal = Vector2.Transform(pawnEntity.LocalPosition, worldToLocal);
            Matrix playerLocalToWorld = pawnEntity.localTransform.LocalToWorld;

            Vector2 playerMinCorner = Vector2.Transform(Vector2.One / -4, playerLocalToWorld);
            Vector2 playerMaxCorner = Vector2.Transform(Vector2.One / 4, playerLocalToWorld);

            Vector2 roundLocal = Vector2.Round(shipLocal);

            // We need to convert the AABB to world space
            Vector2 shipMinCorner = Vector2.Transform(playerMinCorner, worldToLocal);
            Vector2 shipMaxCorner = Vector2.Transform(playerMaxCorner, worldToLocal);

            AABB playerAABB = new AABB(shipMinCorner, shipMaxCorner);

            if (basicShip.shipAABB.OverlapsAABB(playerAABB))
            {
                if (basicShip.TryGetTile((int)roundLocal.X, (int)roundLocal.Y, TileImpl.TileLayer.Floor, out TileEntity tile))
                {
                    if (tile != null && !parentShip)
                    {
                        basicShip.Parent(pawnEntity);
                        parentShip = true;
                    }

                    if (tile == null && parentShip)
                    {
                        worldRoot.Parent(pawnEntity);
                        parentShip = false;
                    }
                }
                else
                {
                    if (parentShip)
                    {
                        worldRoot.Parent(pawnEntity);
                        parentShip = false;
                    }
                }
            }
            else
            {
                if (parentShip)
                {
                    worldRoot.Parent(pawnEntity);
                    parentShip = false;
                }
            }

            basicShip.Angle = TritMath.Wrap(basicShip.Angle, 0, MathF.PI * 2);
            pawnEntity.Angle = TritMath.Wrap(pawnEntity.Angle, 0, MathF.PI * 2);
            pawnEntity.Angle = TritMath.LerpAngle(pawnEntity.Angle, 0, Timing.deltaTime * 5);

            text.text = "";
            //text.text = $"{worldClick}\n{localClick}";
            //foreach (GridEntity.GridRow row in basicShip.Rows)
            //    text.text += $"oX = {row.min}; \n";
            //
            //text.text += $"{shipLocal};\n";
            //text.text += $"{basicShip.BoundingBox.min}; {basicShip.BoundingBox.max}\n";
            //text.text += $"{basicShip.Angle};\n";
            text.text += $"{shipMinCorner};\n{shipMaxCorner}\n\n";
            text.text += $"{playerAABB.min};\n{playerAABB.max}\n";

            // TODO: Optimize this ffs!
            // TODO: Implement console rendering!
            //consoleLog.text = "";
            //foreach (var entry in TritiumConsole.Instance.entries)
            //    consoleLog.text += entry.message;

            panel2.Position = worldClick;
            panel2.Angle = -camera.Angle;

            testThing.Position = playerMaxCorner;

            Camera.ActiveCamera = camera;

            returnButton.Scale = new Vector2(100, 40);
            returnButton.transform.origin = Vector2.One / 2F;
            returnButton.Position = new Vector2(-Camera.ReferenceResolutionX / 2F, -Camera.ReferenceResolutionY / 2F);
            returnButton.Position += returnButton.Scale / 2F;

            UpdateSceneUI(camera.UIScreenMatrix);

            foreach (GridEntity.GridEnclosure enclosure in basicShip.enclosures)
            {
                
            }
        }

        public override void RenderScene(SpriteBatch spriteBatch)
        {
            if (Camera.ActiveCamera != null)
                space.Render(spriteBatch, Camera.ActiveCamera.SpriteViewMatrix, Camera.ActiveCamera.Position);
            
            using (new ScopedLayer(spriteBatch, spriteLayer, Camera.ActiveCamera?.SpriteViewMatrix))
            {
                worldRoot.Render(spriteBatch);
            }

            using (new ScopedLayer(spriteBatch, uiLayer))
                uiContainer.Render(spriteBatch);

            using (new ScopedLayer(spriteBatch, uiLayer, Camera.ActiveCamera?.UIScreenMatrix))
            {
                base.RenderScene(spriteBatch);
            }

            // TODO: Fix MSDF font overlapping with sprites
            ScalableFont.RenderAll(spriteBatch, Camera.ActiveCamera.UIViewProjectionMatrix);
        }
    }
}
