using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    public class Bullet : Entity
    {
        float life = 0;

        public float Speed { get; set; }

        public bool IsActive { get; set; }

        public float TimeToLive { get; set; }

        public Bullet() 
            : base(Util.Texture)
        {
            Width = 16;
            Height = 16;

            Speed = 500;

            TimeToLive = 0.8f;

            Color = Color.Aqua;

            IsActive = true;
        }

        public void OnCollide(Entity e)
        {
            if (e is Enemy)
            {
                e.CanRemove = true;
                IsActive = false;
            }
        }

        public override void Update(float elapsed)
        {
            Position += Velocity * Speed * elapsed;
            life += elapsed;

            if (life >= TimeToLive)
            {
                IsActive = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Util.Texture, CollisionBox, new Rectangle(0, 0, Width, Height), Color);
        }
    }
}
