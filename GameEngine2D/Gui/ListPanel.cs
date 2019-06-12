using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine2D.Gui
{
    public class ListPanel : Panel
    {
        // Direction of the panels subcomponents
        public enum Direction { LEFT_TO_RIGHT = 0, RIGHT_TO_LEFT = 1, TOP_TO_BOTTOM = 2, BOTTOM_TO_TOP = 3 };

        // Panel consists of a list of subcomponents, arranged either vertically or horizontally
        public Direction FlowDirection { get; set; }

        public ListPanel(Widget parent) : base(parent)
        {

        }
    }
}
