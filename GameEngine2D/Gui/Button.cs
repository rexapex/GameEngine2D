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

        public Button(Widget parent) : base(parent)
        {
            
        }
        
        public override void Update()
        {

        }

        public override void Draw(Matrix worldProjMatrix)
        {
            // Transform the matrix by the widget's transform
            Matrix wvp = Transform.WorldMatrix * OriginTransform.WorldMatrix * worldProjMatrix;

            // Multiply by scale matrix to scale to texture's dimensions
            // Rectangle renderer renders a 1x1 rectangle by default
            var scaled = Texture.BaseTextureScale * wvp;

            if (Texture != null)
            {
                RectangleRendererDX.Instance.Draw(scaled, Texture.TextureObj);
            }
        }

        public override void OnParentResize()
        {
            UpdateOrigin();
        }

        // TODO - Include scale in the calculation
        private void UpdateOrigin()
        {
            int originX = 0;
            int originY = 0;
            
            // Calculate the offset wrt. the origin
            // TODO - Re-evaluate this code once nested containers are in use (global transform maybe should be used instead)
            switch(Origin)
            {
                case EOrigin.BOTTOM_CENTER:
                    originX = (int)parent.Transform.Position.X + (parent.Width / 2) - (Width / 2);
                    originY = (int)parent.Transform.Position.Y;
                    break;
                case EOrigin.BOTTOM_LEFT:
                    originX = (int)parent.Transform.Position.X;
                    originY = (int)parent.Transform.Position.Y;
                    break;
                case EOrigin.BOTTOM_RIGHT:
                    originX = (int)parent.Transform.Position.X + parent.Width - Width;
                    originY = (int)parent.Transform.Position.Y;
                    break;
                case EOrigin.CENTER:
                    originX = (int)parent.Transform.Position.X + (parent.Width / 2) - (Width / 2);
                    originY = (int)parent.Transform.Position.Y + (parent.Height / 2) - (Height / 2);
                    break;
                case EOrigin.CENTER_LEFT:
                    originX = (int)parent.Transform.Position.X;
                    originY = (int)parent.Transform.Position.Y + (parent.Height / 2) - (Height / 2);
                    break;
                case EOrigin.CENTER_RIGHT:
                    originX = (int)parent.Transform.Position.X + parent.Width - Width;
                    originY = (int)parent.Transform.Position.Y + (parent.Height / 2) - (Height / 2);
                    break;
                case EOrigin.TOP_CENTER:
                    originX = (int)parent.Transform.Position.X + (parent.Width / 2) - (Width / 2);
                    originY = (int)parent.Transform.Position.Y + parent.Height - Height;
                    break;
                case EOrigin.TOP_LEFT:
                    originX = (int)parent.Transform.Position.X;
                    originY = (int)parent.Transform.Position.Y + parent.Height - Height;
                    break;
                case EOrigin.TOP_RIGHT:
                    originX = (int)parent.Transform.Position.X + parent.Width - Width;
                    originY = (int)parent.Transform.Position.Y + parent.Height - Height;
                    break;
            }

            // Update the origin point
            OriginTransform.SetTranslation(originX, originY);
        }
    }
}
