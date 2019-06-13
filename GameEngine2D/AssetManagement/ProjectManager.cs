using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using System.Reflection;
using GameEngine2D.EngineCore;
using GameEngine2D.EntitySystem;
using GameEngine2D.Rendering;
using GameEngine2D.Math;
using GameEngine2D.Input;
using GameEngine2D.Scripting;
using GameEngine2D.Tiling;
using GameEngine2D.Gui;

namespace GameEngine2D.AssetManagement
{
    class ProjectManager
    {
        // Makes use of the singleton pattern since only one project manager will ever be needed
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

        // The name of the default scene, i.e. the first scene to load on game start
        public string DefaultSceneName { get; private set; }

        // The map from gui name to gui path
        public Dictionary<string, string> Guis { get; private set; }

        // The map from gui name to gui object
        // Only contains guis which have been loaded
        public Dictionary<string, UserInterface> LoadedGuis { get; private set; }

        // The DLL storing the game scripts
        private Assembly scriptsDll;

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

            // Create the map from gui name to gui object
            LoadedGuis = new Dictionary<string, UserInterface>();

            // Load the project info
            LoadProjectInfoFile(projectPath + "/info.xml");

            // Load the scene declerations
            LoadSceneDeclerationsFile(projectPath + "/scenes.xml");

            // Load the gui declerations
            LoadGuiDeclerationsFile(projectPath + "/guis.xml");

            // TODO - Load tags

            // Load the default input setup
            LoadInputFile(projectPath + "/input.xml");

            // Load the scripts dll
            LoadScriptsDLL(projectPath + "/Scripts.dll");
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
                        Entity e = new Entity(null);
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
                            Entity e2 = new Entity(e);
                            ParseEntity(child, e2);
                            e.AddEntity(e2);
                            break;
                        case "transform":
                            ParseTransform(child, e.Transform);
                            break;
                        case "sprite-renderer":
                            SpriteRenderer c = new SpriteRenderer(e);
                            ParseSpriteRenderer(child, c);
                            e.AddComponent(c);
                            break;
                        case "script":
                            Script s = new Script(e);
                            ParseScript(child, s);
                            e.AddComponent(s);
                            break;
                        case "tilemap-renderer":
                            TilemapRenderer t = new TilemapRenderer(e);
                            ParseTilemapRenderer(child, t);
                            e.AddComponent(t);
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

            // Parse the position if at least one of x and y exist
            if(xNode != null || yNode != null)
            {
                try
                {
                    float x = xNode != null ? float.Parse(xNode.Value, CultureInfo.InvariantCulture.NumberFormat) : 0;
                    float y = yNode != null ? float.Parse(yNode.Value, CultureInfo.InvariantCulture.NumberFormat) : 0;
                    t.SetTranslation(x, y);
                }
                catch(FormatException e)
                {
                    Console.WriteLine(e.Message + " when parsing x and/or y nodes in transform");
                }
            }

            // Parse the rotation
            if(rNode != null)
            {
                try
                {
                    float r = float.Parse(rNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    t.SetRotation(r);
                }
                catch(FormatException e)
                {
                    Console.WriteLine(e.Message + " when parsing r node in transform");
                }
            }

            // Parse the scale if at least one of sx and sy exist
            if(sxNode != null || syNode != null)
            {
                try
                {
                    float sx = sxNode != null ? float.Parse(sxNode.Value, CultureInfo.InvariantCulture.NumberFormat) : 0;
                    float sy = syNode != null ? float.Parse(syNode.Value, CultureInfo.InvariantCulture.NumberFormat) : 0;
                    t.SetScale(sx, sy);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message + " when parsing sx and/or sy nodes in transform");
                }
            }
        }

        // Parse the xml node of a sprite renderer component
        private void ParseSpriteRenderer(XElement componentNode, SpriteRenderer c)
        {
            // Parse the component name if there is one
            if(componentNode.Attribute("name") != null)
            {
                c.Name = componentNode.Attribute("name").Value.ToString();
            }

            // Parse the texture node if there is one
            var textureNode = componentNode.Element("texture");
            if(textureNode != null)
            {
                // Add the texture to the asset manager
                c.Texture = AssetManager.Instance.AddTexture(projectPath + "/" + textureNode.Value.ToString());
            }
        }

