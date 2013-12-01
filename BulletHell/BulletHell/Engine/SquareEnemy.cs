using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    class SquareEnemy : Enemy
    {
        public SquareEnemy()
            : base(Util.Texture)
        {
            Width = 32;
            Height = 32;

            XOffset = 0;
            YOffset = 0;

            CanFly = true;

            Speed = Util.Next(50, 100);
        }

        public override void Update(float elapsed)
        {
            base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle drawRect = DrawRectangle;

            spriteBatch.Draw(Texture, new Rectangle((int)position.X - XOffset, (int)position.Y - YOffset, Width, Height), Color);

            base.Draw(spriteBatch);
        }
    }
}
