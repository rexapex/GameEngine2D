using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.Math;
using GameEngine2D.EntitySystem;

namespace GameEngine2D.Camera
{
    public abstract class Camera : Component, IComponentUpdatable
    {
        public Transform Transform { get; private set; }

        public Camera(Entity parent) : base(parent)
        {
            Transform = new Transform();
        }

        public abstract void Update();
    }
}
