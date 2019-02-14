using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
using D3D11 = SharpDX.Direct3D11;

namespace GameEngine2D.AssetManagement
{
    class TextureLoader
    {
        private D3D11.Device d3dDevice;

        public TextureLoader(D3D11.Device d3dDevice)
        {
            this.d3dDevice = d3dDevice;
        }

        // Load a texture from a file
        // https://stackoverflow.com/questions/36068631/sharpdx-3-0-2-d3d11-how-to-load-texture-from-file-and-make-it-to-work-in-shade
        public D3D11.Texture2D loadTexture(string path)
        {
            // Load the texture from the file path
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(path);
            }
            catch(System.IO.FileNotFoundException e)
            {
                Console.WriteLine(e.Source, e.Data);
                return null;
            }

            // Ensure the bitmap is in the correct format
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
            }

            // Get the image data from the bitmap object
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Create a Direct3D texture of the image data
            var tex = new D3D11.Texture2D(d3dDevice, new D3D11.Texture2DDescription()
            {
                Width = bitmap.Width,
                Height = bitmap.Height,
                ArraySize = 1,
                BindFlags = D3D11.BindFlags.ShaderResource,
                Usage = D3D11.ResourceUsage.Immutable,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0)
            }, new SharpDX.DataRectangle(data.Scan0, data.Stride));

            // Return bitmap data
            bitmap.UnlockBits(data);
            return tex;
        }
    }
}
