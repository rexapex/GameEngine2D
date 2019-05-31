using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using GameEngine2D.AssetManagement;
using GameEngine2D.Rendering;

namespace GameEngine2D.Gui
{
    public class Button : Widget
    {
        public TextureDX Texture { get; set; }
        
        public override void Update()
        {
            Console.WriteLine("Button.Update - TODO");
        }

        public override void Draw(Matrix worldProjMatrix)
        {
            // Multiply by scale matrix to scale to texture's dimensions
            // Rectangle renderer renders a 1x1 rectangle by default
            var scaled = Texture.BaseTextureScale * worldProjMatrix;

            if (Texture != null)
            {
                RectangleRendererDX.Instance.Draw(scaled, Texture.TextureObj);
            }
        }
    }
}
