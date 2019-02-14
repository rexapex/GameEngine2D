using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;

namespace GameEngine2D.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexXYUV
    {
        public readonly Vector2 Position;
        public readonly Vector2 TextureUV;

        public VertexXYUV(Vector2 position, Vector2 textureUV)
        {
            Position = position;
            TextureUV = textureUV;
        }
    }
}
