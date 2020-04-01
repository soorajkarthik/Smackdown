using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Smackdown
{
    class Tile
    {
        public enum CollisionType
        {
            Passable,
            Impassable
        }

        public readonly int TILE_SIZE = 64;

        public Rectangle loc;
        Texture2D img;

        public CollisionType collisionType;

        public Tile() : this(new Rectangle(), null)
        {

        }

        public Tile(Rectangle loc, Texture2D img)
        {
            this.loc = loc;
            this.img = img;
        }

        public void Draw(SpriteBatch batch)
        {
            
        }
    }
}
