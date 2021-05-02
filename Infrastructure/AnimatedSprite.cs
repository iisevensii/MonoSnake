using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class AnimatedSprite : ISprite
    {
        public class AnimatedSpriteFrame
        {
            public ISprite Sprite { get; set; }
            public float FrameDisplayLength { get; set; }

            public AnimatedSpriteFrame(ISprite sprite, float frameDisplayLength)
            {
                Sprite = sprite;
                FrameDisplayLength = frameDisplayLength;
            }
        }

        private readonly List<AnimatedSpriteFrame> _frames = new List<AnimatedSpriteFrame>();
        private ISprite _currentFrame;
        private int _currentFrameIndex = 0;
        private double _timeElapsed = 0f;

        public bool Loop { get; set; } = true;

        #region ISprite

        public Texture2D SpriteSheet { get; }
        public int Top { get; }
        public int Left { get; }
        public int Width { get; }
        public int Height { get; }
        public Color TintColor { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }

        #endregion ISprite

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

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            this.Draw(spriteBatch, position, Rotation);
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
