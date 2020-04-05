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
            Passable = 0,
            PassableFromBottom = 1,
            Impassable = 2
        }

        public readonly int TILE_SIZE = 48;

        public Rectangle loc;
        Rectangle imgSource;

        public CollisionType collisionType;

        //a bunch of constructors bc idk y
        public Tile() : this(0, 0, new Rectangle())
        {
        }

        public Tile(Rectangle loc, Rectangle imgSource): this(loc.X, loc.Y, imgSource, CollisionType.Passable)
        {
        }

        public Tile(int x_coord, int y_coord, Rectangle imgSource) : this(x_coord, y_coord, imgSource, CollisionType.Passable)
        {
        }

        public Tile(Rectangle loc, Rectangle imgSource, CollisionType collisionType) : this(loc.X, loc.Y, imgSource, collisionType)
        {
        }
        
        //actual constructor
        public Tile (int x_coord, int y_coord, Rectangle imgSource, CollisionType collisionType)
        {
            loc = new Rectangle(x_coord * TILE_SIZE, y_coord * TILE_SIZE, TILE_SIZE, TILE_SIZE);
            this.imgSource = imgSource;
            this.collisionType = collisionType;
        }

        public void Draw(SpriteBatch batch, Texture2D img)
        {
            batch.Draw(img, loc, imgSource, Color.White);
        }
    }
}
