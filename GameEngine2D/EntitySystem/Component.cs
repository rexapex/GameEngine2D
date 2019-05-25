using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine2D.EntitySystem
{
    public class Component
    {
        // The name of the component
        // Name is displayed in the editor
        public string Name { get; set; }

        // The parent entity of the component
        protected Entity parent;

        public Component(Entity parent)
        {
            this.parent = parent;
        }
    }
}
