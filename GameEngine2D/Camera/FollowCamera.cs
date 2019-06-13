using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;
using GameEngine2D.Math;
using GameEngine2D.EngineCore;

namespace GameEngine2D.Camera
{
    public class FollowCamera : Camera
    {
        public Transform Offset { get; private set; }

        public FollowCamera(Entity parent) : base(parent)
        {
            Offset = new Transform();
        }

        public override void Update()
        {
            // Set the camera transform to the transform of the entity
            Transform.SetTranslation(-parent.Transform.Position);
            Transform.SetScale(parent.Transform.Scale);
            Transform.SetRotation(-parent.Transform.Rotation);

            // Focus the entity at the centre
            Transform.Translate(Context.Instance.Game.Width / 2, Context.Instance.Game.Height / 2);

            // Apply the transform offset
            Transform.Translate(Offset.Position);
            Transform.ScaleBy(Offset.Scale);
            Transform.Rotate(Offset.Rotation);
        }
    }
}
