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
        private int _segmentCount;
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
            _snakeSegments.Add(new SnakeSegment(_snakeTailSprite, Vector2.Zero));
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

            // Move Snake Head
            _snakeHead.Update(gameTime);

            // Move Segments
            foreach (SnakeSegment snakeSegment in _snakeSegments)
            {
                snakeSegment.PreviousSnakeSegmentPosition = _snakeHead.Position;
                snakeSegment.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _snakeHead.Draw(spriteBatch, gameTime);
            _snakeSegments.ForEach(s => s.Draw(spriteBatch, gameTime));
        }
    }
}
