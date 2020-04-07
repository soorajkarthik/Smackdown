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
using System.IO;

namespace Smackdown
{
    class Map
    {
        public Tile[,] tileArray;
        public Texture2D spriteSheet;
        private int rows, cols;

        public Map() : this(1, 1)
        {
        }

        public Map(int rows, int cols) : this (rows, cols, null)
        {
        }

        public Map(int rows, int cols, Texture2D spriteSheet)
        {
            this.tileArray = new Tile[rows, cols];
            this.spriteSheet = spriteSheet;
            this.rows = rows;
            this.cols = cols;
        }

        public void loadMap(string filepath)
        {
            try
            {
                int y = 0;
                using (StreamReader reader = new StreamReader(filepath))
                {
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(' ');
                        if (line.Length != tileArray.GetLength(0))
                        {
                            throw new Exception("file x dimensions are not correct");
                        }
                        for (int x = 0; x < line.Length; x++)
                        {
                            tileArray[x, y] = new Tile(x, y, getImageSourceRect(line[x]), getCollisionType(line[x]));
                        }
                        y++;
                    }
                }
                if (y != tileArray.GetLength(1))
                {
                    throw new Exception("file y dimensions are not correct " + y + " vs. " + tileArray.GetLength(1));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read.");
                Console.WriteLine(e.Message);
            }
        }

        public Rectangle getImageSourceRect(string tileType)
        {
            //TODO: add code to get img based on number in text file
            switch (tileType)
            {
                //"nothing"
                case "0":
                default:
                    return new Rectangle();
                //ordinary
                case "1":
                    return new Rectangle(0, 80, 16, 15);
                //platform
                case "2":
                    return new Rectangle(64, 80, 16, 15);
                //platform left end
                case "3":
                    return new Rectangle(48, 80, 16, 15);
                //platform right end
                case "4":
                    return new Rectangle(80, 80, 16, 15);

            }
        }

        public Tile.CollisionType getCollisionType(string tileType)
        {
            switch (tileType)
            {
                //"nothing"
                case "0":
                default:
                    return Tile.CollisionType.Passable;
                //ordinary
                case "1":
                    return Tile.CollisionType.Impassable;
                //platform
                case "2":
                case "3":
                case "4":
                    return Tile.CollisionType.PassableFromBottom;
            }
        }

        public Tile.CollisionType getCollisionAtCoordinates(int x, int y)
        {
            if (x < 0 || x >= rows || y < 0 || y >= cols)
                return Tile.CollisionType.Impassable;
            
            return tileArray[x, y].collisionType;
        }

        public Rectangle getTileBounds(int x, int y)
        {
            
            if (x < 0 || y < 0 || x >= rows || y >= cols)
                return new Rectangle(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE, Tile.TILE_SIZE, Tile.TILE_SIZE);

            return new Rectangle(x * Tile.TILE_SIZE, (y * Tile.TILE_SIZE) + 5, Tile.TILE_SIZE, Tile.TILE_SIZE - 5);
        
    }

        //check collision of this rect to all tiles and return a new rect that accounts for and deals with collisions 
        //(i.e move parameter rect away from collision and return)
        public Rectangle checkCollisions(Rectangle rect)
        {
            //TODO: implement this
            return rect;
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (Tile tile in tileArray)
            {
                if (tile == null)
                {
                    Console.WriteLine("found null tile in arr");
                }
                if (tile.collisionType != Tile.CollisionType.Passable)
                {
                    tile.Draw(batch, spriteSheet);
                }
            }
        }
    }
}
