using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;

namespace GameEngine2D.EngineCore
{
    class Game : IDisposable
    {
        private RenderForm renderForm;
        private const int Width = 800;
        private const int Height = 600;

        private D3D11.Device d3dDevice;
        private D3D11.DeviceContext d3dDeviceContext;
        private D3D11.RenderTargetView renderTargetView;
        private SwapChain swapChain;

        private bool running = false;

        public Game()
        {
            // Create a window
            renderForm = new RenderForm("2D Game Engine");
            renderForm.ClientSize = new Size(Width, Height);
            renderForm.AllowUserResizing = false;

            // Initialize Direct3D
            InitializeDeviceResources();
        }

        private void InitializeDeviceResources()
        {
            // Create a description of the back buffer
            ModeDescription backBufferDesc = new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);

            // Create a description of the swap chain
            SwapChainDescription swapChainDesc = new SwapChainDescription()
            {
                ModeDescription = backBufferDesc,
                SampleDescription = new SampleDescription(0, 1),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = renderForm.Handle,
                IsWindowed = true
            };

            // Create a Direct3D device using GPU (hardware) rendering
            D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.None, swapChainDesc, out d3dDevice, out swapChain);
            d3dDeviceContext = d3dDevice.ImmediateContext;

            // Set the back buffer as the render target view
            using (D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
            {
                renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
            }
        }

        public void Start()
        {
            if (!running)
            {
                running = true;
                RenderLoop.Run(renderForm, Update);
            }
        }

        private void Update()
        {
            Draw();
        }

        private void Draw()
        {
            d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(127, 127, 127));
            swapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            renderForm.Dispose();
            swapChain.Dispose();
            d3dDevice.Dispose();
            d3dDeviceContext.Dispose();
            renderTargetView.Dispose();
        }
    }
}
