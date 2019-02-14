﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;
using GameEngine2D.Rendering;

namespace GameEngine2D.EngineCore
{
    class Scene
    {
        // List of child entities
        private List<Entity> entities;

        public Scene()
        {
            entities = new List<Entity>();

            // TODO - Remove once scene loading implemented
            Entity player = new Entity()
            {
                Name = "Player"
            };
            Component c = new SpriteRenderer();
            player.AddComponent(c);

            entities.Add(player);
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
        public void Draw()
        {
            foreach (Entity e in entities)
            {
                e.Draw();
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