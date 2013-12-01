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

        public float ShootDelay { get; set; }

        public List<Bullet> Bullets { get; set; }

        public Player(Texture2D texture)
            : base(texture)
        {
            Width = 48;
            Height = 60;

            XOffset = (texture.Width - Width) / 2;
            YOffset = (texture.Height - Height) / 2;

            ShootDelay = 0.2f;

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

        public override void Update(float elapsed)
        {
            if (shootTimer > 0)
            {
                shootTimer -= elapsed;
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
