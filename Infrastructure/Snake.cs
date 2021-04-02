using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.GameObjects;

namespace MonoSnake.Infrastructure
{
    public class Snake
    {
        private readonly SnakeHead _snakeHead;
        private readonly List<SnakeSegment> _snakeSegments = new List<SnakeSegment>();
        private int _segmentCount = 1;
        private Sprite _snakeTailSprite;

        public Snake(SnakeHead snakeHead, Sprite snakeTailSprite)
        {
            _snakeHead = snakeHead;
            _snakeTailSprite = snakeTailSprite;
        }

        private void Eat()
        {
            _segmentCount++;
        }

        private void AddSegment()
        {
            _snakeSegments.Add(new SnakeSegment(_snakeTailSprite, new Vector2(21, 21)));
        }

        public void Update(GameTime gameTime)
        {
            if (_snakeSegments.Count < _segmentCount)
            {
                AddSegment();
            }

            // See if SnakeHead is ready to be moved again
            if(!_snakeHead.CanUpdate())
                return;

            Vector2 previousSnakeHeadPosition = _snakeHead.Position;
            // Move Snake Head
            _snakeHead.Update(gameTime);
            Vector2 previousSegmentPosition = Vector2.Zero;
            // Move Segments
            foreach (SnakeSegment snakeSegment in _snakeSegments)
            {
                int currentIndex = _snakeSegments.IndexOf(snakeSegment);

                if (currentIndex == 0)
                {
                    snakeSegment.PreviousSnakeSegmentPosition = previousSnakeHeadPosition;
                    previousSegmentPosition = snakeSegment.PreviousSnakeSegmentPosition;
                }
                else
                {
                    var newPosition = previousSegmentPosition;
                    previousSegmentPosition = snakeSegment.Position;
                    snakeSegment.PreviousSnakeSegmentPosition = newPosition;
                }

                snakeSegment.Sprite.Rotation = (float)(90 * Math.PI / 180);
                snakeSegment.Update(gameTime);
            }

            if(_segmentCount < 9)
                _segmentCount++;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _snakeHead.Draw(spriteBatch, gameTime);
            foreach (var s in _snakeSegments)
                s.Draw(spriteBatch, gameTime);
        }
    }
}
