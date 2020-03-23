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
    class Player
    {
        Rectangle loc;
        Texture2D img;
        int vx, vy;

        public Player(): this(new Rectangle(), null, 0, 0)
        {

        }

        public Player(Rectangle loc, Texture2D img, int vx, int vy)
        {
            this.loc = loc;
            this.img = img;
            this.vx = vx;
            this.vy = vy;
        }

        public void Update(GamePadState gamePad)
        {
            loc.X += vx;
            loc.Y += vy;

            //vy -= GRAVITY;

            //check level collision
        }

        public void Draw()
        {

        }
    }
}
