using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GameEngine2D.Input
{
    public class KeyboardInput : Input
    {
        public Key Key { get; private set; }

        public KeyboardInput(Key key) : base()
        {
            Key = key;
        }

        // If the input method match, the input state is updated
        public override void UpdateState(Key key, bool keydown)
        {
            if(Key == key)
            {
                State = keydown;
            }
        }
    }
}
