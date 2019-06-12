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
        // The root widget to which all other widgets are children
        public Container RootComponent { get; private set; }

        public UserInterface()
        {
            RootComponent = new Container();
        }

        // Update the user interface
        public void Update()
        {
            RootComponent.Update();
        }

        // Draw the user interface
        public void Draw(Matrix projMatrix)
        {
            RootComponent.Draw(projMatrix);
        }
    }
}
