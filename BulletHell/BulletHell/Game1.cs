using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using BulletHell.Engine;

namespace BulletHell
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldKeyboardState, keyboardState;
        MouseState oldmouseState, mouseState;

        GameMode mode = GameMode.Menu;
        Texture2D titleTexture;
        Texture2D endTexture;

        StringBuilder sb = new StringBuilder();

        Level level;
        int levelNumber = 1;
        int width = 20;
        int height = 11;

        Player player;
        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Util.Initialize(this);

            titleTexture = Content.Load<Texture2D>("title");
            endTexture = Content.Load<Texture2D>("endtitle");

            player = new Player(Content.Load<Texture2D>("Octocat"));

            NewLevel(width, height, 3);

            base.Initialize();
        }

        public void NewLevel(int width, int height, int enemies)
        {
            level = new Level(width, height, levelNumber + 2);
            level.AddEntity(player);
            level.Player = player;
            player.Health++;
            player.Position = new Vector2(3 * Tile.Size + (player.Width / 2), 3 * Tile.Size - (player.Height / 2) + Tile.Size / 2);
            player.Bullets.Clear();
            camera = new Camera(this);
            camera.Focus = player;
            camera.Bounds = new Rectangle(0, 0, level.Width * Tile.Size, level.Height * Tile.Size);

            level.Camera = camera;
            level.Initialize();
            for (int y = 3; y < 5; y++)
            {
                for (int x = 3; x < 5; x++)
                {
                    level.Tiles[x + y * width].Color = Color.SaddleBrown;
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            oldmouseState = mouseState;
            mouseState =Mouse.GetState();

            switch (mode)
            {
                case GameMode.Menu:
                    if (keyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape))
                        this.Exit();
                    if (keyboardState.GetPressedKeys().Length > 0 && keyboardState.IsKeyUp(Keys.Escape))
                        mode = GameMode.Gameplay;
                    break;
                case GameMode.Gameplay:
                    if (keyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape))
                    {
                        mode = GameMode.Menu;
                        break;
                    }
                        
                    player.Velocity = Vector2.Zero;

                    if (keyboardState.IsKeyDown(Keys.Space) && oldKeyboardState.IsKeyUp(Keys.Space))
                    {
                        for (int i = 0; i < level.Tiles.Length; i++)
                        {
                            if (level.Tiles[i].Color == Color.Black)
                            {
                                level.Tiles[i].Color = Color.SaddleBrown;
                            }
                            else
                            {
                                level.Tiles[i].Color = Color.Black;
                            }
                        }
                    }

                    if (keyboardState.IsKeyDown(Keys.A))
                    {
                        player.Velocity = new Vector2(-300, player.Velocity.Y);
                    }
                    else if (keyboardState.IsKeyDown(Keys.D))
                    {
                        player.Velocity = new Vector2(300, player.Velocity.Y);
                    }
                    if (keyboardState.IsKeyDown(Keys.W))
                    {
                        player.Velocity = new Vector2(player.Velocity.X, -300);
                    }
                    else if (keyboardState.IsKeyDown(Keys.S))
                    {
                        player.Velocity = new Vector2(player.Velocity.X, 300);
                    }
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        player.Shoot(Direction.Left);
                    }
                    if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        player.Shoot(Direction.Right);
                    }
                    if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        player.Shoot(Direction.Up);
                    }
                    if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        player.Shoot(Direction.Down);
                    }

                    level.Update(elapsed);
                    if (level.GoToNextLevel)
                    {
                        width += 2;
                        height += 2;
                        levelNumber++;
                        if (width > 40) width = 40;
                        if (height > 35) height = 35;
                        NewLevel(width, height, levelNumber + 2);
                    }
                    if (player.Health <= 0)
                    {
                        mode = GameMode.End;
                    }
                    break;
                case GameMode.End:
                    if (keyboardState.GetPressedKeys().Length > 0 && keyboardState.IsKeyUp(Keys.Escape))
                    {
                        mode = GameMode.Gameplay;
                        width = 20;
                        height = 11;
                        levelNumber = 1;
                        NewLevel(width, height, 3);
                    }
                    break;
                default:
                    break;
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Begin();
            

            switch (mode)
            {
                case GameMode.Menu:
                    spriteBatch.Begin();
                    spriteBatch.Draw(titleTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.End();
                    break;
                case GameMode.Gameplay:
                    spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, camera.Transform);
                    level.Draw(spriteBatch);
                    spriteBatch.End();
                    // HUD
                    spriteBatch.Begin();
                    sb.Clear();
                    sb.AppendLine("Level: " + levelNumber);
                    if (level.NumberOfEnemies <= 0)
                    {
                        sb.AppendLine("No more enemies. Find the RED EXIT!");
                    }
                    else
                    {
                        sb.AppendLine("Enemies left: " + level.NumberOfEnemies);
                    }
                    sb.AppendLine("Health: " + player.Health);
                    spriteBatch.DrawString(Util.Font, sb, Vector2.Zero, Color.Purple);
                    spriteBatch.End();
                    break;
                case GameMode.End:
                    spriteBatch.Begin();
                    sb.Clear();
                    sb.AppendLine("You reached level " + levelNumber);
                    sb.AppendLine("You killed " + player.EnemiesKilled + " enemies");
                    spriteBatch.Draw(endTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Util.EndFont, sb, new Vector2(150, 300), Color.Gray);
                    spriteBatch.End();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
