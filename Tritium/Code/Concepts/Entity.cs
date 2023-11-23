using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tritium.Concepts
{
    /// <summary>
    /// An tangible "thing" in the game world
    /// </summary>
    public class Entity
    {
        //
        // Fields
        //

        // TODO: Allow parenting in xml?
        public Entity parent;
        public List<Entity> children = new List<Entity>();

        public Transform2D transform = new Transform2D();
        public Transform2D localTransform = new Transform2D();

        public bool enabled = true;
        public bool visible = true;

        //
        // Properties
        //
        public virtual Vector2 Position
        {
            get => transform.Position;
            set => transform.Position = value;
        }

        public virtual Vector2 LocalPosition
        {
            get => localTransform.Position;
        }

        public virtual float Angle
        {
            get => transform.Angle;
            set => transform.Angle = value;
        }

        public virtual float LocalAngle
        {
            get => localTransform.Angle;
        }

        public virtual float Depth
        {
            get => transform.depth;
            set => transform.depth = value;
        }

        // TODO: Local depth?
        public virtual float LocalDepth
        {
            get => localTransform.depth;
        }

        public virtual Vector2 Scale
        {
            get => transform.Scale;
            set => transform.Scale = value;
        }

        public virtual Vector2 LocalScale
        {
            get => localTransform.Scale;
        }

        //
        // Methods
        //
        public virtual void UpdateTransform()
        {
            if (parent != null)
                localTransform = transform.Transform(parent.localTransform);
            else
                localTransform = transform;
        }
        public virtual void Update() 
        {
            UpdateTransform();

            foreach (var child in children)
                child.Update();
        }

        public virtual void Render(SpriteBatch batch, Entity parent = null)
        {
            // Batch must've already started before rendering!
            foreach (var child in children)
                child.Render(batch, this);
        }

        // TODO: Collisions? I kinda want to use a physics engine for this!
        public virtual bool Collide(Vector2 position, Vector2 direction)
        {
            return false;
        }

        /// <summary>
        /// Adds a list of children
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Parent(params Entity[] entities)
        {
            foreach (var entity in entities)
            {
                if (entity.parent != null)
                {
                    entity.parent.Unparent(entity);

                    // Like with unparenting, we need to honor existing transforms
                    entity.Position = Vector2.Transform(entity.Position, localTransform.WorldToLocal);
                    entity.Angle -= LocalAngle;
                    entity.Scale /= LocalScale;
                }

                entity.parent = this;
                children.Add(entity);
            }
        }

        public virtual void Unparent(params Entity[] entities)
        {
            foreach (var entity in entities)
            {
                // We need to preserve transforms despite removing them from "this"
                entity.Position = Vector2.Transform(entity.Position, localTransform.LocalToWorld);
                entity.Angle += LocalAngle;
                entity.Scale *= LocalScale;

                children.Remove(entity);
            }
        }
    }
}
