using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameEngine2D.EntitySystem
{
    class Entity
    {
        // The name of the entity
        // Name is displayed in the editor
        public string Name { get; set; }

        // List of child entities
        private List<Entity> entities;

        // List of components
        private List<Component> components;

        public Entity()
        {
            entities = new List<Entity>();
            components = new List<Component>();
        }

        // Update the entity, its child entities and its components
        public void Update()
        {
            foreach(Entity e in entities)
            {
                e.Update();
            }

            foreach(IComponentUpdatable c in components.OfType<IComponentUpdatable>())
            {
                c.Update();
            }
        }

        // Draw the entity, its child entities and its components
        public void Draw(Matrix viewProjMatrix)
        {
            // TODO - Multiply view proj matrix by model matrix

            foreach (Entity e in entities)
            {
                e.Draw(viewProjMatrix);
            }

            foreach (IComponentDrawable c in components.OfType<IComponentDrawable>())
            {
                c.Draw(viewProjMatrix);
            }
        }

        // Add a child entity to the list of entities
        public void AddEntity(Entity e)
        {
            if(e != null)
            {
                entities.Add(e);
            }
        }

        // Get an immutable list of child entities
        public IList<Entity> GetEntities()
        {
            return entities;
        }

        // Add a premade component to the entity
        public void AddComponent(Component c)
        {
            if(c != null)
            {
                components.Add(c);
            }
        }

        // Get the first component of specified type
        // Returns null if no component of type exists
        public T GetComponent<T>()
        {
            return components.OfType<T>().FirstOrDefault();
        }

        // Get an immutable enumerable of all components of the specified type
        public IEnumerable<T> GetComponents<T>()
        {
            return components.OfType<T>();
        }

        // Get an immutable list of all components
        public IList<Component> GetComponents()
        {
            return components.AsReadOnly();
        }
    }
}
