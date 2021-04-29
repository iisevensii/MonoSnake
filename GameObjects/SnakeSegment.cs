using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    public class SnakeSegment : IGameObject
    {

        public int DrawOrder => 0;
        public ISprite Sprite { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public SnakeDirection Direction { get; set; }
        public bool NoRotation { get; set; }

        public SnakeSegment(Sprite sprite, Vector2 position, float rotation, SnakeDirection direction)
        {
            Sprite = sprite;
            Position = position;
            Rotation = rotation;
            Direction = direction;
        }

        public void Update(GameTime gameTime)
        {
            Rotation = NoRotation ? 0f : Rotation;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
