using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameEngine2D.AssetManagement
{
    class ProjectLoader
    {
        // Makes use of the singleton pattern since only one asset manager will ever be needed
        private static readonly ProjectLoader instance = new ProjectLoader();

        private static bool initialized = false;

        static ProjectLoader() { }

        // Private constructor so no other instances can be made
        private ProjectLoader() { }

        public static ProjectLoader Instance
        {
            get => instance;
        }

        // The info data structure of the project
        public ProjectInfo ProjectInfo { get; private set; }

        // The map from scene name to scene path
        public Dictionary<string, string> Scenes { get; private set; }

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
            // Load the project info
            LoadProjectInfoFile(projectPath + "/info.xml");

            // Load the scene declerations
            LoadSceneDeclerationsFile(projectPath + "/scene_declerations.xml");
            
            // TODO - Load tags
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
