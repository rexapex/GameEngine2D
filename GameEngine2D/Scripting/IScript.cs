using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;

namespace GameEngine2D.Scripting
{
    // IScript interface to be extended by programmer scripts
    public abstract class IScript
    {
        protected Entity entity;

        public void Initialize(Entity parent)
        {
            if(parent != null)
            {
                entity = parent;
            }
        }

        public abstract void Start();
        public abstract void Update();
    }
}
