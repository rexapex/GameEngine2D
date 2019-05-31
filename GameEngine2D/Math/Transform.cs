using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using GameEngine2D.EntitySystem;

namespace GameEngine2D.Math
{
    public class Transform
    {
        // IMPORTANT - Ignore VS2017 Redundancy Notifications
        // Replacing private variables with auto properties are not equivalent in this case
        // Rotation value is stored in radians
        private Vector2 position;
        private Vector2 scale;
        private float rotation;

        // Global transform variables give the world transform as opposed to the model transform
        private Vector2 globalPosition;
        private Vector2 globalScale;
        private float globalRotation;

        // Global world matrix of the transform
        private Matrix worldMatrix;
        
        // Allow direct access to the values without allowing direct modification
        public Vector2 Position { get => position; }
        public Vector2 Scale { get => scale; }
        public float Rotation { get => rotation; }

        // Allow direct access to the values without allowing direct modification
        public Vector2 GlobalPosition { get => globalPosition; }
        public Vector2 GlobalScale { get => globalScale; }
        public float GlobalRotation { get => globalRotation; }

        // Allow direct access to the values without allowing direct modification
        public Matrix WorldMatrix { get => worldMatrix; }

        // The parent entity which this transform belongs to
        // Null if the transform does not belong to an entity
        public Entity Parent { get; private set; }
        
        public Transform()
        {
            Parent = null;
            position = new Vector2(0);
            scale = new Vector2(1);
            rotation = 0;
            UpdateGlobalTranslation(position);
            UpdateGlobalScale(scale);
        }

        public Transform(Entity parent)
        {
            Parent = parent;
            position = new Vector2(0);
            scale = new Vector2(1);
            rotation = 0;
            UpdateGlobalTranslation(position);
            UpdateGlobalScale(scale);
        }

        // Set the translation vector by a vector
        public void SetTranslation(Vector2 translation)
        {
            if(translation != null)
            {
                // Set the relative position of this transform object
                position = translation;

                // Update the global position of this transform & child transforms
                UpdateGlobalTranslation(position);
            }
        }

        // Set the translation vector by x and y values
        public void SetTranslation(float x, float y)
        {
            // Set the relative position of this transform object
            position.X = x;
            position.Y = y;

            // Update the global position of this transform & child transforms
            UpdateGlobalTranslation(position);
        }

        // Set the scale vector by a vector
        public void SetScale(Vector2 newScale)
        {
            if (newScale != null)
            {
                // Set the relative scale
                scale = newScale;

                // Update the global scale
                UpdateGlobalScale(scale);
            }
        }

        // Set the scale vector by x and y values
        public void SetScale(float sx, float sy)
        {
            // Set the relative scale
            scale.X = sx;
            scale.Y = sy;

            // Update the global scale
            UpdateGlobalScale(scale);
        }

        // Set the rotation
        // Value of r is to be given in radians
        public void SetRotation(float r)
        {
            // Set the relative rotation
            rotation = r % MathConstants.Tau;

            // Update the global rotation
            UpdateGlobalRotation(rotation);
        }

        // Set the rotation
        // Value of r is to be given in degrees
        public void SetRotationDegrees(float r)
        {
            // Set the relative rotation
            rotation = MathConstants.DegreesToRadiansRatio * (r % 360);

            // Update the global rotation
            UpdateGlobalRotation(rotation);
        }

        // Translate by a vector
        public void Translate(Vector2 translation)
        {
            if(translation != null)
            {
                // Update the relative position
                position += translation;

                // Update the global position
                UpdateGlobalTranslation(position);
            }
        }

        // Translate by x and y values
        public void Translate(float dx, float dy)
        {
            // Update the relative position
            position.X += dx;
            position.Y += dy;

            // Update the global position
            UpdateGlobalTranslation(position);
        }

        // Scale by a vector
        public void ScaleBy(Vector2 scaler)
        {
            if (scale != null)
            {
                // Update the relative scale
                scale *= scaler;

                // Update the global scale
                UpdateGlobalScale(scale);
            }
        }

        // Scale by x and y values
        public void ScaleBy(float sx, float sy)
        {
            // Update the relative scale
            scale.X *= sx;
            scale.Y *= sy;

            // Update the global scale
            UpdateGlobalScale(scale);
        }

        // Rotate by an angle
        // Angle given in radians
        public void Rotate(float angle)
        {
            // Set the relative rotation
            rotation = (rotation + angle) % MathConstants.Tau;

            // Update the global rotation
            UpdateGlobalRotation(rotation);
        }

        // Rotate by an angle
        // Angle given in degrees
        public void RotateDegrees(float angle)
        {
            // Set the relative rotation
            rotation = (rotation + (MathConstants.DegreesToRadiansRatio * angle)) % MathConstants.Tau;

            // Update the global rotation
            UpdateGlobalRotation(rotation);
        }

        // Update the global translation of this transform & all child transforms
        private void UpdateGlobalTranslation(Vector2 translation)
        {
            // Apply translation to this entities global translation vector
            if (Parent != null && Parent.Parent != null)
            {
                // Add the relative offset to the global position of this entities parent
                globalPosition = Parent.Parent.Transform.GlobalPosition + translation;

                // Apply translation to all child entities
                foreach (Entity e in Parent.GetEntities())
                {
                    e.Transform.UpdateGlobalTranslation(translation);
                }
            }
            else
            {
                globalPosition = translation;
            }

            // Calculate the world matrix of the transform
            CalcWorldMatrix();
        }

        // Update the global scale of this transform & all child transforms
        private void UpdateGlobalScale(Vector2 scale)
        {
            if (Parent != null && Parent.Parent != null)
            {
                // Apply scale to this entities global scale vector
                globalScale = Parent.Parent.Transform.GlobalScale * scale;

                // Apply scale to all child entities
                foreach (Entity e in Parent.GetEntities())
                {
                    e.Transform.UpdateGlobalScale(scale);
                }
            }
            else
            {
                globalScale = scale;
            }

            // Calculate the world matrix of the transform
            CalcWorldMatrix();
        }

        // Update the global rotation of this transform & all child transforms
        private void UpdateGlobalRotation(float rotation)
        {
            if (Parent != null && Parent.Parent != null)
            {
                // Apply rotation to this entities global rotation value
                globalRotation = (Parent.Parent.Transform.globalRotation + rotation) % MathConstants.Tau;

                // Apply rotation to all child entities
                foreach (Entity e in Parent.GetEntities())
                {
                    e.Transform.UpdateGlobalRotation(rotation);
                }
            }
            else
            {
                globalRotation = rotation % MathConstants.Tau;
            }

            // Calculate the world matrix of the transform
            CalcWorldMatrix();
        }

        // Update the world matrix
        private void CalcWorldMatrix()
        {
            worldMatrix = Matrix.Scaling(globalScale.X, globalScale.Y, 1) *
                Matrix.RotationZ(globalRotation) *
                Matrix.Translation(globalPosition.X, globalPosition.Y, 0);
        }
    }
}
