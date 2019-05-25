using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GameEngine2D.Input
{
    public class BooleanInput
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

        public void UpdateState()
        {
            State = false;
            // State is set to true iff at least one of the input methods is true
            foreach(Input i in inputs)
            {
                if(i.State)
                {
                    State = true;
                    break;
                }
            }
        }

        public void onKeyUpdate(Key key, bool pressed)
        {
            // Update each individual input based on the key press
            foreach(Input i in inputs)
            {
                i.UpdateState(key, pressed);
            }
        }

        public void AddInput(Input input)
        {
            if(input != null)
            {
                // TODO - Check for adding input multiple times
                inputs.Add(input);
            }
        }

        public void RemoveInput(Input input)
        {
            if (input != null)
            {
                inputs.Remove(input);
            }
        }
    }
}