        // Parse the xml node of a script component
        private void ParseScript(XElement scriptNode, Script s)
        {
            // Parse the component name if there is one
            if (scriptNode.Attribute("name") != null)
            {
                s.Name = scriptNode.Attribute("name").Value.ToString();
            }
            
            // Parse the source file node if there is one
            var sourceNode = scriptNode.Element("source");
            if (sourceNode != null)
            {
                s.SourceFile = sourceNode.Value.ToString();
            }

            // Parse the class name node if there is one
            var classnameNode = scriptNode.Element("classname");
            if (classnameNode != null)
            {
                s.ClassName = classnameNode.Value.ToString();
            }

            // If both source file and class name are given, add script to list of script components
            if(sourceNode != null && classnameNode != null)
            {
                var t = scriptsDll.GetType(s.ClassName);
                IScript i = Activator.CreateInstance(t) as IScript;
                if (i != null)
                {
                    s.OnScriptLoad(i);
                }
            }
        }

        // Parse the xml node of a tilemap renderer component
        private void ParseTilemapRenderer(XElement componentNode, TilemapRenderer t)
        {
            // Parse the component name if there is one
            if (componentNode.Attribute("name") != null)
            {
                t.Name = componentNode.Attribute("name").Value.ToString();
            }

            // Parse the tileset node if there is one
            var tilesetNode = componentNode.Element("tileset");
            if (tilesetNode != null)
            {
                // Create the tileset object
                t.Tileset = new Tileset();

                // Parse the texture node if there is one
                var textureNode = tilesetNode.Element("texture");
                if (textureNode != null)
                {
                    // Add the texture to the asset manager
                    t.Tileset.Texture = AssetManager.Instance.AddTexture(projectPath + "/" + textureNode.Value.ToString());
                }

                // Parse the row length node if there is one
                var rowLengthNode = tilesetNode.Element("row-length");
                if (rowLengthNode != null)
                {
                    int rowLength;
                    if (int.TryParse(rowLengthNode.Value, out rowLength))
                    {
                        t.Tileset.RowLength = rowLength;
                    }
                }

                // Parse the col length node if there is one
                var colLengthNode = tilesetNode.Element("col-length");
                if (colLengthNode != null)
                {
                    int colLength;
                    if (int.TryParse(colLengthNode.Value, out colLength))
                    {
                        t.Tileset.ColLength = colLength;
                    }
                }

                // Parse the num tiles node if there is one
                var numTilesNode = tilesetNode.Element("num-tiles");
                if (numTilesNode != null)
                {
                    int numTiles;
                    if (int.TryParse(numTilesNode.Value, out numTiles))
                    {
                        t.Tileset.NumTiles = numTiles;
                    }
                }
            }

            // Parse the tilemap node if there is one
            var tilemapNode = componentNode.Element("tilemap");
            if (tilemapNode != null)
            {
                // Load the tilemap
                t.Tilemap = new Tilemap(AssetManager.Instance.AddTextFile(projectPath + "/" + tilemapNode.Value.ToString()));
            }
        }

