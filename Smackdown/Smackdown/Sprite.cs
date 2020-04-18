using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Smackdown
{
    class Sprite
    {
        public List<Texture2D> SpriteTextures { get; set; }
        public int FrameWidth;
        public int FrameHeight;
        private int framesPerRow;
        public Dictionary<string, Animation> SpriteAnimations;

        public Sprite(int _frameWidth, int _frameHeight, int _framesPerRow)
        {
            FrameWidth = _frameWidth;
            FrameHeight = _frameHeight;
            framesPerRow = _framesPerRow;
            SpriteTextures = new List<Texture2D>();
            SpriteAnimations = new Dictionary<string, Animation>();
        }

        public Vector2 Origin
        {
            get { return new Vector2(FrameWidth / 2.0f, FrameHeight / 2.0f); }
        }
        public Rectangle GetFrameRectangle(int _frameNumber)
        {
            return new Rectangle(
            (_frameNumber % framesPerRow) * FrameWidth,
            (_frameNumber / framesPerRow) * FrameHeight,
            FrameWidth,
            FrameHeight);
        }
    }
}
