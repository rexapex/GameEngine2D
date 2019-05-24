using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GameEngine2D.Input
{
    class BooleanInput
    {
        public string Name { get; private set; }
        public bool State { get; private set; }
        private List<Input> inputs;

        public BooleanInput(string name)
        {
            Name = name;
            State = false;
            inputs = new List<Input>();
        }

        public void AddInput(Input input)
        {
            if(input != null)
            {
                // TODO - Check for adding input multiple times
                inputs.Add(input);
            }
        }

        public void RemoveKey(Input input)
        {
            if (input != null)
            {
                inputs.Remove(input);
            }
        }
    }
}
