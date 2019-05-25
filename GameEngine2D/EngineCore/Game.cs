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

        private Matrix orthoProjMatrix;

        private Scene scene;

        private bool running = false;

        public Game()
        {
            ProjectManager.Instance.Initialize();

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

            // Create the orthographic projection matrix
            orthoProjMatrix = Matrix.OrthoOffCenterLH(0, Width, 0, Height, 0.0f, 100.0f);

            // Initialize everything which requires reference to the d3d device
            AssetManager.Instance.Initialize(d3dDevice);
            RectangleRendererDX.Instance.Initialize(d3dDevice, d3dDeviceContext);
        }

        public void Start()
        {
            // Can't start if the game is already running or if there is no scene
            if (!running && scene != null)
            {
                running = true;
                RenderLoop.Run(renderForm, Update);
            }
        }

        private void Update()
        {
            InputManager.Instance.Update();
            scene.Update();
            Draw();
        }

        private void Draw()
        {
            // Clear the screen
            d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(127, 178, 229));

            // Draw the scene
            scene.Draw(orthoProjMatrix);

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
                    scene = ProjectManager.Instance.LoadedScenes[sceneName];
                }
                else if(ProjectManager.Instance.Scenes.ContainsKey(sceneName))
                {
                    // Scene not loaded but does exist, therefore, load scene then switch
                    ProjectManager.Instance.LoadScene(sceneName);
                    scene = ProjectManager.Instance.LoadedScenes[sceneName];
                }
                scene.OnSceneSwitch();
            }
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
