using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class AnimatedSprite
    {
        public class AnimatedSpriteFrame
        {
            public Sprite Sprite { get; set; }
            public float FrameDisplayLength { get; set; }

            public AnimatedSpriteFrame(Sprite sprite, float frameDisplayLength)
            {
                Sprite = sprite;
                FrameDisplayLength = frameDisplayLength;
            }
        }

        private List<AnimatedSpriteFrame> _frames = new List<AnimatedSpriteFrame>();
        private Sprite _currentFrame;
        private int _currentFrameIndex = 0;
        private double _timeElapsed = 0f;

        public Sprite Sprite => _currentFrame;

        public bool Loop { get; set; } = true;

        public AnimatedSprite(List<AnimatedSpriteFrame> animatedSpriteFrames, int top, int left, int width, int height)
        {
            if (animatedSpriteFrames.Count == 0)
                throw new ArgumentException("At least one animation frame is required", nameof(animatedSpriteFrames));

            _frames = animatedSpriteFrames;
            _currentFrame = animatedSpriteFrames.FirstOrDefault()?.Sprite;
        }

        public void Update(GameTime gameTime)
        {
            _timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;

            bool onLastFrame = _currentFrameIndex >= _frames.Count - 1;

            bool frameCompleted = _timeElapsed >= _frames[_currentFrameIndex].FrameDisplayLength;

            if (!frameCompleted)
                return;

            _timeElapsed = 0f;

            if (!onLastFrame)
                _currentFrameIndex++;
            else
                if (Loop)
                    _currentFrameIndex = 0;

            _currentFrame = _frames[_currentFrameIndex].Sprite;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            spriteBatch.Draw
            (
                _currentFrame.SpriteSheet,
                position,
                new Rectangle(_frames[_currentFrameIndex].Sprite.Left, _frames[_currentFrameIndex].Sprite.Top, _frames[_currentFrameIndex].Sprite.Width, _frames[_currentFrameIndex].Sprite.Height),
                _currentFrame.TintColor,
                rotation,
                _currentFrame.Origin,
                _currentFrame.Scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
