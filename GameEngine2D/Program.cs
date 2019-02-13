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
            Entity table = new Entity()
            {
                Name = "Table"
            };

            Entity leg1 = new Entity()
            {
                Name = "Leg 1"
            };

            table.AddEntity(leg1);

            Console.WriteLine(table.GetEntities());

            Transform t = new Transform();
            t.SetTranslation(6, 4);
            Console.WriteLine(t.Position);
            Console.WriteLine(t.Position.X);
            Console.WriteLine((float)System.Math.PI);
            Console.WriteLine((float)(System.Math.PI*2));
            Console.WriteLine((float)(System.Math.PI / 180));

            //using (Game game = new Game())
            //{
            //    game.Start();
            //}
            Console.ReadKey();
        }
    }
}
