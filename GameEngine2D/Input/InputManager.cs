﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GameEngine2D.Input
{
    class InputManager
    {
        // SharpDX objects
        private DirectInput directInput;
        private Keyboard keyboard;
        private Mouse mouse;

        // Boolean inputs
        private Dictionary<string, BooleanInput> booleanInputs;

        public InputManager()
        {
            // Instantiate direct input objects
            directInput = new DirectInput();
            keyboard = new Keyboard(directInput);
            mouse = new Mouse(directInput);

            // Acquire the keyboard
            keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();

            // Acquire the mouse
            mouse.Properties.BufferSize = 128;
            mouse.Acquire();

            // Initialize input dictionaries
            booleanInputs = new Dictionary<string, BooleanInput>();
        }

        public void Update()
        {
            // Poll the mouse
            mouse.Poll();
            foreach(var state in mouse.GetBufferedData())
            {
                // NOTE - state contains Offset and Value fields
                Console.WriteLine(state);
            }

            // Poll the keyboard
            keyboard.Poll();
            foreach(var state in keyboard.GetBufferedData())
            {
                // NOTE - state contains Key and IsPressed fields
                
            }
        }

        // Add a new boolean input to the input manager
        // Returns true iff the input is added sucessfully
        public bool AddBooleanInput(BooleanInput input)
        {
            if(input != null && input.Name != null)
            {
                if(!booleanInputs.ContainsKey(input.Name))
                {
                    booleanInputs[input.Name] = input;
                    return true;
                }
            }
            return false;
        }

        // Remove a boolean input by name
        // Returns true iff the input existed and is removed sucessfully
        public bool RemoveBooleanInput(string name)
        {
            if(booleanInputs.ContainsKey(name))
            {
                booleanInputs.Remove(name);
                return true;
            }
            return false;
        }
    }
}
