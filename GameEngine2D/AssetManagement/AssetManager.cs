using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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

        // Map from texture file path to texture object
        private Dictionary<string, TextureDX> textures;

        // Map from file path to text file contents
        private Dictionary<string, string> textFileContents;

        public void Initialize(D3D11.Device d3dDevice)
        {
            if(d3dDevice != null && !initialized)
            {
                // Singleton can only be initialized once
                initialized = true;

                // Create the texture loader
                textureLoader = new TextureLoader(d3dDevice);

                // Create the textures map
                textures = new Dictionary<string, TextureDX>();

                // Create the text file contents map
                textFileContents = new Dictionary<string, string>();
            }
        }

        // Add a new texture to the textures list by file path
        // Returns the texture 2D object once loaded
        // Returns null if the file does not exist or the texture could not be created
        public TextureDX AddTexture(string path)
        {
            if(textures.ContainsKey(path))
            {
                // Texture already loaded, therefore, return existing texture object
                return textures[path];
            }
            else
            {
                // Try to load the texture
                int width, height;
                var tex = textureLoader.loadTexture(path, out width, out height);
                if(tex == null)
                {
                    // Texture couldn't not be loaded, therefore, return null
                    return null;
                }
                else
                {
                    // Add the texture to the map then return the texure object
                    var texObj = new TextureDX(tex, width, height);
                    textures[path] = texObj;
                    return texObj;
                }
            }
        }

        // Add a new text file to the text files list by file path
        // Returns the string object once loaded
        // Returns null if the file does not exist
        public string AddTextFile(string path)
        {
            if (textFileContents.ContainsKey(path))
            {
                // Text file already loaded, therefore, return existing string
                return textFileContents[path];
            }
            else
            {
                try
                {
                    var text = File.ReadAllText(path);
                    textFileContents[path] = text;
                    return text;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return null;
        }

        // Get a texture by its file path
        // Returns the texture object if it exists
        // Returns null if the file does not exist or is unloaded
        public TextureDX GetTexture(string path)
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

        // Get contents of text file by its file path
        // Returns the string if it exists
        // Returns null if the file does not exist or is unloaded
        public string GetTextFileContents(string path)
        {
            if (textFileContents.ContainsKey(path))
            {
                // String is available, therefore, return string
                return textFileContents[path];
            }
            else
            {
                // String isn't loaded, therefore, return null
                return null;
            }
        }
    }
}
