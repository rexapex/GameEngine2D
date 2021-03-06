﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;
using GameEngine2D.AssetManagement;
using SharpDX;
using D3D11 = SharpDX.Direct3D11;

namespace GameEngine2D.Rendering
{
    class SpriteRenderer : Component, IComponentDrawable
    {
        public TextureDX Texture { get; set; }

        public SpriteRenderer(Entity parent) : base(parent) {}

        public void Draw(Matrix worldViewProjMatrix)
        {
            // Multiply by scale matrix to scale to texture's dimensions
            // Rectangle renderer renders a 1x1 rectangle by default
            var scaled = Texture.BaseTextureScale * worldViewProjMatrix;

            if(Texture != null)
            {
                RectangleRendererDX.Instance.Draw(scaled, Texture.TextureObj);
            }
        }
    }
}
