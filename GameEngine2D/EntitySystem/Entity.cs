using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using GameEngine2D.Math;

namespace GameEngine2D.EntitySystem
{
    class Entity
    {
        // The name of the entity
        // Name is displayed in the editor
        public string Name { get; set; }

        // The parent entity of this entity object
        // Can be null if entity belongs to scene
        public Entity Parent { get; private set; }

        // List of child entities
        private List<Entity> entities;

        // List of components
        private List<Component> components;

        // Every entity has a transform component
        public Transform Transform { get; private set; }

        public Entity(Entity parent)
        {
            Parent = parent;
            entities = new List<Entity>();
            components = new List<Component>();
            Transform = new Transform(this);
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
            // Calculate the world view projection matrix of the entity
            Matrix wvp = Transform.WorldMatrix * viewProjMatrix;

            // Draw all child entities
            foreach (Entity e in entities)
            {
                e.Draw(viewProjMatrix);
            }

            // Call the draw method of each drawable component of the entity
            foreach (IComponentDrawable c in components.OfType<IComponentDrawable>())
            {
                c.Draw(wvp);
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
