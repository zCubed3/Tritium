using System;

using Microsoft.Xna.Framework;

namespace Tritium
{
    // TODO: Dirty flag optimization?
    public class Transform2D
    {
        protected Vector2 _position = Vector2.Zero;
        protected Vector2 _scale = Vector2.One;
        protected float _angle = 0;

        public Vector2 origin = new Vector2(0.5F, 0.5F);
        public float depth = 1F;

        protected bool matrixDirty = true;
        public Vector2 Position
        {
            get => _position;
            set
            {
                matrixDirty = true;
                _position = value;
            }
        }

        public float Angle
        {
            get => _angle;
            set
            {
                matrixDirty = true;
                _angle = TritMath.Wrap(value, -MathF.PI, MathF.PI);
            }
        }

        public Vector2 Scale
        {
            get => _scale;
            set
            {
                matrixDirty = true;
                _scale = value;
            }
        }

        protected Matrix _localToWorld;
        protected Matrix _worldToLocal;

        public Matrix LocalToWorld
        {
            get
            {
                if (matrixDirty)
                    CalculateMatrices();

                return _localToWorld;
            }
        }

        public Matrix WorldToLocal
        {
            get
            {
                if (matrixDirty)
                    CalculateMatrices();

                return _worldToLocal;
            }
        }


        public void CalculateMatrices()
        {
            _localToWorld = Matrix.CreateScale(new Vector3(_scale, 1.0f))
                * Matrix.CreateRotationZ(_angle)
                * Matrix.CreateTranslation(new Vector3(_position, 0));

            _worldToLocal = Matrix.Invert(_localToWorld);

            matrixDirty = false;
        }

        public Vector2 GetRectOrigin(Rectangle relative) => relative.GetSize() * origin;
        public void ApplyMirror(bool mirrorX = true, bool mirrorY = true) => Position *= new Vector2(mirrorX ? -1 : 1, mirrorY ? -1 : 1);

        public Transform2D Transform(Transform2D parent, bool dominance = false)
        {
            if (parent == null)
                return this;

            return new Transform2D()
            {
                Position = Vector2.Transform(Position, parent.LocalToWorld),
                Scale = Scale * parent.Scale,
                Angle = Angle + parent.Angle,
                origin = origin,
                depth = depth,
            };
        }
    }
}
