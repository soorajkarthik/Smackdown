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
            PassableFromBottom,
            Impassable
        }

        public readonly int TILE_SIZE = 64;

        public Rectangle loc;
        Texture2D img;

        public CollisionType collisionType;

        public Tile() : this(0, 0, null)
        {
        }

        public Tile(Rectangle loc, Texture2D img): this(loc.X, loc.Y, img, CollisionType.Passable)
        {
        }

        public Tile(int x, int y, Texture2D img) : this(x, y, img, CollisionType.Passable)
        {
        }

        public Tile(Rectangle loc, Texture2D img, CollisionType collisionType) : this(loc.X, loc.Y, img, collisionType)
        {
        }

        public Tile (int x, int y, Texture2D img, CollisionType collisionType)
        {
            loc = new Rectangle(x, y, TILE_SIZE, TILE_SIZE);
            this.img = img;
            this.collisionType = collisionType;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(img, loc, Color.White);
        }
    }
}
