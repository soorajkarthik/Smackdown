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

        public Dodgeball()
        {

        }

        public Dodgeball(Rectangle r, Vector2 vel)
        {
            rect = r;
            velocity = vel;
        }
    }
}
