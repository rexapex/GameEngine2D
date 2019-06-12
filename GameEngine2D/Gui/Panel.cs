using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameEngine2D.Gui
{
    public class Panel : Container
    {
        // Overflow of panels subcomponents
        public enum Overflow { AUTO=0, SCROLL=1, HIDDEN=2, VISIBLE=3 }

        // The scroll bar setting determines how overflow is handled
        public Overflow OverflowX { get; set; }
        public Overflow OverflowY { get; set; }

        public Panel(Widget parent) : base(parent)
        {
            OverflowX = Overflow.AUTO;
            OverflowY = Overflow.AUTO;
        }

        // Draw all components of the panel
        override public void Draw(Matrix worldProjMatrix)
        {

        }
    }
}
