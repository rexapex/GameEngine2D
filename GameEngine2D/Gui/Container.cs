using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameEngine2D.Gui
{
    public class Container : Widget
    {
        // List of all direct subcomponents
        private List<Widget> childWidgets;

        public Container() : base()
        {
            Origin = EOrigin.BOTTOM_LEFT;
            Transform.SetTranslation(0, 0);
            Transform.SetScale(1, 1);
            Transform.SetRotation(0);
            childWidgets = new List<Widget>();
        }

        // Update the child widgets and determine whether input triggers events
        override public void Update()
        {
            foreach(Widget w in childWidgets)
            {
                w.Update();
            }
        }

        // Draw the child widgets to the screen
        override public void Draw(Matrix worldProjMatrix)
        {
            foreach(Widget w in childWidgets)
            {
                w.Draw(worldProjMatrix);
            }
        }

        // Add a child widget to the container
        public void AddChild(Widget w)
        {
            if(w != null && !childWidgets.Contains(w))
            {
                childWidgets.Add(w);
            }
        }
    }
}
