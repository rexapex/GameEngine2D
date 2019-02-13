using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
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
                Vector2[] vertices = new Vector2[]
                {
                    new Vector2(-0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.0f, -0.5f)
                };
                numVertices = vertices.Count();

                // Create a vertex buffer
                vertexBuffer = D3D11.Buffer.Create<Vector2>(d3dDevice, D3D11.BindFlags.VertexBuffer, vertices);
                
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
                    new D3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, 0)
                };

                // Create the input layout matching the input elements to shader input signature
                inputLayout = new D3D11.InputLayout(d3dDevice, inputSignature, inputElements);
            }
        }

        public void Draw()
        {
            // Set the shaders to use for the draw operation
            d3dDeviceContext.VertexShader.Set(vertexShader);
            d3dDeviceContext.PixelShader.Set(pixelShader);

            // Set the primitive topology to triangle list
            d3dDeviceContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            // Set the input layout of the vertices
            d3dDeviceContext.InputAssembler.InputLayout = inputLayout;

            // Draw the rectangle
            d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector2>(), 0));
            d3dDeviceContext.Draw(numVertices, 0);
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
            inputLayout.Dispose();
            inputSignature.Dispose();
        }
    }
}
