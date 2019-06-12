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

        private Tilemap tilemap;
        public Tilemap Tilemap { get => tilemap; set
            {
                tilemap = value;
                if(tilemap != null && tileset != null)
                {
                    InitializeRendererDX();
                }
            }
        }

        private Tileset tileset;
        public Tileset Tileset { get => tileset; set
            {
                tileset = value;
                if(tilemap != null && tileset != null)
                {
                    InitializeRendererDX();
                }
            }
        }

        // Grid renderer specific to this tilemap
        private RectangleGridRendererDX renderer;

        public TilemapRenderer(Entity parent) : base(parent)
        {

        }

        // Initialize the DX renderer used for this component
        private void InitializeRendererDX()
        {
            renderer = new RectangleGridRendererDX(tilemap, tileset);
        }

        // Draw all tiles in the tilemap
        public void Draw(Matrix worldViewProjMatrix)
        {
            if (tilemap == null || tileset == null || tilemap.TileGrid == null || tilemap.TileGrid.Rank != 2)
                return;
            
            renderer.Draw(worldViewProjMatrix, tileset.Texture.TextureObj);

            // Draw each tile individually
            /*for (int x = 0; x < tilemap.Width; x++)
            {
                for(int y = 0; y < tilemap.Height; y++)
                {
                    Vector3 t = new Vector3(x, y, 0);
                    Matrix translation;
                    Matrix.Translation(ref t, out translation);
                    renderer.Draw(translation * scaled, tileset.Texture.TextureObj);
                }
            }*/
        }
    }
}
