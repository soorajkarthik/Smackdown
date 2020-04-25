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
    class Player: Sprite
    {
        private string currentAnim = "Idle";
        private SpriteEffects flip;
        private Texture2D ballTex;
        public bool DeadAnimationEnded;

        public bool isAlive;       
        public bool isOnGround;
        private bool isJumping;

        private bool wasJumping;
        private float jumpTime;
        private float previousBottom;

        private GamePadState gps;
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
        private readonly float MoveStickScale = 1.0f;

        private List<Dodgeball> activeBalls = new List<Dodgeball>();

        public Vector2 position;
        public Vector2 velocity;

        private Rectangle localBounds;
        private Rectangle bounds
        {
            get
            {
                int left = (int)Math.Round(position.X - Origin.X) + localBounds.X;
                int top = (int)Math.Round(position.Y - Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        

        public Player(): this(new Vector2(), null, PlayerIndex.One, null, null)
        {

        }

        public Player(Vector2 pos, Texture2D img, PlayerIndex playerIndex, Map m, Texture2D ballT): base(96, 96, 6)
        {
            this.position = pos;
            SpriteSheet = img;
            this.velocity = new Vector2(0, 0);
            this.playerIndex = playerIndex;
            isAlive = true;
            flip = SpriteEffects.None;

            map = m;
            ballTex = ballT;
            gps = GamePad.GetState(playerIndex);

            LoadAnimations();
        }

        private void LoadAnimations()
        {
            Animation anim = new Animation();
            anim.LoadAnimation("Idle", new List<int> { 0, 0, 6, 6, 7, 7, 7, 8, 8, 8}, 3, true);
            SpriteAnimations.Add("Idle", anim);

            anim = new Animation();
            anim.LoadAnimation("Walking", new List<int> { 9, 12, 9, 10 }, 5, true);
            SpriteAnimations.Add("Walking", anim);

            anim = new Animation();
            anim.LoadAnimation("Jump", new List<int> { 2}, 1, true);
            SpriteAnimations.Add("Jump", anim);

            anim = new Animation();
            anim.LoadAnimation("Land", new List<int> { 2, 3 , 3, 3, 3}, 10, false);
            anim.AnimationCallBack(() =>
            {
                currentAnim = "Idle";
                SpriteAnimations[currentAnim].ResetPlay();
            });
            SpriteAnimations.Add("Land", anim);

            anim = new Animation();
            anim.LoadAnimation("Dead", new List<int> { 0, 4, 4, 5, 5, 5, 5, 5, 16, 16, 16}, 3, false);
            anim.AnimationCallBack(() => DeadAnimationEnded = true);
            SpriteAnimations.Add("Dead", anim);

            int width = FrameWidth - 48;
            int left = (FrameWidth - width) / 2;
            int height = FrameHeight - 23;
            int top = FrameHeight - height - 10;

            localBounds = new Rectangle(left, top, width, height);
            SpriteAnimations[currentAnim].ResetPlay();
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

            SpriteAnimations[currentAnim].Update(gameTime);

            //updates player's balls
            for (int i = 0; i < activeBalls.Count; i++)
            {
                activeBalls[i].Update(gameTime);
            }
        }

        private float GetInput()
        {
            GamePadState oldgps = gps;
            gps = GamePad.GetState(playerIndex);
            float horizMovement = gps.ThumbSticks.Left.X * MoveStickScale;
            isJumping =
               gps.IsButtonDown(Buttons.A) ||
               gps.ThumbSticks.Left.Y > 0.5 || 
               gps.IsButtonDown(Buttons.LeftTrigger);

            if (gps.IsButtonDown(Buttons.RightTrigger) && !oldgps.IsButtonDown(Buttons.RightTrigger))
            {
                throwBall(new Vector2(gps.ThumbSticks.Right.X, gps.ThumbSticks.Right.Y));
            } 

            //For falling through platform
            else if (gps.ThumbSticks.Left.Y <= -0.5)
            {
                int xCoord = (int)Math.Floor((float)bounds.Center.X / Tile.TILE_SIZE);
                int feetYCoord = (int)Math.Ceiling((float)bounds.Bottom / Tile.TILE_SIZE) - 1;
                if(map.getCollisionAtCoordinates(xCoord, feetYCoord) == Tile.CollisionType.Platform)
                {
                    position.Y += 1;
                    previousBottom = int.MaxValue;
                }
            }

            if (!isOnGround && currentAnim != "Jump")
            {
                SpriteAnimations[currentAnim].Stop();
                currentAnim = "Jump";
                SpriteAnimations[currentAnim].ResetPlay();
            }

            if(isOnGround &&  currentAnim == "Jump")
            {
                SpriteAnimations[currentAnim].Stop();
                currentAnim = "Land";
                SpriteAnimations[currentAnim].ResetPlay();
            }

            if (horizMovement != 0 && currentAnim != "Jump" && currentAnim != "Walking")
            {
                SpriteAnimations[currentAnim].Stop();
                currentAnim = "Walking";
                SpriteAnimations[currentAnim].ResetPlay();
            }

            else if (currentAnim != "Jump" && horizMovement == 0 && currentAnim == "Walking")
            {
                SpriteAnimations[currentAnim].Stop();
                currentAnim = "Idle";
                SpriteAnimations[currentAnim].ResetPlay();
            }

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

        private void throwBall(Vector2 throwVector)
        {
            //later switch texture by ball type
            activeBalls.Add(new Dodgeball(new Rectangle((int) position.X, (int) position.Y - 20, 40, 40), throwVector, map, ballTex));
            if (activeBalls.Count > 0)
            {
                activeBalls[activeBalls.Count - 1].throwBall(throwVector);
            }
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
            Rectangle source = GetFrameRectangle(SpriteAnimations[currentAnim].FrameToDraw);        

            spriteBatch.Draw(SpriteSheet, position, source, Color.White, 0.0f, Origin, 1.0f, flip, 0.0f);
            for(int i = 0; i < activeBalls.Count; i++)
            {
                
                activeBalls[i].Draw(spriteBatch);
            }
        }
    }
}
