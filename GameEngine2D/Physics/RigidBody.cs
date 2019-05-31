using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameEngine2D.Physics
{
    class RigidBody
    {
        public float Mass { get; set; }
        public Vector2 CenterOfMass { get; set; }

        public float LinearDamping { get; set; }
        public Vector2 LinearVelocity { get; private set; }
        public Vector2 LinearAcceleration { get; private set; }
        public Vector2 LinearForce { get; private set; }

        public float AngularDamping { get; set; }
        public float AngularVelocity { get; private set; }
        public float AngularAcceleration { get; private set; }
        public float AngularForce { get; private set; }

        public RigidBody()
        {
            CenterOfMass = new Vector2();
            LinearVelocity = new Vector2();
            LinearAcceleration = new Vector2();
            LinearForce = new Vector2();
        }

        // Update the transform of the entity based on rigid body physics
        public void Update()
        {

        }

        // Apply a continuous force to the rigid body
        public void ApplyForce(float fx, float fy)
        {
       //     LinearForce.X += fx;
       //     LinearForce.Y += fy;
        }

        // Apply a continuous force to the rigid body
        public void ApplyForce(Vector2 force)
        {

        }

        // Apply a force for a single game tick
        public void ApplyImpulse()
        {

        }

        // Apply a continuous angular force to the rigid body
        public void ApplyAngularForce()
        {

        }

        // Apply an angular force for a single game tick
        public void ApplyAngularImpulse()
        {

        }
    }
}
