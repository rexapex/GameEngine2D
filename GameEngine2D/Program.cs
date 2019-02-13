using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EngineCore;
using GameEngine2D.EntitySystem;
using GameEngine2D.Math;

namespace GameEngine2D
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Start();
            }
            Console.ReadKey();
        }
    }
}
