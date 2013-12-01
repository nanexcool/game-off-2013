using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    class Enemy : Entity
    {
        public Entity Target { get; set; }

        public List<Vector2> Path { get; set; }

        public float Speed { get; set; }

        // Pathfinding updates
        float pathElapsed = 0;
        float pathTimeToCheck = 1;

        public Enemy(Texture2D texture)
            : base(texture)
        {
            Width = 32;
            Height = 32;

            XOffset = (texture.Width - Width) / 2;
            YOffset = (texture.Height - Height) / 2;

            Speed = Util.Next(100, 200);
        }

        public override void Update(float elapsed)
        {
            if (Target != null)
            {
                if (!CanFly)
                {
                    pathElapsed += elapsed;
                    if (pathElapsed >= pathTimeToCheck)
                    {
                        Path.Clear();
                        Path = Level.Pathfinder.FindPath(new Point(X / Tile.Size, Y / Tile.Size), new Point(Target.X / Tile.Size, Target.Y / Tile.Size));
                        pathElapsed -= pathTimeToCheck;
                    }

                    if (Path.Count > 0)
                    {
                        float distance = Vector2.Distance(Path[0], position);
                        if (distance < 5)
                        {
                            Path.RemoveAt(0);
                            if (Path.Count == 0)
                            {
                                velocity = Vector2.Zero;
                            }
                        }
                        else
                        {
                            Vector2 target = Path[0] - position;
                            target.Normalize();
                            velocity = target * Speed;
                        }
                    }
                    else
                    {
                        velocity = Vector2.Zero;
                    }
                }
                else
                {
                    Vector2 target = Target.Position - position;
                    target.Normalize();
                    velocity = target * Speed;
                }
                
            }

            base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(Path.Count.ToString());
            //spriteBatch.DrawString(Util.Font, sb, new Vector2(DrawRectangle.Left, DrawRectangle.Bottom), Color.Blue);
        }
    }
}
