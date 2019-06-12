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
                Context.Initialize(game);
                ProjectManager.Instance.LoadProject("D:/James/Documents/CSG Engine/Projects/TestProject");
                Console.WriteLine(ProjectManager.Instance.ProjectInfo);
                game.SwitchScene(ProjectManager.Instance.DefaultSceneName);
                game.Start();
            }
            Console.ReadKey();
        }
    }
}
