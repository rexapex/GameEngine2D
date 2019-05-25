using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GameEngine2D.Input
{
    public abstract class Input
    {
        public bool State { get; protected set; }

        public Input()
        {
            State = false;
        }

        // If the input method match, the input state is updated
        public abstract void UpdateState(Key key, bool keydown);
    }
}
