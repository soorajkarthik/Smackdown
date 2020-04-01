using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private static Texture2D tempTexture;

        public Dodgeball()
        {

        }

        public Dodgeball(Rectangle r, Vector2 vel)
        {
            rect = r;
            x = rect.X;
            y = rect.Y;
            velocity = vel;
        }

        public void Update(Map map)
        {
            //Once level class is finished, do level.getGravity
            velocity.Y += 2;
            x += velocity.X;
            y += velocity.Y;

            rect.X = (int)x;
            rect.Y = (int)y;
            //check wall collisions with level here
            //map.checkCollisons(rect);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tempTexture, rect, Color.Blue);
        }    
    }
}
