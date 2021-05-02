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
        private readonly GraphicsDevice _graphicsDevice;
        private readonly CenteredUiFrame _centeredUiFrame;
        private readonly SpriteFont _dialogTitleFont;
        private readonly SpriteFont _dialogPromptFont;
        private readonly string _title;
        private readonly UiButton _cancelButton;
        private readonly UiButton _confirmButton;
        private readonly Rectangle _headerRectangle;
        private readonly Texture2D _headerRectangleTexture2D;
        private readonly Color _headerBackgroundColor;
        private readonly Color _headerForegroundColor;
        private readonly Vector2 _titleTextPosition;
        private readonly Vector2 _titleTextSize;
        private readonly Vector2 _promptTextSize;
        private readonly Vector2 _promptTextPosition;

        public event EventHandler CancelEvent;
        public event EventHandler ConfirmEvent;

        public string Prompt { get; set; }
        public bool DrawHeaderBackground { get; set; } = true;

        public CenteredUiDialog(GraphicsDevice graphicsDevice, CenteredUiFrame centeredUiFrame, SpriteFont dialogTitleFont, SpriteFont dialogPromptFont, string title, string prompt, UiButton confirmButton, UiButton cancelButton, Color headerBackgroundColor, Color headerForegroundColor)
        {
            _graphicsDevice = graphicsDevice;
            _centeredUiFrame = centeredUiFrame;
            _dialogTitleFont = dialogTitleFont;
            _dialogPromptFont = dialogPromptFont;
            _title = title;
            Prompt = prompt;
            _confirmButton = confirmButton;
            _cancelButton = cancelButton;
            _headerBackgroundColor = headerBackgroundColor;
            _headerForegroundColor = headerForegroundColor;

            var confirmX = _centeredUiFrame.Position.X + _centeredUiFrame.ActualWidth - _confirmButton.Width - _centeredUiFrame.BottomRightSpriteWidth - 5;
            var confirmY = _centeredUiFrame.Position.Y + _centeredUiFrame.ActualHeight - _confirmButton.Height - _centeredUiFrame.BottomRightSpriteHeight - 10;

            var cancelX = confirmX - _cancelButton.Width - 10;
            var cancelY = confirmY;

            _confirmButton.Position = new Vector2(confirmX, confirmY);
            _cancelButton.Position = new Vector2(cancelX, cancelY);
            _headerRectangle = new Rectangle((int) _centeredUiFrame.Position.X, (int) _centeredUiFrame.Position.Y, _centeredUiFrame.ActualWidth - centeredUiFrame.TopRightSpriteWidth, 50);
            _headerRectangleTexture2D = new Texture2D(_graphicsDevice, 1, 1);
            _headerRectangleTexture2D.SetData(new[] {_headerBackgroundColor });
            _titleTextSize = _dialogTitleFont.MeasureString(_title);
            _promptTextSize = _dialogPromptFont.MeasureString(Prompt);
            int titleBarBottom = (int) (_centeredUiFrame.Position.Y + _headerRectangle.Height);
            int dialogBottom = (int) _centeredUiFrame.Position.Y + _centeredUiFrame.ActualHeight;
            int y = (int) (((dialogBottom - titleBarBottom) /2) + titleBarBottom - (_promptTextSize.Y));
            _titleTextPosition = new Vector2(_centeredUiFrame.Position.X + _centeredUiFrame.ActualWidth /2 - _titleTextSize.X /2, _centeredUiFrame.Position.Y - _titleTextSize.Y /2 + 25);
            _promptTextPosition = new Vector2(_centeredUiFrame.Position.X + _centeredUiFrame.ActualWidth /2 - _promptTextSize.X /2, y);

            _cancelButton.ClickedEvent += CancelClickedEvent;
            _confirmButton.ClickedEvent += ConfirmClickedEvent; 
        }

        private void CancelClickedEvent(object sender, EventArgs e)
        {
            EventHandler handler = this.CancelEvent;
            handler?.Invoke(this, e);
        }

        private void ConfirmClickedEvent(object sender, EventArgs e)
        {
            EventHandler handler = this.ConfirmEvent;
            handler?.Invoke(this, e);
        }

        public void Update(GameTime gameTime)
        {
            _cancelButton.Update(gameTime);
            _confirmButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _centeredUiFrame.Draw(spriteBatch, gameTime);
            _confirmButton.Draw(spriteBatch, gameTime);
            _cancelButton.Draw(spriteBatch, gameTime);

            if(DrawHeaderBackground)
                spriteBatch.Draw(_headerRectangleTexture2D, _headerRectangle, _headerBackgroundColor);

            spriteBatch.DrawString
            (
                _dialogTitleFont,
                _title,
                _titleTextPosition,
                _headerForegroundColor,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );

            spriteBatch.DrawString
            (
                _dialogPromptFont,
                Prompt,
                _promptTextPosition,
                _headerForegroundColor,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }
}
