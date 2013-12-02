using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    public class Player : Entity
    {
        float shootTimer = 0;

        float damageTimer = 2;

        public float ShootDelay { get; set; }

        public List<Bullet> Bullets { get; set; }

        public override Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public Player(Texture2D texture)
            : base(texture)
        {
            Width = 48;
            Height = 60;

            XOffset = (texture.Width - Width) / 2;
            YOffset = (texture.Height - Height) / 2;

            ShootDelay = 0.2f;

            Health = 5;

            Bullets = new List<Bullet>();
        }

        public void Shoot(Direction d)
        {
            if (shootTimer <= 0)
            {
                // Shoot
                Bullet b = new Bullet();
                b.Position = position;
                switch (d)
                {
                    case Direction.Left:
                        b.Velocity = new Vector2(-1, 0);
                        break;
                    case Direction.Right:
                        b.Velocity = new Vector2(1, 0);
                        break;
                    case Direction.Up:
                        b.Velocity = new Vector2(0, -1);
                        break;
                    case Direction.Down:
                        b.Velocity = new Vector2(0, 1);
                        break;
                    default:
                        break;
                }
                Bullets.Add(b);
                shootTimer += ShootDelay;
            }
        }

        public void TakeDamage(int damage)
        {
            if (damageTimer <= 0)
            {
                Health -= damage;
                damageTimer += 1;
            }
            if (Health <= 0)
            {
                // DEAD
            }
        }

        public override void Update(float elapsed)
        {
            if (shootTimer > 0)
            {
                shootTimer -= elapsed;
            }
            if (damageTimer > 0)
            {
                damageTimer -= elapsed;
            }

            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update(elapsed);

                foreach (Entity e in Level.Entities)
                {
                    if (Bullets[i].CollisionBox.Intersects(e.CollisionBox))
                    {
                        Bullets[i].OnCollide(e);
                    }
                }
                
                if (Level.GetTile(Bullets[i].X / Tile.Size, Bullets[i].Y / Tile.Size).IsSolid())
                {
                    Bullets[i].IsActive = false;
                }
                if (!Bullets[i].IsActive)
                {
                    Bullets.RemoveAt(i);
                    i--;
                }
            }

            if (Level.Complete && CollisionBox.Intersects(Level.ExitTile))
            {
                Level.GoToNextLevel = true;
            }
            base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (Bullet b in Bullets)
            {
                b.Draw(spriteBatch);
            }
        }
    }
}
