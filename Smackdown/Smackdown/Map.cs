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

        public Map() : this(1, 1)
        {

        }

        public Map(int rows, int cols)
        {
            this.tileArray = new Tile[rows, cols];
        }

        public void loadMap(string filepath)
        {
            try
            {
                int y = 0;
                using (StreamReader reader = new StreamReader(filepath))
                {
                    string[] line = reader.ReadLine().Split(' ');
                    for (int x = 0; x < line.Length; x++)
                    {
                        tileArray[x, y] = new Tile(x, y, getImage(line[x]), getCollisionType(line[x]));
                    }
                    y++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read.");
                Console.WriteLine(e.Message);
            }
        }

        public Texture2D getImage(string tileType)
        {
            //TODO: add code to get img based on number in text file
            switch (tileType)
            {
                case "0":
                    return 
                    break;
            }
            return null;
        }

        public Tile.CollisionType getCollisionType(string tileType) {
            //TODO: add code to get collision type based on number of text file
            return Tile.CollisionType.Passable;
        }

        public Rectangle checkCollisions(Rectangle rect)
        {
            //TODO: implement this
            return rect;
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (Tile tile in tileArray) {
                tile.Draw(batch);
            }
        }
    }
}
