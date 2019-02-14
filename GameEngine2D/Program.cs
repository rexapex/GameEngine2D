using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EngineCore;
using GameEngine2D.EntitySystem;
using GameEngine2D.Math;
using GameEngine2D.AssetManagement;

namespace GameEngine2D
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                // TODO - ACK
                ProjectManager.Instance.LoadProject("D:/James/Documents/CSG Engine/Projects/TestProject");
                ProjectManager.Instance.LoadScene("scene1");
                game.SwitchScene("scene1");
                game.Start();
            }
            Console.ReadKey();
        }
    }
}
