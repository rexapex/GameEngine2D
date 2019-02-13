﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using GameEngine2D.Rendering;

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

        private Viewport viewport;

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
            // Create a description of the swap chain
            SwapChainDescription swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(renderForm.ClientSize.Width,
                    renderForm.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = !renderForm.IsFullscreen,
                OutputHandle = renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create a Direct3D device using GPU (hardware) rendering
            D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.None, swapChainDesc, out d3dDevice, out swapChain);
            d3dDeviceContext = d3dDevice.ImmediateContext;

            // Set the back buffer as the render target view
            using (D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
            {
                renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
            }

            // Create the viewport
            viewport = new Viewport(0, 0, Width, Height);
            d3dDeviceContext.Rasterizer.SetViewport(viewport);

            // Initialize the rectangle renderer
            RectangleRendererDX.Instance.Initialize(d3dDevice, d3dDeviceContext);
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
            // Clear the screen
            d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(127, 178, 229));

            // Draw the scene
            // TODO

            // Swap the front and back buffers
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