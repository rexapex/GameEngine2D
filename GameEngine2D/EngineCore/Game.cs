using System;
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
using GameEngine2D.AssetManagement;
using GameEngine2D.Input;
using GameEngine2D.Gui;

namespace GameEngine2D.EngineCore
{
    public class Game : IDisposable
    {
        private RenderForm renderForm;
        public int Width { get; private set; } = 800;
        public int Height { get; private set; } = 600;

        private D3D11.Device d3dDevice;
        private D3D11.DeviceContext d3dDeviceContext;
        private D3D11.RenderTargetView renderTargetView;
        private SwapChain swapChain;
        private Viewport viewport;

        private D3D11.BlendState blendState;
        private D3D11.BlendStateDescription blendStateDesc;

        private D3D11.DepthStencilView depthStencilView;
        private D3D11.DepthStencilState depthStencilState;
        private D3D11.Texture2DDescription depthTextureDesc;
        private D3D11.DepthStencilStateDescription depthStencilStateDesc;

        private Matrix orthoProjMatrix;

        public Scene Scene { get; private set; }
        private UserInterface gui;

        private bool running = false;

        public Game()
        {
            ProjectManager.Instance.Initialize();

            // Create a window
            renderForm = new RenderForm("2D Game Engine");
            renderForm.ClientSize = new Size(Width, Height);
            renderForm.AllowUserResizing = true;
            renderForm.Resize += OnRenderFormResize;
            
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

            // Initialize the back buffer
            InitializeBackBuffer();

            // Setup the viewport with the new width and height settings
            InitializeViewport();

            // Enable alpha blending
            EnableAlphaBlending();

            // Enable depth test
            EnableDepthTest();

            // Create the orthographic projection matrix
            InitializeProjectionMatrix();

            // Initialize everything which requires reference to the d3d device
            AssetManager.Instance.Initialize(d3dDevice);
            RectangleRendererDX.Instance.Initialize(d3dDevice, d3dDeviceContext);
            RectangleGridRendererDX.Initialize(d3dDevice, d3dDeviceContext);
        }

        public void Start()
        {
            // Can't start if the game is already running or if there is no scene
            if (!running && Scene != null)
            {
                running = true;
                RenderLoop.Run(renderForm, Update);
            }
        }

        private void Update()
        {
            InputManager.Instance.Update();
            gui.Update();
            Scene.Update();
            Draw();
        }

        private void Draw()
        {
            // Clear the screen
            d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(127, 178, 229));
            d3dDeviceContext.ClearDepthStencilView(depthStencilView, D3D11.DepthStencilClearFlags.Depth, 0, 0);

            // Draw the scene
            Scene.Draw(orthoProjMatrix);

            // Draw the gui
            if(gui != null)
            {
                gui.Draw(orthoProjMatrix);
            }

            // Swap the front and back buffers
            swapChain.Present(1, PresentFlags.None);
        }

        // Switch the current scene to the scene specified
        // If the scene is not loaded, will block until loaded
        public void SwitchScene(string sceneName)
        {
            if(sceneName != null)
            {
                if(ProjectManager.Instance.LoadedScenes.ContainsKey(sceneName))
                {
                    // Scene is already loaded so switch to it
                    Scene = ProjectManager.Instance.LoadedScenes[sceneName];
                }
                else if(ProjectManager.Instance.Scenes.ContainsKey(sceneName))
                {
                    // Scene not loaded but does exist, therefore, load scene then switch
                    ProjectManager.Instance.LoadScene(sceneName);
                    Scene = ProjectManager.Instance.LoadedScenes[sceneName];
                }
                Scene.OnSceneSwitch();
            }
        }

        // Switch the current gui to the gui specified
        // If the gui is not loaded, will block until loaded
        public void SwitchGui(string guiName)
        {
            if(guiName != null)
            {
                if(ProjectManager.Instance.LoadedGuis.ContainsKey(guiName))
                {
                    // Gui is already loaded so switch to it
                    gui = ProjectManager.Instance.LoadedGuis[guiName];
                    gui.OnRenderFormResize();
                }
                else if (ProjectManager.Instance.Guis.ContainsKey(guiName))
                {
                    // Gui not loaded but does exist, therefore, load gui then switch
                    ProjectManager.Instance.LoadGui(guiName);
                    gui = ProjectManager.Instance.LoadedGuis[guiName];
                    gui.OnRenderFormResize();
                }
            }
        }

