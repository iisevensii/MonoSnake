using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.GameObjects;
using MonoSnake.Infrastructure;
using MonoSnake.UI;

namespace MonoSnake
{
    public class SnakeGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FrameCounter _frameCounter = new FrameCounter();

        const int SCREEN_WIDTH = 780;
        const int SCREEN_HEIGHT = 820;

        private Snake _snake;
        private Apple _appleGameObject;
        private SnakeHead _snakeHeadGameObject;

        private InputController _inputController;
        private SpriteFont _logoFont;
        private SpriteFont _gameOverFont;
        private SpriteFont _leaderboardFont;
        private SpriteFont _scoreBoardFont;
        private Texture2D _appleTexture;
        private Texture2D _snakeHeadSpriteSheet;
        private Texture2D _snakeSegmentsSpriteSheet;
        private Texture2D _gameAreaRectangleTexture;
        private Texture2D _snakeHeadRectangleTexture;
        private SoundEffect _eatSoundEffect;
        private SoundEffect[] _moveSoundEffects;
        private Rectangle _gameAreaRectangle;
        private Rectangle _snakeHeadRectangle;
        private Rectangle _appleRectangle;
        private List<Rectangle> _cells;
        private List<Rectangle> _occupiedCells = new List<Rectangle>();
        private List<Rectangle> _unOccupiedCells = new List<Rectangle>();
        private ScoreBoard _scoreBoard;

        // Draw diagnostic grid?
        private bool _drawDiagnosticGrid = false;

        private bool _appleEaten;
        private bool _applePlaced;
        private bool _isGameOver;
        private bool _showFpsMonitor = false;
        private Sprite _snakeCwUpToRightCcwLeftToDownSprite;
        private Sprite _snakeCwRightToDownCcwUpToLeftSprite;
        private Sprite _snakeCwDownToLeftCcwRightToUpSprite;
        private Sprite _snakeCwLeftToUpCcwDownToRightSprite;
        private Sprite _snakeStraightBodySprite;
        private UiFrame _startScreenUiFrame;
        private bool _atStartMenu = true;
        private bool _atHighScoreScreen = false;
        private CenteredUiFrame _highScoreUiFrame;
        private const string MONO_SNAKE_STRING = "MonoSnake";
        private const int START_SCREEN_TRANSPARENCY = 200;
        const string GAME_OVER_STRING = "Game Over";
        private const string LEADERBOARD_STRING = "Leaderboard";

        const float GAME_OVER_FONT_SCALE = 1f;
        private const int HIT_BOX_PADDING = 5;
        private const int DEFAULT_SPRITE_SIZE = 40;
        private const int DEFAULT_SPRITE_HALF_SIZE = 20;
        private const string APPLE_SPRITE_SHEET_NAME = "Apple";
        private const string SNAKE_HEAD_SPRITE_SHEET_NAME = "SnakeHead";
        private const string SNAKE_SEGMENTS_SPRITE_SHEET_NAME = "SnakeSegments";
        private const string EAT_SOUND_EFFECT_NAME = "eat";
        private const string HISS_SOUND_EFFECT_NAME = "hiss";
        private const string MOVE_SOUND_EFFECT_NAME = "sand_rattle";

        public SnakeGame()  
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _scoreBoard = new ScoreBoard(Assembly.GetEntryAssembly().Location);
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadAssets();

            InitializeGameObjects();

            InitializeDiagnosticObjects();

            GenerateGrid();

            GenerateApple();

