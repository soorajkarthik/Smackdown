using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smackdown
{
    class Dodgeball
    {
        private double x, y;
        public Rectangle rect;
        public Vector2 velocity;
        float gravityAcceleration;
        float MaxFallSpeed;
        bool isOnGround;
        Vector2 position;
        private float previousBottom;
        Map map;
        Rectangle localBounds;

        //multipliers for throw. indicates how fast the ball will go in a vector when thrown, per second
        float xMultiplier = 50f;
        float yMultiplier = 5000f;
        
        private static Texture2D texture;

        private Rectangle bounds
        {
            get
            {
                int left = (int)Math.Round(position.X - origin.X) + localBounds.X;
                int top = (int)Math.Round(position.Y - origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        private Vector2 origin
        {
            get
            {
                return new Vector2(texture.Width / 2, texture.Height / 2);
            }
        }

        public Dodgeball()
        {

        }

        public Dodgeball(Rectangle r, Vector2 vel, Map m, Texture2D img)
        {
            MaxFallSpeed = 300f;
            rect = new Rectangle(r.X, r.Y, 40, 40);
            x = rect.X;
            y = rect.Y;
            velocity = vel;
            gravityAcceleration = +50f;
            position = new Vector2((float)x, (float)y);
            texture = img;
            map = m;

            int width = img.Width - 4;
            int left = (img.Width - width) / 2;
            int height = img.Height - 4;
            int top = img.Height - height;

            localBounds = new Rectangle(left, top, width, height);
            
    }

        public void throwBall(Vector2 throwVector)
        {

            velocity.X = throwVector.X * xMultiplier;
            
            velocity.Y = -throwVector.Y * yMultiplier;
            Console.Write(velocity.X + " ");
            Console.WriteLine(velocity.Y);
        }


        public void HandlePhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 previousPosition = position;

            velocity.Y = MathHelper.Clamp(velocity.Y + gravityAcceleration, -MaxFallSpeed * 4f, MaxFallSpeed * 2f) ;
            velocity.X = MathHelper.Clamp(velocity.X, -20, 20);
            if (!isOnGround)
            {
                position.X += velocity.X;
            }

            position += velocity * elapsed;
            position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

            


            HandleCollisions();

            if (position.X == previousPosition.X)
                velocity.X = 0;

            if (position.Y == previousPosition.Y)
                velocity.Y = 0;

            if (position.Y > map.cols * Tile.TILE_SIZE)
            {
                position.Y = 0 - localBounds.Height + 60;
            }

            rect.X = (int)position.X;
            rect.Y = (int)position.Y;
            
        }

        private void HandleCollisions()
        {
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.TILE_SIZE);
            int rightTile = (int)Math.Ceiling((float)bounds.Right / Tile.TILE_SIZE) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.TILE_SIZE);
            int bottomTile = (int)Math.Ceiling((float)bounds.Bottom / Tile.TILE_SIZE) - 1;

            isOnGround = false;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    Tile.CollisionType collision = map.getCollisionAtCoordinates(x, y);
                    if (collision != Tile.CollisionType.Passable)
                    {
                        Rectangle tileBounds = map.getTileBounds(x, y);
                        Vector2 depth = bounds.GetIntersectionDepth(tileBounds);

                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            if (absDepthY < absDepthX || collision == Tile.CollisionType.Platform)
                            {
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                if (collision == Tile.CollisionType.Impassable || isOnGround)
                                {
                                    position = new Vector2(position.X, position.Y + depth.Y);
                                }
                            }
                            else if (collision == Tile.CollisionType.Impassable)
                            {
                                position = new Vector2(position.X + depth.X / 2, position.Y);
                            }
                        }
                    }
                }
            }
            previousBottom = bounds.Bottom;
        }

        public void Update(GameTime gameTime)
        {
            //Once level class is finished, do level.getGravity
            

            //rect.X = (int)x;
            //rect.Y = (int)y;
            //check wall collisions with level here
            //map.checkCollisons(rect);
            HandlePhysics(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, Color.White);
        }    
    }
}
