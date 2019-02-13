using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameEngine2D.Math
{
    class Transform
    {
        // IMPORTANT - Ignore VS2017 Redundancy Notifications
        // Replacing private variables with auto properties are not equivalent in this case
        // Rotation value is stored in radians
        private Vector2 position;
        private Vector2 scale;
        private float rotation;

        // Allow direct access to the values without allowing direct modification
        public Vector2 Position { get => position; }
        public Vector2 Scale { get => scale; }
        public float Rotation { get => rotation; }
        
        public Transform()
        {
            position = new Vector2(0);
            scale = new Vector2(1);
            rotation = 0;
        }

        // Set the translation vector by a vector
        public void SetTranslation(Vector2 translation)
        {
            if(translation != null)
            {
                position = translation;
            }
        }

        // Set the translation vector by x and y values
        public void SetTranslation(float x, float y)
        {
            position.X = x;
            position.Y = y;
        }

        // Set the scale vector by a vector
        public void SetScale(Vector2 newScale)
        {
            if (newScale != null)
            {
                scale = newScale;
            }
        }

        // Set the scale vector by x and y values
        public void SetScale(float sx, float sy)
        {
            scale.X = sx;
            scale.Y = sy;
        }

        // Set the rotation
        // Value of r is to be given in radians
        public void SetRotation(float r)
        {
            // TODO - Create a PI constant using float type
            rotation = r % MathConstants.Tau;
        }

        // Set the rotation
        // Value of r is to be given in degrees
        public void SetRotationDegrees(float r)
        {
            rotation = MathConstants.DegreesToRadiansRatio * (r % 360);
        }

        // Translate by a vector
        public void Translate(Vector2 translation)
        {
            if(translation != null)
            {
                position += translation;
            }
        }

        // Translate by x and y values
        public void Translate(float dx, float dy)
        {
            position.X += dx;
            position.Y += dy;
        }

        // Scale by a vector
        public void ScaleBy(Vector2 scaler)
        {
            if (scale != null)
            {
                scale *= scaler;
            }
        }

        // Scale by x and y values
        public void ScaleBy(float sx, float sy)
        {
            scale.X *= sx;
            scale.Y *= sy;
        }

        // Rotate by an angle
        // Angle given in radians
        public void Rotate(float angle)
        {
            rotation = (rotation + angle) % MathConstants.Tau;
        }

        // Rotate by an angle
        // Angle given in degrees
        public void RotateDegrees(float angle)
        {
            rotation = (rotation + (MathConstants.DegreesToRadiansRatio * angle)) % MathConstants.Tau;
        }
    }
}
