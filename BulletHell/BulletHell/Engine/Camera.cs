using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    public class Camera
    {
        Vector2 position;
        float scale;

        public int Width { get; set; }
        public int Height { get; set; }

        public int X { get { return (int)Math.Floor(position.X); } }
        public int Y { get { return (int)Math.Floor(position.Y); } }

        Vector2 center;
        public Vector2 Origin { get; set; }

        // Are we shaking?
        private bool shaking;

        // The maximum magnitude of our shake offset
        private float shakeMagnitude;

        // The total duration of the current shake
        private float shakeDuration;

        // A timer that determines how far into our shake we are
        private float shakeTimer;

        // The shake offset vector
        private Vector2 shakeOffset;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public float NewScale { get; set; }
        public float Rotation { get; set; }
        public Matrix Transform { get; set; }
        public Entity Focus { get; set; }

        public Rectangle Bounds { get; set; }

        public Camera(Game game)
        {
            Width = game.GraphicsDevice.Viewport.Width;
            Height = game.GraphicsDevice.Viewport.Height;
            Bounds = game.GraphicsDevice.Viewport.Bounds;
            center = new Vector2(Width / 2, Height / 2);
            scale = 1;
            NewScale = scale;
            Rotation = 0;
        }

        public void ResetViewport(Viewport v)
        {
            Width = v.Width;
            Height = v.Height;
            center = new Vector2(Width / 2, Height / 2);
        }

        public void Update(float elapsed)
        {
            scale += (NewScale - Scale) * 4f * elapsed;

            Transform = Matrix.Identity *
                        Matrix.CreateTranslation(-X, -Y, 0) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                        Matrix.CreateScale(new Vector3(scale, scale, scale));

            Origin = center / scale;

            // If we're shaking...
            if (shaking)
            {
                // Move our timer ahead based on the elapsed time
                shakeTimer += elapsed;

                // If we're at the max duration, we're not going to be shaking anymore
                if (shakeTimer >= shakeDuration)
                {
                    shaking = false;
                    shakeTimer = shakeDuration;
                }

                // Compute our progress in a [0, 1] range
                float progress = shakeTimer / shakeDuration;

                // Compute our magnitude based on our maximum value and our progress. This causes
                // the shake to reduce in magnitude as time moves on, giving us a smooth transition
                // back to being stationary. We use progress * progress to have a non-linear fall 
                // off of our magnitude. We could switch that with just progress if we want a linear 
                // fall off.
                float magnitude = shakeMagnitude * (1f - (progress * progress));

                // Generate a new offset vector with two random values and our magnitude
                shakeOffset = new Vector2(Util.NextFloat() * 2f - 1f, Util.NextFloat() * 2f - 1f) * magnitude;

                // If we're shaking, add our offset to our position and target
                Origin += shakeOffset;
                //Focus.Position += shakeOffset;
            }

            float speed = 4f;

            position.X += (Focus.X + (Focus.Width / 2) - position.X) * speed * elapsed;
            position.Y += (Focus.Y + (Focus.Height / 2) - position.Y) * speed * elapsed;

            if (Bounds != Rectangle.Empty)
            {
                position = Vector2.Clamp(position, new Vector2(Bounds.Left + Origin.X, Bounds.Top + Origin.Y), new Vector2(Bounds.Right - Origin.X, Bounds.Bottom - Origin.Y));
            }
        }

        /// <summary>
        /// Shakes the camera with a specific magnitude and duration. Taken from http://xnaessentials.com/archive/2011/04/26/shake-that-camera.aspx
        /// </summary>
        /// <param name="magnitude">The largest magnitude to apply to the shake.</param>
        /// <param name="duration">The length of time (in seconds) for which the shake should occur.</param>
        public void Shake(float magnitude, float duration)
        {
            // We're now shaking
            shaking = true;

            // Store our magnitude and duration
            shakeMagnitude = magnitude;
            shakeDuration = duration;

            // Reset our timer
            shakeTimer = 0f;
        }
    }
}
