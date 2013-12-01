using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    public class Entity
    {
        protected Vector2 position;
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected Vector2 oldPosition;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public int X { get { return (int)Math.Floor(position.X); } }
        public int Y { get { return (int)Math.Floor(position.Y); } }

        public int Width { get; set; }
        public int Height { get; set; }

        public int XOffset { get; set; }
        public int YOffset { get; set; }

        protected bool canMoveHorizontal = true;
        protected bool canMoveVertical = true;

        public bool CanFly { get; set; }

        public Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(X - XOffset, Y - YOffset, Width + XOffset * 2, Height + YOffset * 2);
            }
        }

        public Texture2D Texture { get; set; }
        public Color Color { get; set; }

        public Level Level { get; set; }

        public bool CanRemove { get; set; }

        public Entity(Texture2D texture)
        {
            Texture = texture;
            Width = texture.Width;
            Height = texture.Height;
            Color = Color.White;

            CanFly = false;
            
            CanRemove = false;
        }

        public virtual void Update(float elapsed)
        {
            if (velocity.X != 0 || velocity.Y != 0)
            {
                Move(elapsed);

                position = Vector2.Clamp(position, Vector2.Zero, new Vector2(Level.Width * Tile.Size - Width, Level.Height * Tile.Size - Height));
            }
        }

        private void Move(float elapsed)
        {
            Vector2 wantedPosition = position;
            canMoveHorizontal = true;
            canMoveVertical = true;

            if (!CanFly)
            {
                // Moving LEFT
                if (velocity.X < 0)
                {
                    wantedPosition.X += velocity.X * elapsed;

                    int x1 = (int)(wantedPosition.X / Tile.Size);
                    int x2 = (int)((wantedPosition.X + Width) / Tile.Size);
                    int y1 = (int)(wantedPosition.Y / Tile.Size);
                    int y2 = (int)((wantedPosition.Y + Height) / Tile.Size);

                    for (int y = y1; y <= y2; y++)
                    {
                        if (Level.GetTile(x1, y).IsSolid())
                        {
                            canMoveHorizontal = false;
                            velocity.X = 0;
                        }
                    }
                }
                // Moving RIGHT
                if (velocity.X > 0)
                {
                    wantedPosition.X += velocity.X * elapsed;

                    int x1 = (int)(wantedPosition.X / Tile.Size);
                    int x2 = (int)((wantedPosition.X + Width) / Tile.Size);
                    int y1 = (int)(wantedPosition.Y / Tile.Size);
                    int y2 = (int)((wantedPosition.Y + Height) / Tile.Size);

                    for (int y = y1; y <= y2; y++)
                    {
                        if (Level.GetTile(x2, y).IsSolid())
                        {
                            canMoveHorizontal = false;
                            velocity.X = 0;
                        }
                    }
                }

                // reset X position
                wantedPosition.X = position.X;

                // Moving UP
                if (velocity.Y < 0)
                {
                    wantedPosition.Y += velocity.Y * elapsed;

                    int x1 = (int)(wantedPosition.X / Tile.Size);
                    int x2 = (int)((wantedPosition.X + Width) / Tile.Size);
                    int y1 = (int)(wantedPosition.Y / Tile.Size);
                    int y2 = (int)((wantedPosition.Y + Height) / Tile.Size);

                    for (int x = x1; x <= x2; x++)
                    {
                        if (Level.GetTile(x, y1).IsSolid())
                        {
                            canMoveVertical = false;
                            velocity.Y = 0;
                        }
                    }
                }
                // Moving DOWN
                if (velocity.Y > 0)
                {
                    wantedPosition.Y += velocity.Y * elapsed;

                    int x1 = (int)(wantedPosition.X / Tile.Size);
                    int x2 = (int)((wantedPosition.X + Width) / Tile.Size);
                    int y1 = (int)(wantedPosition.Y / Tile.Size);
                    int y2 = (int)((wantedPosition.Y + Height) / Tile.Size);

                    for (int x = x1; x <= x2; x++)
                    {
                        if (Level.GetTile(x, y2).IsSolid())
                        {
                            canMoveVertical = false;
                            velocity.Y = 0;
                        }
                    }
                }
            }

            if (canMoveHorizontal)
            {
                position.X += velocity.X * elapsed;
            }

            if (canMoveVertical)
            {
                position.Y += velocity.Y * elapsed;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle drawRect = DrawRectangle;
            
            spriteBatch.Draw(Texture, new Vector2(position.X - XOffset, position.Y - YOffset), Color);
            
            //spriteBatch.Draw(Util.Texture, CollisionBox, Color.Blue * 0.5f);
        }
    }
}
