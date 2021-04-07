using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.GameObjects;
using MonoSnake.Infrastructure;

namespace MonoSnake
{
    public class SnakeGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _appleTexture;
        private Vector2 _applePosition;
        private float _appleSpeed;
        private int _screenWidth;
        private int _screenHeight;
        private SnakeHead _snakeHeadGameObject;
        private Apple _appleGameObject;

        private InputController _inputController;
        private Snake _snake;
        private SpriteFont _scoreBoardFont;
        private Texture2D _gameAreaRectangleTexture;
        private Texture2D _snakeHeadSpriteSheet;
        private Texture2D _snakeSegmentsSpriteSheet;
        private Texture2D _snakeHeadRectangleTexture;
        private Rectangle _gameAreaRectangle;
        private Rectangle _snakeHeadRectangle;
        private Rectangle _appleRectangle;
        private bool _appleEaten;
        private const int HIT_BOX_PADDING = 5;
        private const int DEFAULT_SPRITE_SIZE = 42;
        private const int DEFAULT_SPRITE_HALF_SIZE = 21;
        private const string APPLE_SPRITE_SHEET_NAME = "Apple";
        private const string SNAKE_HEAD_SPRITE_SHEET_NAME = "SnakeHead";
        private const string SNAKE_SEGMENTS_SPRITE_SHEET_NAME = "SnakeSegments";

        public SnakeGame()  
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 780;
            _graphics.PreferredBackBufferHeight = 820;
            _graphics.ApplyChanges();
            _screenWidth = _graphics.PreferredBackBufferWidth;
            _screenHeight = _graphics.PreferredBackBufferHeight;
            _applePosition = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            _appleSpeed = 1000f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Font
            _scoreBoardFont = Content.Load<SpriteFont>("score");
            // Load Textures
            _appleTexture = Content.Load<Texture2D>(APPLE_SPRITE_SHEET_NAME);
            _snakeHeadSpriteSheet = Content.Load<Texture2D>(SNAKE_HEAD_SPRITE_SHEET_NAME);
            _snakeSegmentsSpriteSheet = Content.Load<Texture2D>(SNAKE_SEGMENTS_SPRITE_SHEET_NAME);

