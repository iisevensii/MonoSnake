using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.Infrastructure;

namespace MonoSnake.UI
{
    public class ToggleUiButton : UiButton
    {
        private bool _isToggled;
        private readonly Sprite _firstNormalStateSprite;
        private readonly Sprite _firstHoverStateSprite;
        private readonly Sprite _secondNormalStateSprite;
        private readonly Sprite _secondHoverStateSprite;
        public event EventHandler ClickEvent;

        public bool IsToggled => _isToggled;

        public bool IsEnabled { get; set; }

        public ToggleUiButton(Sprite firstNormalStateSprite, Sprite firstHoverStateSprite, Sprite secondNormalStateSprite, Sprite secondHoverStateSprite, Vector2 position, float rotation)
            : base(firstNormalStateSprite, firstHoverStateSprite, position, rotation)
        {
            _firstNormalStateSprite = firstNormalStateSprite;
            _firstHoverStateSprite = firstHoverStateSprite;
            _secondNormalStateSprite = secondNormalStateSprite;
            _secondHoverStateSprite = secondHoverStateSprite;
            ClickEvent += ToggleUiButton_ClickEvent;
        }

        protected override void OnClick(EventArgs e)
        {
            if (!IsEnabled)
                return;

            EventHandler handler = this.ClickEvent;
            handler?.Invoke(this, e); ;
        }

        private void ToggleUiButton_ClickEvent(object sender, EventArgs e)
        {
            if (!IsEnabled)
                return;

            Toggle();
        }

        public void Toggle()
        {
            _isToggled = !_isToggled;

            if (_isToggled)
            {
                Sprite = !IsMouseOver ? _firstNormalStateSprite : _firstHoverStateSprite;
            }
            else
            {
                Sprite = !IsMouseOver ? _secondNormalStateSprite : _secondHoverStateSprite;
            }
        }

        protected override void UpdateButtonState()
        {
            base.UpdateButtonState();

            if (!IsEnabled)
            {
                Sprite = IsToggled ? _secondNormalStateSprite : _firstNormalStateSprite;
                return;
            }

            if (IsMouseOver)
            {
                    Sprite = !_isToggled ? _firstHoverStateSprite : _secondHoverStateSprite;
            }
            else
            {
                Sprite = !_isToggled ? _firstNormalStateSprite : _secondNormalStateSprite;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateButtonState();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }
    }
}
