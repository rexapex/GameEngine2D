using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GameEngine2D.Input
{
    class KeyboardInput : Input
    {
        public Key Key { get; private set; }

        public KeyboardInput(Key key) : base()
        {
            Key = key;
        }
    }
}
