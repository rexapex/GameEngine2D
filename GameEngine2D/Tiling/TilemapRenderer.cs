using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using GameEngine2D.EntitySystem;
using GameEngine2D.Rendering;

namespace GameEngine2D.Tiling
{
    public class TilemapRenderer : Component, IComponentDrawable
    {
        // TODO - Enum with type of render: square, hex, oblong, etc.
        public Tilemap Tilemap { get; set; }
        public Tileset Tileset { get; set; }

        public TilemapRenderer(Entity parent) : base(parent)
        {

        }

        // Draw all tiles in the tilemap
        public void Draw(Matrix worldViewProjMatrix)
        {
            if (Tilemap == null || Tileset == null || Tilemap.TileGrid == null || Tilemap.TileGrid.Rank != 2)
                return;

            // Multiply by scale matrix to scale to texture's dimensions
            // Rectangle renderer renders a 1x1 rectangle by default
            var scaled = Tileset.Texture.BaseTextureScale * worldViewProjMatrix;

            // Draw each tile individually
            for (int x = 0; x < Tilemap.TileGrid.GetLength(0); x++)
            {
                for(int y = 0; y < Tilemap.TileGrid.GetLength(1); y++)
                {
                    Vector3 t = new Vector3(x, y, 0);
                    Matrix translation;
                    Matrix.Translation(ref t, out translation);
                    RectangleRendererDX.Instance.Draw(translation * scaled, Tileset.Texture.TextureObj);
                }
            }
        }
    }
}
