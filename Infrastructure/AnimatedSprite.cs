using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class AnimatedSprite : Sprite
    {
        public class AnimatedSpriteFrame
        {
            public Texture2D Texture2D { get; set; }
            public float FrameDisplayLength { get; set; }
        }

        private List<AnimatedSpriteFrame> _frames = new List<AnimatedSpriteFrame>();
        private Texture2D _currentFrame;
        private int _currentFrameIndex = 0;
        private double _timeElapsed = 0f;

        public bool Loop { get; set; }

        public AnimatedSprite(List<AnimatedSpriteFrame> spriteSheets, int top, int left, int width, int height)
            : base(spriteSheets.FirstOrDefault()?.Texture2D, top, left, width, height)
        {
            _currentFrame = spriteSheets.FirstOrDefault()?.Texture2D;
        }

        public AnimatedSprite(Texture2D spriteSheet, int top, int left, int width, int height)
            : base(spriteSheet, top, left, width, height)
        {
            _currentFrame = spriteSheet;
        }

        public AnimatedSprite(List<PositionedTexture2D> positionedTexture2D, int width, int height)
            : base(positionedTexture2D.FirstOrDefault(), width, height)
        {
            _currentFrame = positionedTexture2D.FirstOrDefault()?.SpriteSheet;
        }

        public void Update(GameTime gameTime)
        {
            _timeElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;

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
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            spriteBatch.Draw
            (
                _currentFrame,
                position,
                new Rectangle(Left, Top, Width, Height),
                TintColor,
                rotation,
                Origin,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
