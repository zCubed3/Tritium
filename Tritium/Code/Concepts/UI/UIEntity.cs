using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Tritium.Concepts.UI
{
    public class UIEntity : Entity
    {
        /// <summary>
        /// The currently moused over entity
        /// </summary>
        public static UIEntity SelectedEntity;

        /// <summary>
        /// Locks the UI processor to prevent exiting an entity when moving it
        /// </summary>
        public static bool LockSelection = false;

        /// <summary>
        /// The last entity that was moused over
        /// </summary>
        public static UIEntity LastSelectedEntity;

        /// <summary>
        /// Tests whether this entity is closer than the other selected entity
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCloser()
        {
            if (SelectedEntity != null)
                return Depth > SelectedEntity.Depth;
            else
                return true;
        }

        /// <summary>
        /// Called whenever the cursor "enters" this entity
        /// </summary>
        public virtual void OnUIEnter() { }

        /// <summary>
        /// Called whenever the cursor "exits" this entity
        /// </summary>
        public virtual void OnUIExit() { }

        /// <summary>
        /// Called when the cursor clicks while hovering over this entity
        /// </summary>
        public virtual void OnUIClick() { }

        /// <summary>
        /// Called when the cursor unclicks while hovering over this entity
        /// </summary>
        public virtual void OnUIRelease() { }

        /// <summary>
        /// Called when the cursor holds click while hovering over this entity
        /// </summary>
        public virtual void OnUIHold() { }

        /// <summary>
        /// Alternate update function used by the UI to do UI space operations
        /// </summary>
        public virtual void UIUpdate(Matrix? worldToScreenMatrix = null)
        {
            foreach (var child in children)
                if (child is UIEntity uiEnt)
                    uiEnt.UIUpdate(worldToScreenMatrix ?? Matrix.Identity);
        }
    }
}
