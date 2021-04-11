using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.UI
{
    interface IUiObject
    {
        int DrawOrder { get; }
        Sprite Sprite { get; }
        Vector2 Position { get; set; }
        public float Rotation { get; set; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
