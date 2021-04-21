using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class TextEntry
    {
        private readonly Texture2D _letterEntryTexture2DFrame0;
        private readonly Texture2D _letterEntryTexture2DFrame1;
        private readonly ISprite _letterEntrySpriteFrame0;
        private readonly ISprite _letterEntrySpriteFrame1;
        private readonly ISprite _letterEntryAnimatedSprite;
        private readonly Vector2 _position;

        public TextEntry(GraphicsDevice graphicsDevice, Vector2 position)
        {
            _position = position;
            // Test High Score Entry Cursor (Static)
            if (_letterEntryTexture2DFrame0 == null)
            {
                _letterEntryTexture2DFrame0 = new Texture2D(graphicsDevice, 1, 1);
                _letterEntryTexture2DFrame0.SetData(new[] { Color.White });
            }

            if (_letterEntryTexture2DFrame1 == null)
            {
                _letterEntryTexture2DFrame1 = new Texture2D(graphicsDevice, 1, 1);
                _letterEntryTexture2DFrame1.SetData(new[] { Color.Transparent });
            }

            _letterEntrySpriteFrame0 = new Sprite(_letterEntryTexture2DFrame0, 0, 0, 20, 5);
            _letterEntrySpriteFrame1 = new Sprite(_letterEntryTexture2DFrame1, 0, 0, 20, 5);

            List<AnimatedSprite.AnimatedSpriteFrame> letterEntryAnimatedSpriteFrames =
                new List<AnimatedSprite.AnimatedSpriteFrame>
                {
                    new AnimatedSprite.AnimatedSpriteFrame(_letterEntrySpriteFrame0, 0.5f),
                    new AnimatedSprite.AnimatedSpriteFrame(_letterEntrySpriteFrame1, 0.5f)
                };
            _letterEntryAnimatedSprite = new AnimatedSprite(letterEntryAnimatedSpriteFrames, 0, 0, 0, 0);
        }

        public void Update(GameTime gameTime)
        {
            _letterEntryAnimatedSprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _letterEntryAnimatedSprite.Draw(spriteBatch, _position, 0f);
        }
    }
}
