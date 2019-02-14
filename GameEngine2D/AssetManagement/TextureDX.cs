using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpDX;

namespace GameEngine2D.AssetManagement
{
    class TextureDX
    {
        public Texture2D TextureObj { get; private set; }
        public Matrix BaseTextureScale { get => baseTextureScale; }
        private Matrix baseTextureScale;

        public TextureDX(Texture2D texObj, int texWidth, int texHeight)
        {
            TextureObj = texObj;
            Vector3 scale = new Vector3(texWidth, texHeight, 1);
            Matrix.Scaling(ref scale, out baseTextureScale);
        }
    }
}
