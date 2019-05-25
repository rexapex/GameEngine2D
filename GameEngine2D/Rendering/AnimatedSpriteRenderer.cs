using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;
using SharpDX;

namespace GameEngine2D.Rendering
{
    class AnimatedSpriteRenderer : Component, IComponentUpdatable, IComponentDrawable
    {
        public AnimatedSpriteRenderer(Entity parent) : base(parent) {}

        public void Update()
        {

        }

        public void Draw(Matrix worldViewProjMatrix)
        {

        }
    }
}
