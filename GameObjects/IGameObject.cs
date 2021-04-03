using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    interface IGameObject
    {
        int DrawOrder { get; }
        Sprite Sprite { get; }

        Vector2 Position { get; set; }
        float Rotation { get; set; }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
