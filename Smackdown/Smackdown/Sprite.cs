using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Smackdown
{
    class Sprite
    {
        public int FrameWidth;
        public int FrameHeight;
        private int FramesPerRow;

        public Texture2D SpriteSheet { get; set; }
        public Dictionary<string, Animation> SpriteAnimations;

        public Sprite(int width, int height, int framesPerRow)
        {
            FrameWidth = width;
            FrameHeight = height;
            FramesPerRow = framesPerRow;
            SpriteSheet = null;
            SpriteAnimations = new Dictionary<string, Animation>();
        }

        public Vector2 Origin
        {
            get { return new Vector2(FrameWidth / 2.0f, FrameHeight / 2.0f); }
        }
        public Rectangle GetFrameRectangle(int frameNumber)
        {
            return new Rectangle(
            (frameNumber % FramesPerRow) * FrameWidth,
            (frameNumber / FramesPerRow) * FrameHeight,
            FrameWidth,
            FrameHeight);
        }
    }
}
