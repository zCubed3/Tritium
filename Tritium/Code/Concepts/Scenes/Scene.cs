using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Concepts.UI;
using Tritium.Input;

namespace Tritium.Concepts.Scenes
{
    public abstract class Scene
    {
        /// <summary>
        /// Top level registered entities to update and render in this scene
        /// </summary>
        protected List<Entity> sceneEntities = new List<Entity>();

        /// <summary>
        /// A readonly copy of the scene entity list, there's no need to modify this outside of a scene
        /// </summary>
        public IReadOnlyList<Entity> SceneEntities => sceneEntities;

        /// <summary>
        /// What color we use when clearing the framebuffer
        /// </summary>
        public virtual Color ClearColor => Color.Black;

        /// <summary>
        /// When not null, the game changes to the set scene next frame!
        /// </summary>
        public virtual Scene NextScene { get; protected set; } = null;

        /// <summary>
        /// Creates data and initializes the scene
        /// </summary>
        public abstract void SetupScene();

        /// <summary>
        /// Adds entities to the list of scene entities
        /// </summary>
        /// <param name="entities">A varying number of entities to add</param>
        public virtual void AddEntities(params Entity[] entities)
        {
            foreach (var entity in entities)
                sceneEntities.Add(entity);
        }

        /// <summary>
        /// Calls Update() on each registered entity
        /// </summary>
        public virtual void UpdateScene()
        {
            foreach (var ent in sceneEntities)
                ent.Update();
        }

        /// <summary>
        /// Calls UpdateUI() on each registered UIEntity
        /// </summary>
        /// <param name="screenToWorldMatrix">The same matrix used to render the UI, provides accurate world to screen transform info</param>
        public virtual void UpdateSceneUI(Matrix? screenToWorldMatrix)
        {
            UIEntity.LastSelectedEntity = UIEntity.SelectedEntity;

            if (!UIEntity.LockSelection)
                UIEntity.SelectedEntity = null;

            foreach (var ent in sceneEntities)
                if (ent is UI.UIEntity uiEnt)
                    uiEnt.UIUpdate(screenToWorldMatrix ?? Matrix.Identity);

            if (UIEntity.LastSelectedEntity != UIEntity.SelectedEntity)
            {
                UIEntity.LastSelectedEntity?.OnUIExit();
                UIEntity.SelectedEntity?.OnUIEnter();
            }

            if (InputSystem.mouseInfo.leftButton.justPressed)
                UIEntity.SelectedEntity?.OnUIClick();

            if (InputSystem.mouseInfo.leftButton.pressed)
                UIEntity.SelectedEntity?.OnUIHold();

            if (InputSystem.mouseInfo.leftButton.justReleased)
                UIEntity.SelectedEntity?.OnUIRelease();
        }

        /// <summary>
        /// Calls Render() on each registered entity
        /// </summary>
        /// <param name="spriteBatch">Spritebatch used for rendering</param>
        public virtual void RenderScene(SpriteBatch spriteBatch)
        {
            foreach (var ent in sceneEntities)
                ent.Render(spriteBatch);
        }
    }
}
