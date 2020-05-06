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
        public int rows, cols;

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
                            throw new Exception("file x dimensions are not correct " + line.Length + " vs." + tileArray.GetLength(0) + "on line " + y);
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
                case ".":
                default:
                    return new Rectangle();
                //ordinary
                case "1":
                    return new Rectangle(0, 80, 16, 15);
                //empty box
                case "2":
                    return new Rectangle(48, 96, 16, 16);
                //full box
                case "3":
                    return new Rectangle(80, 96, 16, 16);
                //platform
                case "4":
                    return new Rectangle(64, 80, 16, 15);
                //platform left end
                case "5":
                    return new Rectangle(48, 80, 16, 15);
                //platform right end
                case "6":
                    return new Rectangle(80, 80, 16, 15);
                //spike facing up
                case "7":
                    return new Rectangle(32, 97, 15, 15);
                //spike facing down
                case "8":
                    return new Rectangle(49, 128, 15, 15);
                //spike facing left
                case "9":
                    return new Rectangle(32, 128, 16, 15);
                //spike facing right
                case "10":
                    return new Rectangle(16, 128, 16, 16);
            }
        }

        public Tile.CollisionType getCollisionType(string tileType)
        {
            switch (tileType)
            {
                //"nothing"
                case ".":
                default:
                    return Tile.CollisionType.Passable;
                //ordinary
                case "1":
                //boxes
                case "2":
                case "3":
                    return Tile.CollisionType.Impassable;
                //platform
                case "4":
                case "5":
                case "6":
                    return Tile.CollisionType.Platform;
                //spikes
                case "7": //up
                case "8": //down
                case "9": //left
                case "10": //right
                    return Tile.CollisionType.Spikes;
            }
        }

        public Tile.CollisionType getCollisionAtCoordinates(int x, int y)
        {
            if (x < 0 || x >= rows || y < 0 || y >= cols)
                return Tile.CollisionType.Passable;
            
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
