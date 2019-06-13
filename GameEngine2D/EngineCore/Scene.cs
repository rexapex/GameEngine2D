using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using GameEngine2D.EntitySystem;
using GameEngine2D.Rendering;
using GameEngine2D.Camera;

namespace GameEngine2D.EngineCore
{
    public class Scene
    {
        // List of child entities
        private List<Entity> entities;

        // Scene is transformed relative to the main camera
        public Camera.Camera MainCamera { get; set; }

        public Scene()
        {
            entities = new List<Entity>();
        }

        // Called when a scene is switched to
        public void OnSceneSwitch()
        {
            foreach (Entity e in entities)
            {
                e.Initialize();
            }
        }

        // Update the scene and all the entities in it
        public void Update()
        {
            foreach (Entity e in entities)
            {
                e.Update();
            }
        }

        // Draw the scene and all the entities in it
        public void Draw(Matrix projMatrix)
        {
            Matrix viewProjMatrix = projMatrix;

            if(MainCamera != null)
            {
                viewProjMatrix = MainCamera.Transform.WorldMatrix * viewProjMatrix;
            }

            foreach (Entity e in entities)
            {
                e.Draw(viewProjMatrix);
            }
        }

        // Add a child entity to the list of entities
        public void AddEntity(Entity e)
        {
            if (e != null)
            {
                entities.Add(e);
            }
        }

        // Get an immutable list of child entities
        public IList<Entity> GetEntities()
        {
            return entities;
        }
    }
}
