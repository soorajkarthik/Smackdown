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
        public Vector2 position;
        public Texture2D img; //replace with animation later
        private SpriteEffects flip;
        public Vector2 velocity;
        public bool isAlive;
        public bool isOnGround;       
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        public PlayerIndex playerIndex;
        
        private readonly float MoveAcceleration = 14000f;
        private readonly float MaxMoveSpeed = 2000f;
        private readonly float GroundDragFactor = 0.58f;
        private readonly float AirDragFactor = 0.65f;

        private readonly float MaxJumpTime = 0.35f;
        private readonly float JumpLauchVelocity = -4000.0f;
        private readonly float GravityAcceleration = 7000.0f;
        private readonly float MaxFallSpeed = 1000.0f;
        private readonly float JumpControlPower = 0.14f;

        private const float MoveStickScale = 1.0f;

        public Player(): this(new Vector2(), null, PlayerIndex.One)
        {

        }

        public Player(Vector2 pos, Texture2D img, PlayerIndex playerIndex)
        {
            this.position = pos;
            this.img = img;
            this.velocity = new Vector2(0, 0);
            this.playerIndex = playerIndex;
            isAlive = true;
            flip = SpriteEffects.None;
        }

        public void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                float horizMovement = GetInput();

                HandlePhysics(gameTime, horizMovement);

                //temporary code
                if (position.Y >= 200)
                {
                    position.Y = 200;
                    isOnGround = true;
                }
                else
                {
                    isOnGround = false;
                }
                //end of temporary code

                velocity = new Vector2(0, 0);
                isJumping = false;
            }
        }

        private float GetInput()
        {
            GamePadState gps = GamePad.GetState(playerIndex);
            float horizMovement = gps.ThumbSticks.Left.X * MoveStickScale;
            isJumping =
               gps.IsButtonDown(Buttons.A) ||
               gps.ThumbSticks.Left.Y > 0.5;

            return horizMovement;
        }

        public void HandlePhysics(GameTime gameTime, float horizMovement)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 previousPosition = position;

            velocity.X += horizMovement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);

            if (isOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
           
            position += velocity * elapsed;
            position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

            HandleCollisions();

            if (position.X == previousPosition.X)
                velocity.X = 0;

            if (position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        private float DoJump(float yVel, GameTime gameTime)
        {
            if (isJumping)
            {
                if ((!wasJumping && isOnGround) || jumpTime > 0f)
                {
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    yVel = JumpLauchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    jumpTime = 0f;
                }
            }
            else
            {
                jumpTime = 0f;
            }

            wasJumping = isJumping;

            return yVel;
        }

        private void HandleCollisions()
        {

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else
                flip = SpriteEffects.None;
            
            spriteBatch.Draw(img, position, null, Color.White, 0.0f, new Vector2(img.Width/2, img.Height/2), 1.0f, flip, 0.0f);
        }
    }
}
