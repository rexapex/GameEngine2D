using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.D3DCompiler;
using D3D11 = SharpDX.Direct3D11;

namespace GameEngine2D.Rendering
{
    sealed class RectangleRendererDX : IDisposable
    {
        // Makes use of the singleton pattern since only one rectangle renderer will ever be needed
        private static readonly RectangleRendererDX instance = new RectangleRendererDX();

        // True once the instance has been initialized
        private bool initialized = false;

        static RectangleRendererDX() {}

        // Private constructor so no other instances can be made
        private RectangleRendererDX() {}

        public static RectangleRendererDX Instance
        {
            get => instance;
        }

        private D3D11.Device d3dDevice;
        private D3D11.DeviceContext d3dDeviceContext;

        private D3D11.Buffer vertexBuffer;
        private D3D11.InputElement[] inputElements;
        private int numVertices;

        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;
        private ShaderSignature inputSignature;
        private D3D11.InputLayout inputLayout;
        private D3D11.SamplerState samplerState;

        private D3D11.Buffer worldViewProjBuffer;

        public void Initialize(D3D11.Device d3dDevice, D3D11.DeviceContext d3dDeviceContext)
        {
            if(d3dDevice != null && d3dDeviceContext != null && !instance.initialized)
            {
                // Singleton can only be initialized once
                instance.initialized = true;

                // Set the device and device context fields
                this.d3dDevice = d3dDevice;
                this.d3dDeviceContext = d3dDeviceContext;

                // Vertices of the rectangle
                // Rectangle drawn as two triangles
                VertexXYUV[] vertices = new VertexXYUV[]
                {
                    new VertexXYUV(new Vector2(0, 1), new Vector2(0, 0)),
                    new VertexXYUV(new Vector2(1, 1), new Vector2(1, 0)),
                    new VertexXYUV(new Vector2(1, 0), new Vector2(1, 1)),
                    new VertexXYUV(new Vector2(1, 0), new Vector2(1, 1)),
                    new VertexXYUV(new Vector2(0, 0), new Vector2(0, 1)),
                    new VertexXYUV(new Vector2(0, 1), new Vector2(0, 0))
                };
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
                        Filter = D3D11.Filter.MinMagMipLinear
                    });

                // Create the world view projection buffer
                worldViewProjBuffer = new SharpDX.Direct3D11.Buffer(d3dDevice, Utilities.SizeOf<Matrix>(), D3D11.ResourceUsage.Default,
                    D3D11.BindFlags.ConstantBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
            }
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