        // Load the scenes.xml file
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
                        // Check if the scene is the default scene
                        if(child.Attribute("default") != null)
                        {
                            DefaultSceneName = sceneName;
                        }
                    }
                }
            }
        }

        // Load a graphical user interface
        // The gui name must a key in the Guis map
        public void LoadGui(string guiName)
        {
            if (projectPath != null && Guis.ContainsKey(guiName) && !LoadedGuis.ContainsKey(guiName))
            {
                string path = projectPath + "/" + Guis[guiName];
                UserInterface gui = new UserInterface();

                // Load the xml file
                XElement root = XElement.Load(path);

                // Each child of root should be an entity
                foreach (XElement child in root.Elements())
                {
                    ParseWidget(child, gui.RootComponent);
                }

                // Refresh the transforms of the widgets
                gui.OnRenderFormResize();

                // Add the scene to the loaded scenes map
                LoadedGuis[guiName] = gui;
            }
        }

        // Parse a gui widget
        private void ParseWidget(XElement node, Container parent)
        {
            Widget w = null;

            // Parse the widget specific info
            switch (node.Name.ToString())
            {
                case "button":
                    Button b = new Button(parent);
                    ParseButton(node, b);
                    w = b;
                    break;
            }

            if(w != null)
            {
                // Parse the generic widget info
                foreach(XElement child in node.Elements())
                {
                    switch (child.Name.ToString())
                    {
                        case "transform":
                            ParseTransform(child, w.Transform);
                            break;
                        case "origin":
                            w.Origin = ParseOrigin(child);
                            break;
                    }
                }

                // Add the widget to the parent container
                parent.AddChild(w);
            }
        }

        // Parse a widget origin
        private EOrigin ParseOrigin(XElement node)
        {
            try
            {
                int value = int.Parse(node.Value.ToString());
                // Check value is within enum's range
                if(value >= 0 && value <= 8)
                {
                    return (EOrigin)value;
                }
            }
            catch(FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            // Default to center
            return EOrigin.CENTER;
        }
    
        // Parse a gui button
        private void ParseButton(XElement node, Button b)
        {
            // Parse the texture node if there is one
            var textureNode = node.Element("texture");
            if (textureNode != null)
            {
                // Add the texture to the asset manager
                b.Texture = AssetManager.Instance.AddTexture(projectPath + "/" + textureNode.Value.ToString());
            }
        }

        // Load the guis.xml file
        // File should be at the project root
        public void LoadGuiDeclerationsFile(string path)
        {
            // Create the map from gui name to gui path
            Guis = new Dictionary<string, string>();

            // Load the xml file
            XElement root = XElement.Load(path);

            // Each child of root is a scene decleration
            foreach (XElement child in root.Elements())
            {
                // Ensure the tag is a gui tag and contains the required data
                if (child.Name == "gui" && child.Attribute("name") != null && child.Attribute("path") != null)
                {
                    string guiName = child.Attribute("name").Value.ToString();
                    string guiPath = child.Attribute("path").Value.ToString();
                    if (guiName != null && guiPath != null)
                    {
                        // Add the scene to the Scenes map if the name is not already taken
                        if (!Guis.ContainsKey(guiName))
                        {
                            Guis[guiName] = guiPath;
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
                    case "date-created":
                        dateCreated = child.Value;
                        break;
                    case "date-modified":
                        dateModified = child.Value;
                        break;
                }
            }

            // Set the project info property
            ProjectInfo = new ProjectInfo(name, engineVersion, gameVersion, dateCreated, dateModified);
        }

        // Load the info.xml file
        // File should be at the project root
        private void LoadInputFile(string path)
        {
            // Initialize the input manager (done after DX context loaded)
            InputManager.Instance.Initialize();

            // Load the xml file
            XElement root = XElement.Load(path);

            // In info file, all elements are direct children of root
            foreach (XElement child in root.Elements())
            {
                // All boolean and axis inputs must have a name
                if (child.Attribute("name") != null)
                {
                    string inputName = child.Attribute("name").Value.ToString();
                    switch (child.Name.ToString())
                    {
                        // Boolean inputs are used for on/off type input
                        case "boolean":
                            BooleanInput b = new BooleanInput(inputName);
                            ParseBooleanInput(child, b);
                            InputManager.Instance.AddBooleanInput(b);
                            break;
                        // Axis inputs are used for variable strength input with 2 endpoints
                        case "axis":
                            AxisInput a = new AxisInput(inputName);
                            ParseAxisInput(child, a);
                            InputManager.Instance.AddAxisInput(a);
                            break;
                    }
                }
            }
        }

        // Parse a list of input methods within a boolean/axis input
        private void ParseBooleanInput(XElement parent, BooleanInput booleanInput)
        {
            foreach(XElement inputNode in parent.Elements())
            {
                switch (inputNode.Name.ToString())
                {
                    case "key":
                        // Get the key required by input method
                        int key = int.Parse(inputNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        // Create the new input method
                        Input.Input i = new KeyboardInput((SharpDX.DirectInput.Key)key);
                        // Add the input method to the boolean input
                        booleanInput.AddInput(i);
                        break;
                    case "mouse":
                        // Get the mouse button required by input method
                        int btn = int.Parse(inputNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        // Create the new input method
                        // TODO
                        break;
                }
            }
        }

        // Parse a list of input methods within a boolean/axis input
        private void ParseAxisInput(XElement parent, AxisInput axisInput)
        {
            // Axis contains 2 input groups, the positive input methods and the negative input methods
            foreach (XElement c in parent.Elements())
            {
                bool pos = c.Name.ToString() == "pos";
                foreach (XElement inputNode in c.Elements())
                {
                    switch (inputNode.Name.ToString())
                    {
                        case "key":
                            // Get the key required by input method
                            int key = int.Parse(inputNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                            // Create the new input method
                            Input.Input i = new KeyboardInput((SharpDX.DirectInput.Key)key);
                            // Add the input method to the axis input
                            if (pos)
                            {
                                axisInput.AddPosInput(i);
                            }
                            else
                            {
                                axisInput.AddNegInput(i);
                            }
                            break;
                        case "mouse":
                            // Get the mouse button required by input method
                            int btn = int.Parse(inputNode.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                            // Create the new input method
                            // TODO
                            break;
                    }
                }
            }
        }

        // Load the DLL used to store game scripts
        private void LoadScriptsDLL(string path)
        {
            Console.WriteLine("Loading scripts dll");

            // Load the scripts dll file
            scriptsDll = Assembly.LoadFile(path);
        }
    }
}
