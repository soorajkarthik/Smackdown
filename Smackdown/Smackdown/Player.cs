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
        private float previousBottom;

        public PlayerIndex playerIndex;
        public Map map;

        private readonly float MoveAcceleration = 30000f;
        private readonly float MaxMoveSpeed = 2000f;
        private readonly float GroundDragFactor = 0.58f;
        private readonly float AirDragFactor = 0.65f;

        private readonly float MaxJumpTime = 0.35f;
        private readonly float JumpLauchVelocity = -5000.0f;
        private readonly float GravityAcceleration = 21000f;
        private readonly float MaxFallSpeed = 30000f;
        private readonly float JumpControlPower = 0.14f;

        private const float MoveStickScale = 1.0f;

        private Vector2 origin
        {
            get
            {
                return new Vector2(img.Width / 2, img.Height / 2);
            }
        }

        private Rectangle localBounds;

        private Rectangle bounds
        {
            get
            {
                int left = (int)Math.Round(position.X - origin.X) + localBounds.X;
                int top = (int)Math.Round(position.Y - origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public Player(): this(new Vector2(), null, PlayerIndex.One, null)
        {

        }

        public Player(Vector2 pos, Texture2D img, PlayerIndex playerIndex, Map m)
        {
            this.position = pos;
            this.img = img;
            this.velocity = new Vector2(0, 0);
            this.playerIndex = playerIndex;
            isAlive = true;
            flip = SpriteEffects.None;

            int width = img.Width - 4;
            int left = (img.Width - width) / 2;
            int height = img.Height - 4;
            int top = img.Height - height;

            localBounds = new Rectangle(left, top, width, height);
            map = m;
        }

        public void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                float horizMovement = GetInput();
                HandlePhysics(gameTime, horizMovement);

                if (velocity.X > 0)
                    flip = SpriteEffects.FlipHorizontally;
                else if (velocity.X < 0)
                    flip = SpriteEffects.None;

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

            if (position.Y > map.cols * Tile.TILE_SIZE)
            {
                position.Y = 0 - localBounds.Height + 60;
            }
            //if (position.X > map.rows * Tile.TILE_SIZE)
            //{
            //    position.X = 0 - localBounds.Width + 60;
            //}
            //if (position.X + localBounds.Width < 0)
            //{
            //    position.X = map.rows * Tile.TILE_SIZE - 40;
            //}
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

                            if (absDepthY < absDepthX || collision == Tile.CollisionType.PassableFromBottom)
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
                                position = new Vector2(position.X + depth.X/2, position.Y);
                            }
                        }
                    }
                }
            }
            previousBottom = bounds.Bottom;
        }


        public void Draw(SpriteBatch spriteBatch)
        {           
            spriteBatch.Draw(img, position, null, Color.White, 0.0f, new Vector2(img.Width/2, img.Height/2), 1.0f, flip, 0.0f);
        }
    }
}
