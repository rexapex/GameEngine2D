using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameEngine2D.Gui
{
    public class UserInterface
    {
        // Makes use of the singleton pattern since only one user interface will ever be needed
        private static readonly UserInterface instance = new UserInterface();

        private static bool initialized = false;

        static UserInterface() { }

        // Private constructor so no other instances can be made
        private UserInterface() { }

        public static UserInterface Instance
        {
            get => instance;
        }

        public void Initialize()
        {
            if (!initialized)
            {
                // Singleton can only be initialized once
                initialized = true;
            }
        }

        // The main container to be rendered and updated
        private Container mainContainer;

        // Update the user interface
        public void Update()
        {
            if(mainContainer != null)
            {
                mainContainer.Update();
            }
        }

        // Draw the user interface
        public void Draw(Matrix projMatrix)
        {
            if(mainContainer != null)
            {
                mainContainer.Draw(projMatrix);
            }
        }

        // Switch the main container of the gui, only sub-widgets of this container will be displayed and updated
        public void SwitchMainContainer(Container c)
        {
            if(c != null)
            {
                mainContainer = c;
            }
        }
    }
}
