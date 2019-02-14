﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;

namespace GameEngine2D.AssetManagement
{
    class AssetManager
    {
        // Makes use of the singleton pattern since only one asset manager will ever be needed
        private static readonly AssetManager instance = new AssetManager();

        private static bool initialized = false;
        
        static AssetManager() { }

        // Private constructor so no other instances can be made
        private AssetManager() { }

        public static AssetManager Instance
        {
            get => instance;
        }

        // Used to load and create D3D textures
        private TextureLoader textureLoader;

        // Map from texture file path to D3D texture object
        private Dictionary<string, D3D11.Texture2D> textures;

        public void Initialize(D3D11.Device d3dDevice)
        {
            if(d3dDevice != null && !initialized)
            {
                // Singleton can only be initialized once
                initialized = true;

                // Create the texture loader
                textureLoader = new TextureLoader(d3dDevice);

                // Create the textures map
                textures = new Dictionary<string, D3D11.Texture2D>();
            }
        }

        // Add a new texture to the textures list by file path
        // Returns the D3D texture 2D object once loaded
        // Returns null if the file does not exist or the texture could not be created
        public D3D11.Texture2D AddTexture(string path)
        {
            if(textures.ContainsKey(path))
            {
                // Texture already loaded, therefore, return existing texture object
                return textures[path];
            }
            else
            {
                // Try to load the texture
                var tex = textureLoader.loadTexture(path);
                if(tex == null)
                {
                    // Texture couldn't not be loaded, therefore, return null
                    return null;
                }
                else
                {
                    // Add the texture to the map then return the texure object
                    textures[path] = tex;
                    return tex;
                }
            }
        }

        // Get a texture by its file path
        // Returns the D3D texture object if it exists
        // Returns null if the file does not exist or is unloaded
        public D3D11.Texture2D GetTexture(string path)
        {
            if(textures.ContainsKey(path))
            {
                // Texture is available, therefore, return texture object
                return textures[path];
            }
            else
            {
                // Texture isn't loaded, therefore, return null
                return null;
            }
        }
    }
}
