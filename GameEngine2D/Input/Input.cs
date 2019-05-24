using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine2D.Input
{
    abstract class Input
    {
        public bool State { get; private set; }

        public Input()
        {
            State = false;
        }
    }
}
