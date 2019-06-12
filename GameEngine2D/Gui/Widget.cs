using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using GameEngine2D.Math;

namespace GameEngine2D.Gui
{
    // Widget is the type from which all Gui components derive
    public abstract class Widget
    {
        public string Name { get; set; }
        public EOrigin Origin { get; set; }
        public Transform Transform { get; private set; }

        public int Width { get; set; }
        public int Height { get; set; }
        protected Transform OriginTransform;

        protected Widget parent;

        public Widget(Widget parent)
        {
            this.parent = parent;
            Width = 32;
            Height = 32;
            Origin = EOrigin.CENTER;
            Transform = new Transform();
            OriginTransform = new Transform();
        }

        // Update the widget and determine whether input triggers events
        abstract public void Update();

        // Draw the widget to the screen
        abstract public void Draw(Matrix worldProjMatrix);

        // Called when the parent widget is resized
        virtual public void OnParentResize() { }
    }
}
