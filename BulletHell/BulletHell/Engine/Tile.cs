using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    public enum TileType
    {
        Empty,
        Grass
    }

    public struct Tile
    {
        public static int Size = 64;
        public Color Color { get; set; }
        public Color PreviousColor { get; set; }
        public TileType Type { get; set; }
        
        public void SwapColor(Color c)
        {
            PreviousColor = Color;
            Color = c;            
        }

        public bool IsSolid()
        {
            return Color == Color.Black ? true : false;
        }
    }
}
