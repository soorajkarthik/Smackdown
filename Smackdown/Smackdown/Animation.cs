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

        public void LoadAnimation(string name, List<int> frames, int fps, bool loop)
        {
            AnimationName = name;
            frameList = frames;
            frameCount = frameList.Count;
            framesPerSec = fps;
            timePerFrame = (float)1 / framesPerSec;
            currentFrame = 0;
            totalElapsed = 0;
            loopAnimation = loop;
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

        public void AnimationCallBack(AnimationEndCallback callback)
        {
            this.callBack = callback;
        }

        public void Update(GameTime gameTime)
        {
            if (!isPlaying) return;
            totalElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (totalElapsed <= timePerFrame) return;
            currentFrame++;
            currentFrame = currentFrame % frameCount;
            totalElapsed -= timePerFrame;
            if (!loopAnimation && currentFrame == 0)
            {
                isPlaying = false;
                callBack?.Invoke();
            }
        }
    }
}
