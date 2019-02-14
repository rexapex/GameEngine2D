using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GameEngine2D.EngineCore;
using GameEngine2D.EntitySystem;
using GameEngine2D.Rendering;
using GameEngine2D.Math;
using System.Globalization;

namespace GameEngine2D.AssetManagement
{
    class ProjectManager
    {
        // Makes use of the singleton pattern since only one asset manager will ever be needed
        private static readonly ProjectManager instance = new ProjectManager();

        private static bool initialized = false;

        static ProjectManager() { }

        // Private constructor so no other instances can be made
        private ProjectManager() { }

        public static ProjectManager Instance
        {
            get => instance;
        }

        // The path of the current project's root directory
        private string projectPath = null;

        // The info data structure of the project
        public ProjectInfo ProjectInfo { get; private set; }

        // The map from scene name to scene path
        public Dictionary<string, string> Scenes { get; private set; }

        // The map from scene name to scene object
        // Only contains scenes which have been explicitly loaded
        public Dictionary<string, Scene> LoadedScenes { get; private set; }

        public void Initialize()
        {
            if (!initialized)
            {
                // Singleton can only be initialized once
                initialized = true;
            }
        }

        // Load a project
        // Does not load any scene, just loads the scene list, info etc.
        // Sets the scene list, info etc. of the ProjectLoader which can be queried
        public void LoadProject(string projectPath)
        {
            // Set the current project's path
            this.projectPath = projectPath;

            // Create the map from scene name to scene object
            LoadedScenes = new Dictionary<string, Scene>();

            // Load the project info
            LoadProjectInfoFile(projectPath + "/info.xml");

            // Load the scene declerations
            LoadSceneDeclerationsFile(projectPath + "/scene_declerations.xml");
            
            // TODO - Load tags
        }

        // Load a scene
        // The scene name must be a key in the Scenes map
        public void LoadScene(string sceneName)
        {
            if(projectPath != null && Scenes.ContainsKey(sceneName) && !LoadedScenes.ContainsKey(sceneName))
            {
                string path = projectPath + "/" + Scenes[sceneName];
                Scene s = new Scene();

                // Load the xml file
                XElement root = XElement.Load(path);

                // Each child of root should be an entity
                foreach (XElement child in root.Elements())
                {
                    if(child.Name == "entity")
                    {
                        Entity e = new Entity();
                        ParseEntity(child, e);
                        s.AddEntity(e);
                    }
                }

                // Add the scene to the loaded scenes map
                LoadedScenes[sceneName] = s;
            }
        }

        // Parse the xml node of an entity
        private void ParseEntity(XElement entityNode, Entity e)
        {
            // Extract the name of the entity from the xml
            var nameAttrib = entityNode.Attribute("name");
            if(nameAttrib != null)
            {
                // Set the name of the entity
                e.Name = nameAttrib.Value.ToString();

                // Loop over each child entity and component tag
                foreach(XElement child in entityNode.Elements())
                {
                    switch(child.Name.ToString())
                    {
                        case "entity":
                            Entity e2 = new Entity();
                            ParseEntity(child, e2);
                            e.AddEntity(e2);
                            break;
                        case "transform":
                            Transform t = new Transform();
                            ParseTransform(child, t);
                            // TODO - Set the transform of e to t
                            break;
                        case "sprite-renderer":
                            SpriteRenderer c = new SpriteRenderer();
                            ParseSpriteRenderer(child, c);
                            e.AddComponent(c);
                            break;
                    }
                }
            }
        }

        // Parse the xml node of a transform component
        private void ParseTransform(XElement transformNode, Transform t)
        {
            var xNode = transformNode.Element("x");
            var yNode = transformNode.Element("y");
            var rNode = transformNode.Element("r");
            var sxNode = transformNode.Element("sx");
            var syNode = transformNode.Element("sy");

            // All nodes are required for a valid transform
            if(xNode != null && yNode != null && rNode != null && sxNode != null && syNode != null)
            {
                try
                {
                    // Convert the strings into floats
                    float x = float.Parse(xNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(yNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float r = float.Parse(rNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float sx = float.Parse(sxNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float sy = float.Parse(syNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

                    // Set the data of the transform
                    t.SetTranslation(x, y);
                    t.SetRotation(r);
                    t.SetScale(sx, sy);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Source + ", " + e.Data);
                }
            }
        }

        // Parse the xml node of a sprite renderer component
        private void ParseSpriteRenderer(XElement componentNode, SpriteRenderer c)
        {
            var textureNode = componentNode.Element("texture");
            if(textureNode != null)
            {
                // Add the texture to the asset manager
                c.Texture = AssetManager.Instance.AddTexture(projectPath + "/" + textureNode.Value.ToString());
            }
        }

        // Load the scene_declerations.xml file
        // File should be at the project root
        private void LoadSceneDeclerationsFile(string path)
        {
            // Create the map from scene name to scene path
            Scenes = new Dictionary<string, string>();

            // Load the xml file
            XElement root = XElement.Load(path);

            // Each child of root is a scene decleration
            foreach(XElement child in root.Elements())
            {
                // Ensure the tag is a scene tag and contains the required data
                if(child.Name == "scene" && child.Attribute("name") != null && child.Attribute("path") != null)
                {
                    string sceneName = child.Attribute("name").Value.ToString();
                    string scenePath = child.Attribute("path").Value.ToString();
                    if(sceneName != null && scenePath != null)
                    {
                        // Add the scene to the Scenes map if the name is not already taken
                        if(!Scenes.ContainsKey(sceneName))
                        {
                            Scenes[sceneName] = scenePath;
                        }
                    }
                }
            }
        }

        // Load the info.xml file
        // File should be at the project root
        private void LoadProjectInfoFile(string path)
        {
            // Data to extract from file
            string name = "Unknown";
            string engineVersion = "Unknown";
            string gameVersion = "Unknown";
            string dateCreated = "Unknown";
            string dateModified = "Unknown";

            // Load the xml file
            XElement root = XElement.Load(path);

            // In info file, all elements are direct children of root
            foreach(XElement child in root.Elements())
            {
                switch(child.Name.ToString())
                {
                    case "name":
                        name = child.Value;
                        break;
                    case "engine-version":
                        engineVersion = child.Value;
                        break;
                    case "game-version":
                        gameVersion = child.Value;
                        break;
                    case "dateCreated":
                        dateCreated = child.Value;
                        break;
                    case "dateModified":
                        dateModified = child.Value;
                        break;
                }
            }

            // Set the project info property
            ProjectInfo = new ProjectInfo(name, engineVersion, gameVersion, dateCreated, dateModified);
        }
    }
}
