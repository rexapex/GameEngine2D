using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.AssetManagement;

namespace GameEngine2D.Tiling
{
    public class Tileset
    {
        // Texture atlas containing all tiles
        public TextureDX Texture { get; set; }

        // RowLength and ColLength refer to the no. of tiles within one row/column
        public int RowLength { get; set; }
        public int ColLength { get; set; }

        // Total number of tiles contained within the texture
        public int NumTiles { get; set; }
    }
}
