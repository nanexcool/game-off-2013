using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    public class Level
    {
        public Camera Camera { get; set; }

        public Pathfinder Pathfinder { get; private set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Tile[] Tiles { get; set; }

        public List<Entity> Entities { get; set; }
        public Player Player { get; set; }

        public int NumberOfEnemies { get; set; }

        private float enemyTimer = 0;

        public Level(int width, int height, int enemies)
        {
            Width = width;
            Height = height;

            NumberOfEnemies = enemies;

            Tiles = new Tile[width * height];

            // Initialize all tiles
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tiles[x + y * Width].Color = Color.White * Util.NextFloat();
                    Tiles[x + y * Width].Color = Color.SaddleBrown;
                    if (x % Width == 0 || y % Height == 0 || x == Width -1 || y == Height - 1)
                    {
                        Tiles[x + y * Width].Color = Color.Black;
                    }
                    else if (Util.NextDouble() < 0.1)
                    {
                        Tiles[x + y * Width].Color = Color.Black;
                    }
                }
            }

            Pathfinder = new Pathfinder(this);

            Entities = new List<Entity>();

            
        }

        public void Initialize()
        {
            for (int i = 0; i < NumberOfEnemies; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (AddEnemy())
                    {
                        break;
                    }
                }
            }
        }

        public void AddEntity(Entity e)
        {
            Entities.Add(e);
            e.Level = this;
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return Tiles[0];
            }
            return Tiles[x + y * Width];
        }

        public bool AddEnemy()
        {
            Enemy e;
            if (Util.NextDouble() > 0.5)
            {
                e = new Enemy(Util.OctoTexture);
                e.Color = Color.Brown;
            }
            else
            {
                e = new SquareEnemy();
            }

            int x = Util.Next(1, Width);
            int y = Util.Next(1, Height);

            // Only add enemy if not on solid tile
            if (!GetTile(x, y).IsSolid())
            {
                e.Position = new Vector2(x * Tile.Size, y * Tile.Size);
                e.Path = Pathfinder.FindPath(new Point(e.X / Tile.Size, e.Y / Tile.Size), new Point(Player.X / Tile.Size, Player.Y / Tile.Size));
                e.Target = Player;
                AddEntity(e);
                return true;
            }
            return false;
        }

        public virtual void Update(float elapsed)
        {
            enemyTimer += elapsed;

            if (enemyTimer > 3 && Util.NextDouble() < elapsed)
            {
                enemyTimer = 0;
                //AddEnemy();
            }

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Update(elapsed);
                
                if (Entities[i].CanRemove)
                {
                    if (Entities[i] is Enemy)
                    {
                        NumberOfEnemies--;
                    }
                    Entities.RemoveAt(i);
                    i--;
                }
            }

            Camera.Update(elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Util.Texture, new Rectangle(0, 0, Width * Tile.Size, Height * Tile.Size), Color.Black);

            int x1 = (int)(Camera.X - Camera.Origin.X) / Tile.Size;
            int y1 = (int)(Camera.Y - Camera.Origin.Y) / Tile.Size;

            int x2 = (int)(Camera.X + Camera.Origin.X) / Tile.Size;
            int y2 = (int)(Camera.Y + Camera.Origin.Y) / Tile.Size;

            x1--;
            y1--;
            x2++;
            y2++;

            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;
            if (x2 > Width) x2 = Width;
            if (y2 > Height) y2 = Height;

            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    spriteBatch.Draw(Util.Texture, new Rectangle(x * Tile.Size, y * Tile.Size, Tile.Size, Tile.Size), GetTile(x, y).Color);

                }
            }

            foreach (Entity e in Entities)
            {
                e.Draw(spriteBatch);
            }
        }
    }
}
