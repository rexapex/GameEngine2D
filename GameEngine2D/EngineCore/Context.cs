using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine2D.EngineCore
{
    public class Context
    {
        // Makes use of the singleton pattern since only one context will ever be needed
        private static readonly Context instance = new Context();

        private static bool initialized = false;

        static Context() { }

        // Private constructor so no other instances can be made
        private Context() { }

        public static Context Instance
        {
            get => instance;
        }

        // Properties
        public Game Game { get; private set; }

        public static void Initialize(Game g)
        {
            if(!initialized && g != null)
            {
                initialized = true;
                instance.Game = g;
            }
        }
    }
}