        // Called when render form is resized by the user
        // Based on: https://stackoverflow.com/questions/18658508/sharpdx-windowresize
        private void OnRenderFormResize(object sender, EventArgs args)
        {
            // Unbind everything
            d3dDevice.ImmediateContext.ClearState();

            // Dispose of the old render target view
            if(renderTargetView != null)
            {
                renderTargetView.Dispose();
            }

            // Set the new width and height
            Width = renderForm.ClientSize.Width;
            Height = renderForm.ClientSize.Height;

            // Resize the swap chain
            swapChain.ResizeBuffers(1, Width, Height, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);

            // Initialize the back buffer
            InitializeBackBuffer();

            // Setup the viewport with the new width and height settings
            InitializeViewport();

            // Enable alpha blending
            EnableAlphaBlending();

            // Enable depth test
            EnableDepthTest();

            // Create the orthographic projection matrix
            InitializeProjectionMatrix();

            // Notify the gui if there is one
            if(gui != null)
            {
                gui.OnRenderFormResize();
            }
        }

        private void InitializeBackBuffer()
        {
            // Set the back buffer as the render target view
            using (D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
            {
                if(renderTargetView != null)
                {
                    renderTargetView.Dispose();
                }
                renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
            }
        }

        private void InitializeViewport()
        {
            // Create the viewport
            viewport = new Viewport(0, 0, Width, Height);
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;
            d3dDeviceContext.Rasterizer.SetViewport(viewport);
        }

        private void InitializeProjectionMatrix()
        {
            // Create the orthographic projection matrix
            orthoProjMatrix = Matrix.OrthoOffCenterLH(0, Width, 0, Height, 0.0f, 100.0f);
        }

        private void EnableAlphaBlending()
        {
            if(blendState != null)
            {
                blendState.Dispose();
            }

            // Enable alpha blending
            // Based on: https://stackoverflow.com/questions/24899337/sharpdx-dx11-alpha-blend
            blendStateDesc = new D3D11.BlendStateDescription();
            blendStateDesc.RenderTarget[0].IsBlendEnabled = true;
            blendStateDesc.RenderTarget[0].SourceBlend = D3D11.BlendOption.SourceAlpha;
            blendStateDesc.RenderTarget[0].DestinationBlend = D3D11.BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTarget[0].BlendOperation = D3D11.BlendOperation.Add;
            blendStateDesc.RenderTarget[0].SourceAlphaBlend = D3D11.BlendOption.One;
            blendStateDesc.RenderTarget[0].DestinationAlphaBlend = D3D11.BlendOption.Zero;
            blendStateDesc.RenderTarget[0].AlphaBlendOperation = D3D11.BlendOperation.Add;
            blendStateDesc.RenderTarget[0].RenderTargetWriteMask = D3D11.ColorWriteMaskFlags.All;
            blendState = new D3D11.BlendState(d3dDevice, blendStateDesc);
            d3dDeviceContext.OutputMerger.SetBlendState(blendState);
        }


        // https://gamedev.stackexchange.com/questions/75461/how-do-i-set-up-a-depth-buffer-in-sharpdx
        // https://docs.microsoft.com/en-us/windows/desktop/direct3d11/d3d10-graphics-programming-guide-depth-stencil
        private void EnableDepthTest()
        {
            if(depthStencilView != null)
            {
                depthStencilView.Dispose();
            }

            if(depthStencilState != null)
            {
                depthStencilState.Dispose();
            }

            // Create the depth stencil description
            depthTextureDesc = new D3D11.Texture2DDescription
            {
                Format = Format.D16_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = this.Width,
                Height = this.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default,
                BindFlags = D3D11.BindFlags.DepthStencil,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            // Create the depth stencil view
            using (var depthTex = new D3D11.Texture2D(d3dDevice, depthTextureDesc))
            {
                depthStencilView = new D3D11.DepthStencilView(d3dDevice, depthTex);
            }

            // Create the depth stencil state description
            depthStencilStateDesc = new D3D11.DepthStencilStateDescription();
            depthStencilStateDesc.IsDepthEnabled = true;
            depthStencilStateDesc.DepthWriteMask = D3D11.DepthWriteMask.All;
            depthStencilStateDesc.DepthComparison = D3D11.Comparison.Less;
            depthStencilStateDesc.IsStencilEnabled = false;

            // Create the depth stencil state
            depthStencilState = new D3D11.DepthStencilState(d3dDevice, depthStencilStateDesc);

            // Update the context
            d3dDeviceContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
            d3dDeviceContext.OutputMerger.SetDepthStencilState(depthStencilState);
        }

        public void Dispose()
        {
            renderForm.Dispose();
            swapChain.Dispose();
            d3dDevice.Dispose();
            d3dDeviceContext.Dispose();
            renderTargetView.Dispose();
            blendState.Dispose();
            depthStencilView.Dispose();
            depthStencilState.Dispose();
        }
    }
}