            // Create Sprite Objects
            Sprite appleSprite = new Sprite(_appleTexture, 0, 0, _appleTexture.Width, _appleTexture.Height)
            {
                Origin = new Vector2(_appleTexture.Width / 2f, _appleTexture.Height / 2f)
            };
            Sprite snakeHeadSprite = new Sprite(_snakeHeadSpriteSheet, 0, 0, _snakeHeadSpriteSheet.Width, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            Sprite snakeTailSprite = new Sprite(_snakeSegmentsSpriteSheet, 84, 0, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            Sprite snakeStraightBodySprite = new Sprite(_snakeSegmentsSpriteSheet, 84, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            Sprite snakeCW_UpToRight_CCW_LeftToDownSprite = new Sprite(_snakeSegmentsSpriteSheet, 0, 0, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            Sprite snakeCW_RightToDown_CCW_UpToLeftSprite = new Sprite(_snakeSegmentsSpriteSheet, 0, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            Sprite snakeCW_DownToLeft_CCW_RightToUpSprite = new Sprite(_snakeSegmentsSpriteSheet, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            Sprite snakeCW_LeftToUp_CCW_DownToRightSprite = new Sprite(_snakeSegmentsSpriteSheet, DEFAULT_SPRITE_SIZE, 0, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };

            // Create GameObjects
            _snakeHeadGameObject = new SnakeHead(snakeHeadSprite, new Vector2(53, 83));
            _appleGameObject = new Apple(appleSprite, new Vector2(_graphics.PreferredBackBufferWidth / 2f + DEFAULT_SPRITE_HALF_SIZE, _graphics.PreferredBackBufferHeight / 2f + DEFAULT_SPRITE_HALF_SIZE));
            _appleGameObject.Sprite.Scale = new Vector2(0.65f, 0.65f);

            // Initialize Snake
            _snake = new Snake
            (
                _snakeHeadGameObject,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                _scoreBoardFont,
                snakeTailSprite,
                snakeStraightBodySprite,
                snakeCW_UpToRight_CCW_LeftToDownSprite,
                snakeCW_RightToDown_CCW_UpToLeftSprite,
                snakeCW_DownToLeft_CCW_RightToUpSprite,
                snakeCW_LeftToUp_CCW_DownToRightSprite
            );

            _gameAreaRectangleTexture = new Texture2D(GraphicsDevice, 2, 2);
            _snakeHeadRectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _gameAreaRectangleTexture.SetData(new [] { Color.White, Color.White, Color.White, Color.White, });
            _snakeHeadRectangleTexture.SetData(new[] { Color.White });
            _gameAreaRectangle = new Rectangle
            (
                20,
                50,
                _graphics.PreferredBackBufferWidth -35,
                _graphics.PreferredBackBufferHeight -70
            );
            _snakeHeadRectangle = new Rectangle
            (
                (int)Math.Round(_snake.SnakeHead.Position.X - _snake.SnakeHead.Sprite.Width / 2f * _snake.SnakeHead.Sprite.Scale.X) - HIT_BOX_PADDING,
                (int)Math.Round(_snake.SnakeHead.Position.Y - _snake.SnakeHead.Sprite.Height / 2f * _snake.SnakeHead.Sprite.Scale.Y) - HIT_BOX_PADDING,
                (int)(_snake.SnakeHead.Sprite.Width * _snake.SnakeHead.Sprite.Scale.X),
                (int)(_snake.SnakeHead.Sprite.Height * _snake.SnakeHead.Sprite.Scale.Y)
            );
            _appleRectangle = new Rectangle
                (
                (int)Math.Round(_appleGameObject.Position.X - _appleGameObject.Sprite.Width / 2f * _appleGameObject.Sprite.Scale.X) - HIT_BOX_PADDING,
                (int)Math.Round(_appleGameObject.Position.Y - _appleGameObject.Sprite.Height / 2f * _appleGameObject.Sprite.Scale.Y) - HIT_BOX_PADDING,
                (int) (_appleGameObject.Sprite.Width * _appleGameObject.Sprite.Scale.X) + HIT_BOX_PADDING * 2,
                (int) (_appleGameObject.Sprite.Height * _appleGameObject.Sprite.Scale.Y) + HIT_BOX_PADDING * 2
                );

            // Initialize InputController
            _inputController = new InputController(_snake);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var player1GamePadState = GamePad.GetState(PlayerIndex.One);

            if (player1GamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Process Input
            _inputController.ProcessInput();

            // Update GameObjects
            _snake.Update(gameTime);

            _snakeHeadRectangle.X = (int)Math.Round(_snake.SnakeHead.Position.X - _snake.SnakeHead.Sprite.Width / 2f * _snake.SnakeHead.Sprite.Scale.X);
            _snakeHeadRectangle.Y = (int)Math.Round(_snake.SnakeHead.Position.Y - _snake.SnakeHead.Sprite.Height / 2f * _snake.SnakeHead.Sprite.Scale.Y);

            if (_snakeHeadRectangle.Intersects(_appleRectangle))
            {
                Trace.WriteLine("GULP!");
                _appleEaten = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Draw Game Area
            foreach (Vector2 outlinePixel in _gameAreaRectangle.OutlinePixels())
            {
                _spriteBatch.Draw(_gameAreaRectangleTexture, outlinePixel, Color.Green);
            }

            _snake.Draw(_spriteBatch, gameTime);

            if (!_appleEaten)
            {
                _appleGameObject.Draw(_spriteBatch, gameTime);

                foreach (Vector2 outlinePixel in _appleRectangle.OutlinePixels())
                {
                    _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Red);
                }
            }

            //_spriteBatch.Draw(_snakeHeadRectangleTexture, _snakeHeadRectangle, Color.Red);
            foreach (Vector2 outlinePixel in _snakeHeadRectangle.OutlinePixels())
            {
                _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Red);
            }

            // Draw Grid (Temp)
            List<Rectangle> cells = new List<Rectangle>();
            // Columns
            // Rows
            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    Rectangle nextRectangle = new Rectangle(i * 40 + 32, j * 40 + 62, 42, 42);
                    cells.Add(nextRectangle);
                }
            }

            foreach (Rectangle rectangle in cells)
            {
                foreach (Vector2 outlinePixel in rectangle.OutlinePixels())
                {
                    _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Blue);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
