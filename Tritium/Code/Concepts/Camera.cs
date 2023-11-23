using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Tritium.Concepts
{
    // TODO: Not make active camera static here?
    public class Camera : Entity
    {
        public static Camera ActiveCamera = null;

        // 2D View Matrices
        public Matrix UIScreenMatrix { get; private set; }
        public Matrix InverseUIScreenMatrix { get; private set; }
        public Matrix SpriteViewMatrix { get; private set; }
        public Matrix InverseSpriteViewMatrix { get; private set; }

        // Important for 3D effects
        // Note: Y is flipped here! In accordance to OpenGL!
        public Matrix UIViewMatrix { get; private set; }
        public Matrix UIProjectionMatrix { get; private set; }
        public Matrix UIViewProjectionMatrix { get; private set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        //
        // Fields
        //
        public bool interpolatePos = true;
        public bool interpolateAngle = true;
        public bool interpolateZoom = true;

        public float posInterpolationRate = 1.0f;
        public float angleInterpolationRate = 1.0f;
        public float zoomInterpolationRate = 1.0f;

        public static float PixelToWorldRatio => 10.0f;
        public static float ReferenceResolutionX => 1280;
        public static float ReferenceResolutionY => 720;

        protected bool worldDirty = true;
        protected bool uiDirty = true;

        //
        // Members
        //
        protected Vector2 _internalPosition;
        protected Vector2 InternalPosition
        {
            get => interpolatePos ? _internalPosition : LocalPosition;
            set
            {
                worldDirty = true;
                _internalPosition = value;
            }
        }

        public override Vector2 Position 
        { 
            get => base.Position;
            set
            {
                worldDirty = true;
                base.Position = value;
            }
        }


        protected float _internalAngle;
        protected float InternalAngle 
        {
            get => interpolateAngle ? _internalAngle : LocalAngle;
            set
            {
                worldDirty = true;
                _internalAngle = value;
            }
        }

        public override float Angle
        {
            get => base.Angle;
            set
            {
                worldDirty = true;
                base.Angle = value;
            }
        }


        protected float _internalZoom = 1;
        protected float InternalZoom
        {
            get => interpolateZoom ? _internalZoom : Zoom;
            set
            {
                worldDirty = true;
                _internalZoom = value;
            }
        }

        protected float _zoom = 1;
        public float Zoom
        {
            get => _zoom;
            set
            {
                worldDirty = true;
                _zoom = value;
            }
        }

        public Vector2 PixelRatio => Vector2.One * PixelToWorldRatio;

        protected Vector2 LastViewportSize = Vector2.Zero;


        public Camera()
        {
            UIViewMatrix = Matrix.CreateLookAt(Vector3.Forward, Vector3.Backward, Vector3.Down);
            UIProjectionMatrix = Matrix.CreateOrthographic(ReferenceResolutionX, ReferenceResolutionY, 0F, 1F);

            UIViewProjectionMatrix = Matrix.Identity * UIViewMatrix * UIProjectionMatrix;
        }

        public virtual void CameraUpdate(Viewport viewport)
        {
            // Interpolate if necessary
            if (interpolatePos)
                InternalPosition = Vector2.Lerp(InternalPosition, LocalPosition, posInterpolationRate * Timing.deltaTime);

            if (interpolateAngle)
                InternalAngle = TritMath.LerpAngle(InternalAngle, LocalAngle, angleInterpolationRate * Timing.deltaTime);

            if (interpolateZoom)
                InternalZoom = TritMath.Lerp(InternalZoom, Zoom, zoomInterpolationRate * Timing.deltaTime);

            // The difference between our reference resolution and our actual resolution
            // TODO: Allow the user to switch ratios?
            Vector2 view = viewport.GetSize();
            Vector2 scale = view / new Vector2(ReferenceResolutionX, ReferenceResolutionY);

            if (LastViewportSize != view)
            {
                worldDirty = true;
                uiDirty = true;
            }

            LastViewportSize = view;

            if (worldDirty)
            {
                SpriteViewMatrix = Matrix.CreateTranslation(new Vector3(InternalPosition * new Vector2(-1, -1), 0))
                               * Matrix.CreateRotationZ(InternalAngle)
                               * Matrix.CreateScale(new Vector3(PixelRatio * scale * InternalZoom, 1.0f))
                               * Matrix.CreateTranslation(new Vector3(viewport.Width / 2.0f, viewport.Height / 2.0f, 0));

                InverseSpriteViewMatrix = Matrix.Invert(SpriteViewMatrix);

                float hW = viewport.Width / 2.0f;
                float hH = viewport.Height / 2.0f;

                ViewMatrix = Matrix.CreateTranslation(new Vector3(InternalPosition * new Vector2(-1, -1), 0));

                ProjectionMatrix = Matrix.CreateOrthographicOffCenter(-hW, hW, hH, -hH, 0F, 1F)
                    * Matrix.CreateScale(new Vector3(PixelRatio * scale * InternalZoom, 1.0f));

                worldDirty = false;
            }

            if (uiDirty)
            {
                UIScreenMatrix = Matrix.CreateTranslation(new Vector3(ReferenceResolutionX / 2.0f, ReferenceResolutionY / 2.0f, 0))
                    * Matrix.CreateScale(new Vector3(Vector2.One * scale, 1.0f));

                InverseUIScreenMatrix = Matrix.Invert(UIScreenMatrix);

                uiDirty = false;
            }
        }

        // The UI uses a virtual coordinate system to maintain scale between resolutions
        public Vector2 ScreenToUI(Vector2 point)
        {
            return Vector2.Transform(point, UIScreenMatrix);
        }

        public Vector2 UIToScreen(Vector2 point)
        {
            return Vector2.Transform(point, InverseUIScreenMatrix);
        }

        public Vector2 WorldToScreen(Vector2 point)
        {
            return Vector2.Transform(point, SpriteViewMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 point)
        {
            return Vector2.Transform(point, InverseSpriteViewMatrix);
        }
    }
}
