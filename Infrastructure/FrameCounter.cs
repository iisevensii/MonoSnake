using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class FrameCounter
    {
        private SpriteFont _spriteFont;
        private int _screenWidth;
        private int _screenHeight;
        private Vector2 _position;
        private Color _color;
        private Vector2 _scale;

        public FrameCounter(SpriteFont spriteFont, int screenWidth, int screenHeight, Color color, Vector2 scale)
        {
            _spriteFont = spriteFont;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _color = color;
            _scale = scale;
        }

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }
        public string FpsString => $"FPS    {CurrentFramesPerSecond}";

        public const int MAXIMUM_SAMPLES = 100;

        private readonly Queue<float> _sampleBuffer = new Queue<float>();

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaTime = deltaTime == 0f ? 1f : deltaTime;
            CurrentFramesPerSecond = (float) Math.Round(1.0f / deltaTime);
            Vector2 cFpsScale = _spriteFont.MeasureString(FpsString);
            _position = new Vector2(_screenWidth - cFpsScale.X * 2 - 20,  cFpsScale.Y /2);

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString
            (
                _spriteFont,
                FpsString,
                _position,
                _color,
                0f,
                Vector2.Zero,
                _scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
