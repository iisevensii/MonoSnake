using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.UI
{
    public class CenteredUiDialog
    {
        private readonly CenteredUiFrame _centeredUiFrame;
        private readonly UiButton _confirmButton;
        private readonly UiButton _cancelButton;

        public CenteredUiDialog(CenteredUiFrame centeredUiFrame, UiButton confirmButton, UiButton cancelButton)
        {
            _centeredUiFrame = centeredUiFrame;
            _confirmButton = confirmButton;
            _cancelButton = cancelButton;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _centeredUiFrame.Draw(spriteBatch, gameTime);
            _confirmButton.Draw(spriteBatch, gameTime);
            _cancelButton.Draw(spriteBatch, gameTime);
        }
    }
}
