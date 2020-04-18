using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smackdown
{
    class Animation
    {
        public string AnimationName
        {
            get;
            private set;
        }

        public int TextureId
        {
            get;
            private set;
        }

        public int FrameToDraw
        {
            get { return frameList[currentFrame]; }
        }

        public delegate void AnimationEndCallback();
        private AnimationEndCallback callBack;
        private List<int> frameList;
        private float totalElapsed;
        private float timePerFrame;
        private int currentFrame;
        private int frameCount;
        private int framesPerSec;
        private bool isPlaying;
        private bool loopAnimation;

        public void LoadAnimation(string _animationName, int _textureId, List<int> _frameList, int _framesPerSec, bool _loop)
        {
            AnimationName = _animationName;
            TextureId = _textureId;
            frameList = _frameList;
            frameCount = frameList.Count;
            framesPerSec = _framesPerSec;
            timePerFrame = (float)1 / framesPerSec;
            currentFrame = 0;
            totalElapsed = 0;
            loopAnimation = _loop;
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void ResetPlay()
        {
            currentFrame = 0;
            totalElapsed = 0;
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
        }

        public void AnimationCallBack(AnimationEndCallback _callback)
        {
            callBack = _callback;
        }

        public void Update(GameTime _gameTime)
        {
            if (!isPlaying) return;
            totalElapsed += (float)_gameTime.ElapsedGameTime.TotalSeconds;
            if (totalElapsed <= timePerFrame) return;
            currentFrame++;
            currentFrame = currentFrame % frameCount;
            totalElapsed -= timePerFrame;
            if (!loopAnimation && currentFrame == 0)
            {
                isPlaying = false;
                if (callBack != null)
                    callBack();
            }
        }
    }
}
