using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;

namespace GameEngine2D.Rendering
{
    class SpriteRenderer : Component, IComponentDrawable
    {
        public void Draw()
        {
            RectangleRendererDX.Instance.Draw();
        }
    }
}
