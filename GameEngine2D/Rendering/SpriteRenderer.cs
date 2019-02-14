using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;
using D3D11 = SharpDX.Direct3D11;

namespace GameEngine2D.Rendering
{
    class SpriteRenderer : Component, IComponentDrawable
    {
        public D3D11.Texture2D Texture { get; set; }

        public void Draw()
        {
            if(Texture != null)
            {
                RectangleRendererDX.Instance.Draw(Texture);
            }
        }
    }
}