            // Initialize InputController
            _inputController = new InputController(_snake);
            _inputController.StartEvent += InputController_StartEvent;
            _inputController.RestartEvent += InputController_RestartEvent;
            _inputController.ExitEvent += InputController_ExitEvent;
            _inputController.HeadTurnEvent += InputController_HeadTurnEvent;
        }

        private void InputController_StartEvent(object sender, EventArgs e)
        {
            InitializeGameObjects();
            // Initialize InputController
            _inputController = new InputController(_snake);
            _inputController.StartEvent += InputController_StartEvent;
            _inputController.ExitEvent += InputController_ExitEvent;
            _inputController.RestartEvent += InputController_RestartEvent;
            _inputController.HeadTurnEvent += InputController_HeadTurnEvent;
            GenerateGrid();
            _appleEaten = false;
            _applePlaced = false;
            GenerateApple();
            _isGameOver = false;
        }

        private void InputController_RestartEvent(object sender, EventArgs e)
        {
            InitializeGameObjects();
            // Initialize InputController
            _inputController = new InputController(_snake);
            _inputController.StartEvent += InputController_StartEvent;
            _inputController.ExitEvent += InputController_ExitEvent;
            _inputController.RestartEvent += InputController_HeadTurnEvent;
            _inputController.HeadTurnEvent += InputController_HeadTurnEvent;
            GenerateGrid();
            _atStartMenu = false;
            _atHighScoreScreen = false;
            _appleEaten = false;
            _applePlaced = false;
            GenerateApple();
            _isGameOver = false;
        }

        private void InputController_ExitEvent(object sender, EventArgs e)
        {
            Exit();
        }

        private Random random = new Random();
        private void InputController_HeadTurnEvent(object sender, EventArgs e)
        {
            if (_moveSoundEffects != null)
            {
                int roll = random.Next(1, 101);
                if (roll <= 55) //55% chance
                {
                    //pick either a hiss or a sand shuffle (or whatever else we put in the array)
                    int i = random.Next(0, _moveSoundEffects.Length);
                    //randomly changing the pitch is great way to trick
                    //player's into not hating the same sound repeated
                    double randomAmplify = random.NextDouble() - 0.5;

                    _moveSoundEffects[i].Play(1f, (float)randomAmplify, 0f);
                }
            }
        }

        private void InitializeGameObjects()
        {
            // Create Sprite Objects
            Sprite appleSprite = new Sprite(_appleTexture, 0, 0, _appleTexture.Width, _appleTexture.Height)
            {
                Origin = new Vector2(_appleTexture.Width / 2f, _appleTexture.Height / 2f)
            };
            PositionedTexture2D headPositionedTexture2D = new PositionedTexture2D(_snakeHeadSpriteSheet, 1, 0, 0);
            Sprite snakeHeadSprite = new Sprite(headPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            PositionedTexture2D tailPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 0, 2);
            Sprite snakeTailSprite = new Sprite(tailPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };

            PositionedTexture2D straightBodyPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 2);
            _snakeStraightBodySprite = new Sprite(straightBodyPositionedTexture2D,DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };

            PositionedTexture2D snakeCwUpToRightCcwLeftToDownPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 0, 0);
            _snakeCwUpToRightCcwLeftToDownSprite = new Sprite(snakeCwUpToRightCcwLeftToDownPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };
            
            PositionedTexture2D snakeCwRightToDownCcwUpToLeftPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 0);
            _snakeCwRightToDownCcwUpToLeftSprite = new Sprite(snakeCwRightToDownCcwUpToLeftPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };

            PositionedTexture2D snakeCwDownToLeftCcwRightToUpPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 1);
            _snakeCwDownToLeftCcwRightToUpSprite = new Sprite(snakeCwDownToLeftCcwRightToUpPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };

            PositionedTexture2D snakeCwLeftToUpCcwDownToRightPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 0, 1);
            _snakeCwLeftToUpCcwDownToRightSprite = new Sprite(snakeCwLeftToUpCcwDownToRightPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE)
            };

            // Create GameObjects
            _snakeHeadGameObject = new SnakeHead(snakeHeadSprite, new Vector2(53, 83));
            _appleGameObject = new Apple(appleSprite,
                new Vector2(_graphics.PreferredBackBufferWidth / 2f + DEFAULT_SPRITE_HALF_SIZE,
                    _graphics.PreferredBackBufferHeight / 2f + DEFAULT_SPRITE_HALF_SIZE));
            _appleGameObject.Sprite.Scale = new Vector2(0.65f, 0.65f);

            // Initialize Snake
            _snake = new Snake
            (
                _snakeHeadGameObject,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                _scoreBoardFont,
                snakeTailSprite,
                _snakeStraightBodySprite,
                _snakeCwUpToRightCcwLeftToDownSprite,
                _snakeCwRightToDownCcwUpToLeftSprite,
                _snakeCwDownToLeftCcwRightToUpSprite,
                _snakeCwLeftToUpCcwDownToRightSprite
            );

            if (_atHighScoreScreen)
            {
                _highScoreUiFrame = new CenteredUiFrame
                (
                    _graphics,
                    Vector2.Zero,
                    18,
                    18,
                    SCREEN_WIDTH,
                    SCREEN_HEIGHT,
                    _snakeStraightBodySprite,
                    _snakeStraightBodySprite,
                    _snakeCwUpToRightCcwLeftToDownSprite,
                    _snakeCwRightToDownCcwUpToLeftSprite,
                    _snakeCwDownToLeftCcwRightToUpSprite,
                    _snakeCwLeftToUpCcwDownToRightSprite,
                    Color.FromNonPremultiplied(46, 51, 106, START_SCREEN_TRANSPARENCY)
                );
            }
            if (_atStartMenu)
            {
                //Vector2 startScreenUiFramePosition = new Vector2(SCREEN_WIDTH / 2 - _startScreenUiFrame.ActualWidth / 2 + DEFAULT_SPRITE_HALF_SIZE, SCREEN_HEIGHT / 2 - _startScreenUiFrame.ActualHeight / 2 + DEFAULT_SPRITE_HALF_SIZE);
                _startScreenUiFrame = new CenteredUiFrame
                (
                    _graphics,
                    Vector2.Zero,
                    18,
                    18,
                    SCREEN_WIDTH,
                    SCREEN_HEIGHT,
                    _snakeStraightBodySprite,
                    _snakeStraightBodySprite,
                    _snakeCwUpToRightCcwLeftToDownSprite,
                    _snakeCwRightToDownCcwUpToLeftSprite,
                    _snakeCwDownToLeftCcwRightToUpSprite,
                    _snakeCwLeftToUpCcwDownToRightSprite,
                    Color.FromNonPremultiplied(46, 51, 106, START_SCREEN_TRANSPARENCY)
                );
            }
        }

        private void LoadAssets()
        {
            // Load Fonts
            _logoFont = Content.Load<SpriteFont>("Logo");
            _gameOverFont = Content.Load<SpriteFont>("GameOver");
            _leaderboardFont = Content.Load<SpriteFont>("Arcade");
            _scoreBoardFont = Content.Load<SpriteFont>("score");
            // Load Textures
            _appleTexture = Content.Load<Texture2D>(APPLE_SPRITE_SHEET_NAME);
            _snakeHeadSpriteSheet = Content.Load<Texture2D>(SNAKE_HEAD_SPRITE_SHEET_NAME);
            _snakeSegmentsSpriteSheet = Content.Load<Texture2D>(SNAKE_SEGMENTS_SPRITE_SHEET_NAME);
            _eatSoundEffect = Content.Load<SoundEffect>(EAT_SOUND_EFFECT_NAME);
            _moveSoundEffects = new SoundEffect[] {
                Content.Load<SoundEffect>(HISS_SOUND_EFFECT_NAME),
                Content.Load<SoundEffect>(MOVE_SOUND_EFFECT_NAME)
            };
        }

        private void InitializeDiagnosticObjects()
        {
            _gameAreaRectangleTexture = new Texture2D(GraphicsDevice, 2, 2);
            _snakeHeadRectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _gameAreaRectangleTexture.SetData(new[] {Color.White, Color.White, Color.White, Color.White,});
            _snakeHeadRectangleTexture.SetData(new[] {Color.White});
            _gameAreaRectangle = new Rectangle
            (
                20,
                50,
                _graphics.PreferredBackBufferWidth - 35,
                _graphics.PreferredBackBufferHeight - 70
            );
            _snakeHeadRectangle = new Rectangle
            (
                (int) Math.Round(_snake.SnakeHead.Position.X -
                                 _snake.SnakeHead.Sprite.Width / 2f * _snake.SnakeHead.Sprite.Scale.X) - HIT_BOX_PADDING,
                (int) Math.Round(_snake.SnakeHead.Position.Y -
                                 _snake.SnakeHead.Sprite.Height / 2f * _snake.SnakeHead.Sprite.Scale.Y) - HIT_BOX_PADDING,
                (int) (_snake.SnakeHead.Sprite.Width * _snake.SnakeHead.Sprite.Scale.X),
                (int) (_snake.SnakeHead.Sprite.Height * _snake.SnakeHead.Sprite.Scale.Y)
            );
        }

        protected override void Update(GameTime gameTime)
        {
            // Process Input
            _inputController.ProcessInput();

            if (_isGameOver)
                return;

            // Update GameObjects
            if(!_atStartMenu && !_atHighScoreScreen)
                _snake.Update(gameTime);

            _snakeHeadRectangle.X = (int)Math.Round(_snake.SnakeHead.Position.X - _snake.SnakeHead.Sprite.Width / 2f * _snake.SnakeHead.Sprite.Scale.X);
            _snakeHeadRectangle.Y = (int)Math.Round(_snake.SnakeHead.Position.Y - _snake.SnakeHead.Sprite.Height / 2f * _snake.SnakeHead.Sprite.Scale.Y);

            // Hit edge of play area
            if (!_gameAreaRectangle.Contains(_snake.SnakeHead.Position))
            {
                // GAME OVER!
                EndGameAndRecordScore();
            }

            if (_snake.SnakeSegments.Any(s =>
                s.Position.X == _snake.SnakeHead.Position.X && s.Position.Y == _snake.SnakeHead.Position.Y))
            {
                EndGameAndRecordScore();
            }

            if (_snakeHeadRectangle.Intersects(_appleRectangle))
            {
                _appleEaten = true;
                _applePlaced = false;
                _eatSoundEffect.Play(1f, 0f, 0f);
                _snake.AddSegment();
            }

            GenerateGrid();

            if (_appleEaten)
            {
                GenerateApple();
            }

            if (_atStartMenu)
            {
                _startScreenUiFrame.Update(gameTime);
            }

            if (_atHighScoreScreen)
            {
                _highScoreUiFrame.Update(gameTime);
            }
            base.Update(gameTime);
        }

        private void EndGameAndRecordScore()
        {
            _scoreBoard.HighScores.AddHighScore(new ScoreEntry("SeVeN", _snake.Score));

            _scoreBoard.SaveHighScores();

            _isGameOver = true;
        }

        private void GenerateGrid()
        {
            // Draw Grid (Temp)
            _cells = new List<Rectangle>();
            //Occupied Cells
            _occupiedCells = new List<Rectangle>();
            // Columns
            // Rows
            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    Rectangle rectangle = new Rectangle(i * 40 + 32, j * 40 + 62, 42, 42);
                    _cells.Add(rectangle);
                }
            }

            foreach (Rectangle cell in _cells)
            {
                if (Math.Round(_snake.SnakeHead.Position.X) == cell.X + 21 &&
                    Math.Round(_snake.SnakeHead.Position.Y) == cell.Y + 21)
                {
                    _occupiedCells.Add(cell);
                }

                if (_snake.SnakeSegments.Any(s =>
                    Math.Round(s.Position.X) == cell.X + 21 && Math.Round(s.Position.Y) == cell.Y + 21))
                {
                    _occupiedCells.Add(cell);
                }
            }

            _unOccupiedCells = _cells.Except(_occupiedCells).ToList();
        }

        private void GenerateApple()
        {
            Random randomApple = new Random();

            int nextIndex = randomApple.Next(0, _unOccupiedCells.Count);

            if (!_applePlaced)
            {
                if (_unOccupiedCells.Count > 0)
                {
                    Vector2 nextAppleLoc = new Vector2(_unOccupiedCells[nextIndex].X + 22, _unOccupiedCells[nextIndex].Y + 22);

                    _appleGameObject.Position = nextAppleLoc;
                }

                _appleRectangle = new Rectangle
                (
                    (int) Math.Round(_appleGameObject.Position.X -
                                     _appleGameObject.Sprite.Width / 2f * _appleGameObject.Sprite.Scale.X) - HIT_BOX_PADDING,
                    (int) Math.Round(_appleGameObject.Position.Y -
                                     _appleGameObject.Sprite.Height / 2f * _appleGameObject.Sprite.Scale.Y) - HIT_BOX_PADDING,
                    (int) (_appleGameObject.Sprite.Width * _appleGameObject.Sprite.Scale.X) + HIT_BOX_PADDING * 2,
                    (int) (_appleGameObject.Sprite.Height * _appleGameObject.Sprite.Scale.Y) + HIT_BOX_PADDING * 2
                );

                _applePlaced = true;
                _appleEaten = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            var fps = $"FPS: {_frameCounter.CurrentFramesPerSecond}";

            if (_isGameOver)
            {
                _spriteBatch.Begin();
                DrawGameOverText();
                _spriteBatch.End();
                return;
            }

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (_showFpsMonitor)
                _spriteBatch.DrawString
                    (
                        _scoreBoardFont,
                        fps,
                        new Vector2(20, 10),
                        Color.Blue,
                        0f,
                        Vector2.Zero,
                        new Vector2(2, 2),
                        SpriteEffects.None,
                    0f
                    );

            if (_isGameOver)
                DrawGameOverText();

            // Draw Game Area
            if(!_atStartMenu && !_atHighScoreScreen)
                foreach (Vector2 outlinePixel in _gameAreaRectangle.OutlinePixels())
            {
                _spriteBatch.Draw(_gameAreaRectangleTexture, outlinePixel, Color.Green);
            }

            if(!_atStartMenu && !_atHighScoreScreen)
                _snake.Draw(_spriteBatch, gameTime);

            if(!_atStartMenu && !_atHighScoreScreen)
                _appleGameObject.Draw(_spriteBatch, gameTime);
            
            if (_drawDiagnosticGrid)
            {
                foreach (Rectangle rectangle in _cells)
                {
                    foreach (Vector2 outlinePixel in rectangle.OutlinePixels())
                    {
                        _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Blue);
                    }
                }

                // Rectangle around Snake Head
                foreach (Vector2 outlinePixel in _snakeHeadRectangle.OutlinePixels())
                {
                    _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Red);
                }

                // Rectangle around Apple
                foreach (Vector2 outlinePixel in _appleRectangle.OutlinePixels())
                {
                    _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Red);
                }
            }

            if (_atStartMenu)
            {
                DrawStartScreenUiFrame(gameTime);
                DrawLogoText();
            }

            if (_atHighScoreScreen)
            {
                DrawHighScoreUiFrame(gameTime);
                DrawLeaderboardText();
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        
        private void DrawStartScreenUiFrame(GameTime gameTime)
        {
            _startScreenUiFrame.Draw(_spriteBatch, gameTime);
        }

        private void DrawHighScoreUiFrame(GameTime gameTime)
        {
            _highScoreUiFrame.Draw(_spriteBatch, gameTime);
        }

        private void DrawLogoText()
        {
            Vector2 logoStringWidth = _logoFont.MeasureString(MONO_SNAKE_STRING);
            float logoX = _startScreenUiFrame.Position.X + _startScreenUiFrame.ActualWidth /2 - logoStringWidth.X / 2 - _snakeCwUpToRightCcwLeftToDownSprite.Width /2;
            float logoY = _startScreenUiFrame.Position.Y / 2 + logoStringWidth.Y;
            Vector2 logoPosition = new Vector2(logoX, logoY);

            _spriteBatch.DrawString
            (
                _logoFont,
                MONO_SNAKE_STRING,
                logoPosition,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        private void DrawLeaderboardText()
        {
            Vector2 leaderboardStringWidth = _leaderboardFont.MeasureString(LEADERBOARD_STRING);
            float leaderboardX = _highScoreUiFrame.Position.X + _highScoreUiFrame.ActualWidth / 2 - leaderboardStringWidth.X / 2 - _snakeCwUpToRightCcwLeftToDownSprite.Width / 2;
            float leaderboardY = _highScoreUiFrame.Position.Y / 2 + leaderboardStringWidth.Y;
            Vector2 leaderboardPosition = new Vector2(leaderboardX, leaderboardY);

            _spriteBatch.DrawString
            (
                _leaderboardFont,
                LEADERBOARD_STRING,
                leaderboardPosition,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        private void DrawGameOverText()
        {
            Vector2 gameOverStringWidth = _gameOverFont.MeasureString(GAME_OVER_STRING) * GAME_OVER_FONT_SCALE;
            float gameOverX = SCREEN_WIDTH / 2 - gameOverStringWidth.X / 2;
            float gameOverY = SCREEN_HEIGHT / 2 - gameOverStringWidth.Y / 2;
            Vector2 gameOverPosition = new Vector2(gameOverX, gameOverY);

            _spriteBatch.DrawString
            (
                _gameOverFont,
                GAME_OVER_STRING,
                gameOverPosition,
                Color.White,
                0f,
                Vector2.Zero,
                GAME_OVER_FONT_SCALE,
                SpriteEffects.None,
                0f
            );
        }
    }
}
