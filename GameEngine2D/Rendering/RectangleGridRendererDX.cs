using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.D3DCompiler;
using D3D11 = SharpDX.Direct3D11;
using GameEngine2D.Tiling;

namespace GameEngine2D.Rendering
{
    sealed class RectangleGridRendererDX : IDisposable
    {
        private static D3D11.Device d3dDevice;
        private static D3D11.DeviceContext d3dDeviceContext;

        private D3D11.Buffer vertexBuffer;
        private D3D11.InputElement[] inputElements;
        private int numVertices;

        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;
        private ShaderSignature inputSignature;
        private D3D11.InputLayout inputLayout;
        private D3D11.SamplerState samplerState;

        private D3D11.Buffer worldViewProjBuffer;

        public static void Initialize(D3D11.Device d3dDevice, D3D11.DeviceContext d3dDeviceContext)
        {
            RectangleGridRendererDX.d3dDevice = d3dDevice;
            RectangleGridRendererDX.d3dDeviceContext = d3dDeviceContext;
        }

        public RectangleGridRendererDX(Tilemap tilemap, Tileset tileset)
        {
            int gridWidth = tilemap.Width;
            int gridHeight = tilemap.Height;
            // Width and height of each tile in the tileset
            int tileWidth = tileset.Texture.Width / tileset.RowLength;
            int tileHeight = tileset.Texture.Height / tileset.ColLength;
            // Calculate the distance from the edge of a pixel to the centre of the pixel as a ratio of the dimensions of the texture
            float halfPixelWidth = (1 / tileset.Texture.Width) / 2;
            float halfPixelHeight = (1 / tileset.Texture.Height) / 2;
            // Vertices of the rectangle
            // Rectangle drawn as two triangles
            VertexXYUV[] vertices = new VertexXYUV[6 * gridWidth * gridHeight];
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    int value = tilemap.TileGrid[i, j];
                    // Check the tile texture is valid for the tileset given
                    if (value >= 0 && value < tileset.NumTiles)
                    {
                        // Calculate the position of the tile in the texture atlas
                        int atlasX = value % tileset.RowLength;
                        int atlasY = value / tileset.ColLength;
                        // Calculate the texture co-ordinates for the tile
                        float u0 = (float)atlasX / tileset.RowLength;
                        float u1 = (float)(atlasX + 1) / tileset.RowLength;
                        float v0 = (float)atlasY / tileset.ColLength;
                        float v1 = (float)(atlasY + 1) / tileset.ColLength;
                        // Calculate the position co-ordinates for the tile
                        float x0 = i * tileWidth + halfPixelWidth;
                        float x1 = (i + 1) * tileWidth - halfPixelWidth;
                        float y0 = j * tileHeight + halfPixelHeight;
                        float y1 = (j + 1) * tileHeight - halfPixelHeight;
                        // Set the vertex's position and texture co-ordinates
                        vertices[6*(i + gridWidth * j) + 0] = new VertexXYUV(new Vector2(x0, y1), new Vector2(u0, v0));
                        vertices[6*(i + gridWidth * j) + 1] = new VertexXYUV(new Vector2(x1, y1), new Vector2(u1, v0));
                        vertices[6*(i + gridWidth * j) + 2] = new VertexXYUV(new Vector2(x1, y0), new Vector2(u1, v1));
                        vertices[6*(i + gridWidth * j) + 3] = new VertexXYUV(new Vector2(x1, y0), new Vector2(u1, v1));
                        vertices[6*(i + gridWidth * j) + 4] = new VertexXYUV(new Vector2(x0, y0), new Vector2(u0, v1));
                        vertices[6*(i + gridWidth * j) + 5] = new VertexXYUV(new Vector2(x0, y1), new Vector2(u0, v0));
                    }
                }
            }
            numVertices = vertices.Count();
            
            // Create a vertex buffer
            vertexBuffer = D3D11.Buffer.Create(d3dDevice, D3D11.BindFlags.VertexBuffer, vertices);

            // Create the vertex and pixel shaders
            using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile("EngineAssets/VertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug))
            {
                inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
                vertexShader = new D3D11.VertexShader(d3dDevice, vertexShaderByteCode);
            }
            using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile("EngineAssets/PixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug))
            {
                pixelShader = new D3D11.PixelShader(d3dDevice, pixelShaderByteCode);
            }

            // Array of input elements tell D3D how data is stored in vertex buffer
            inputElements = new D3D11.InputElement[]
            {
                // R32G32_Float tells d3d that a position is a Vector2 of floats
                new D3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, 0, 0, D3D11.InputClassification.PerVertexData, 0),
                new D3D11.InputElement("TEXTUREUV", 0, SharpDX.DXGI.Format.R32G32_Float, 8, 0, D3D11.InputClassification.PerVertexData, 0)
            };

            // Create the input layout matching the input elements to shader input signature
            inputLayout = new D3D11.InputLayout(d3dDevice, inputSignature, inputElements);

            // Create the sampler used to sample textures in the shaders
            samplerState = new D3D11.SamplerState(d3dDevice,
                new D3D11.SamplerStateDescription
                {
                    AddressU = D3D11.TextureAddressMode.Wrap,
                    AddressV = D3D11.TextureAddressMode.Wrap,
                    AddressW = D3D11.TextureAddressMode.Wrap,
                    Filter = D3D11.Filter.MinMagPointMipLinear
                });

            // Create the world view projection buffer
            worldViewProjBuffer = new SharpDX.Direct3D11.Buffer(d3dDevice, Utilities.SizeOf<Matrix>(), D3D11.ResourceUsage.Default,
                D3D11.BindFlags.ConstantBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
        }

        public void Draw(Matrix worldViewProjMatrix, D3D11.Texture2D texture)
        {
            // Set the shaders to use for the draw operation
            d3dDeviceContext.VertexShader.Set(vertexShader);
            d3dDeviceContext.PixelShader.Set(pixelShader);

            // Set the primitive topology to triangle list
            d3dDeviceContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            // Set the input layout of the vertices
            d3dDeviceContext.InputAssembler.InputLayout = inputLayout;

            // Bind the texture resource to the pixel shader
            var textureView = new D3D11.ShaderResourceView(d3dDevice, texture);
            d3dDeviceContext.PixelShader.SetShaderResource(0, textureView);
            d3dDeviceContext.PixelShader.SetSampler(0, samplerState);
            textureView.Dispose();

            // Pass the world view projection matrix to the vertex shader
            d3dDeviceContext.VertexShader.SetConstantBuffer(0, worldViewProjBuffer);
            d3dDeviceContext.UpdateSubresource(ref worldViewProjMatrix, worldViewProjBuffer);

            // Draw the rectangle
            d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<VertexXYUV>(), 0));
            d3dDeviceContext.Draw(numVertices, 0);
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
            inputLayout.Dispose();
            inputSignature.Dispose();
            worldViewProjBuffer.Dispose();
            samplerState.Dispose();
        }
    }
}
