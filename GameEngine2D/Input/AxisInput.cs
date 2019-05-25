using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GameEngine2D.Input
{
    class AxisInput
    {
        public string Name { get; private set; }
        public float Value { get; private set; }    // Value between -1 and 1 inclusively
        private List<Input> posInputs;
        private List<Input> negInputs;

        public AxisInput(string name)
        {
            Name = name;
            Value = 0.0f;
            posInputs = new List<Input>();
            negInputs = new List<Input>();
        }

        public void UpdateValue()
        {
            float posValue = 0;
            float negValue = 0;
            // Value is the result of posValue - negValue
            // Test the positive inputs
            foreach (Input i in posInputs)
            {
                if (i.State)
                {
                    posValue = 1;
                    break;
                }
            }
            // Test the negative inputs
            foreach(Input i in negInputs)
            {
                if(i.State)
                {
                    negValue = 1;
                    break;
                }
            }
            // Calculate the overall value
            Value = posValue - negValue;
        }

        public void onKeyUpdate(Key key, bool pressed)
        {
            // Update each individual input based on the key press
            foreach (Input i in posInputs)
            {
                i.UpdateState(key, pressed);
            }
            foreach (Input i in negInputs)
            {
                i.UpdateState(key, pressed);
            }
        }

        public void AddPosInput(Input input)
        {
            if (input != null)
            {
                // TODO - Check for adding input multiple times
                posInputs.Add(input);
            }
        }

        public void AddNegInput(Input input)
        {
            if (input != null)
            {
                // TODO - Check for adding input multiple times
                negInputs.Add(input);
            }
        }

        public void RemovePosInput(Input input)
        {
            if (input != null)
            {
                posInputs.Remove(input);
            }
        }

        public void RemoveNegInput(Input input)
        {
            if (input != null)
            {
                negInputs.Remove(input);
            }
        }
    }
}
