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
        public Rectangle loc;
        public Texture2D img;
        public Vector2 velocity;

        public PlayerIndex playerIndex;

        public Player(): this(new Rectangle(), null, new Vector2(), PlayerIndex.One)
        {

        }

        public Player(Rectangle loc, Texture2D img, Vector2 velocity, PlayerIndex playerIndex)
        {
            this.loc = loc;
            this.img = img;
            this.velocity = velocity;
            this.playerIndex = playerIndex;
        }

        public void Update()
        {
            GamePadState pad = GamePad.GetState(playerIndex);

            loc.X += (int) velocity.X;
            loc.Y += (int) velocity.Y;

            //vy -= GRAVITY;

            //check level collision
        }

        public void Draw(SpriteBatch batch)
        {

        }
    }
}
