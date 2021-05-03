using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.UI;

namespace MonoSnake.Infrastructure
{
    public class ScoreBoard
    {
        private const string HIGH_SCORES_FILE_NAME = "HighScores.json";
        private const int SCORES_MARGIN_TOP = 50;
        private const int SCORE_BOARD_VERTICAL_MARGIN = 20;
        private const int SCORE_BOARD_HORIZONTAL_MARGIN = 20;
        private const int SCORE_VERTICAL_SPACING = 50;

        private readonly string _highScoresStoragePath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly SpriteFont _scoreBoardFont;
        private readonly CenteredUiDialog _confirmationDialog;

        private GraphicsDevice _graphicsDevice;
        private CenteredUiFrame _uiFrame;
        private HighScores HighScores { get; set; }
        private TextEntry _textEntry;
        private int _screenWidth;
        private int _screenHeight;
        private int _leftInsideEdgeOfFrame;
        private int _rightInsideEdgeOfFrame;
        private int _margin;
        private int _newHighScoreRowIndex = 0;
        private int _newHighScore;

        private List<ScoreEntry> _scoreEntryBeforeList = new List<ScoreEntry>();
        private List<ScoreEntry> _scoreEntryAfterList = new List<ScoreEntry>();

        public bool InHighScoreEntryConfirmState { get; set; }
        public event EventHandler HighScoreEntryCompletedEvent;

        private const int SCORE_BOARD_FONT_LEFT_PADDING = 10;

        public bool HighScoreEntryState { get; set; }

        public ScoreBoard(string applicationPath, GraphicsDevice graphicsDevice, SpriteFont scoreBoardFont, CenteredUiFrame uiFrame, CenteredUiDialog confirmationDialog, int screenWidth, int screenHeight)
        {
            _highScoresStoragePath = Path.Combine(Path.GetDirectoryName(applicationPath), HIGH_SCORES_FILE_NAME);
            _graphicsDevice = graphicsDevice;
            _scoreBoardFont = scoreBoardFont;
            _uiFrame = uiFrame;
            _confirmationDialog = confirmationDialog;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
            // ToDo: Temporary position set on TextEntry
            _textEntry = new TextEntry(graphicsDevice, new Vector2(screenWidth /2, screenHeight /2), scoreBoardFont);
            _margin = (_screenWidth - _uiFrame.ActualWidth) / 2;
            _leftInsideEdgeOfFrame = SCORE_BOARD_HORIZONTAL_MARGIN + _margin + SCORE_BOARD_FONT_LEFT_PADDING + 40 / 2;
            _rightInsideEdgeOfFrame = _screenWidth - SCORE_BOARD_HORIZONTAL_MARGIN - _margin - SCORE_BOARD_FONT_LEFT_PADDING - 40 / 2;

            HighScores = LoadHighScores();

            _confirmationDialog.CancelEvent += ConfirmationDialogOnCancelEvent;
            _confirmationDialog.ConfirmEvent += ConfirmationDialogOnConfirmEvent;
        }

        private void ConfirmationDialogOnCancelEvent(object sender, EventArgs e)
        {
            EntryCanceled();
        }

        private void ConfirmationDialogOnConfirmEvent(object sender, EventArgs e)
        {
            EntryConfirmed();
        }

        private void EntryCanceled()
        {
            InHighScoreEntryConfirmState = false;
        }

        private void EntryConfirmed()
        {
            AddHighScore(_newHighScore);
            InHighScoreEntryConfirmState = false;
            _textEntry.Reset();
            OnTextEntryCompleted(EventArgs.Empty);
        }

        public void HandleEnterKeyPress(int score = 0)
        {
            if (HighScoreEntryState)
            {
                if (InHighScoreEntryConfirmState)
                    EntryConfirmed();
                else
                    ConfirmNewHighScoreEntry(score);
            }
        }

        public void HandleEscKeyPress()
        {
            EntryCanceled();
        }

        public void ConfirmNewHighScoreEntry(int score)
        {
            ShowConfirmationDialog();
        }

        protected void OnTextEntryCompleted(EventArgs e)
        {
            EventHandler handler = this.HighScoreEntryCompletedEvent;
            handler?.Invoke(this, e);
        }

        private void AddHighScore(int score)
        {
            HighScores.ScoreEntries = new List<ScoreEntry>();
            HighScores.ScoreEntries.AddRange(_scoreEntryBeforeList);
            HighScores.ScoreEntries.Add(new ScoreEntry(false, _textEntry.InputtedString, _newHighScore));
            HighScores.ScoreEntries.AddRange(_scoreEntryAfterList);
            this.SaveHighScores();
        }

        private HighScores LoadHighScores()
        {
            if (File.Exists(_highScoresStoragePath))
            {
                var highScoresText = File.ReadAllText(_highScoresStoragePath);
                HighScores = JsonSerializer.Deserialize<HighScores>(highScoresText, _jsonSerializerOptions);

                return HighScores;
            }

            return new HighScores();
        }

        private void SaveHighScores()
        {
            if (HighScores.HighScoresUpdated)
            {
                string highScoresSerialized = JsonSerializer.Serialize(HighScores, _jsonSerializerOptions);

                File.WriteAllText(_highScoresStoragePath, highScoresSerialized);

                HighScores.HighScoresUpdated = false;
            }
        }

        public bool IsNewHighScore(int score)
        {
            bool isNewHighScore = this.HighScores.ScoreEntries.All(s => score != s.Score)
                                  && (this.HighScores.ScoreEntries.Any(s => score > s.Score)
                                      || score > 0 && !this.HighScores.ScoreEntries.Any());
            return isNewHighScore;
        }

        public void StartHighScoreEntry(int score)
        {
            _newHighScore = score;

            _scoreEntryBeforeList = HighScores.ScoreEntries.Where(s => s.Score > score).ToList();
            _scoreEntryAfterList = HighScores.ScoreEntries.Where(s => s.Score < score).Take(10 - _scoreEntryBeforeList.Count - 1).ToList();

            _newHighScoreRowIndex = HighScores.ScoreEntries.Count - _scoreEntryAfterList.Count;

            // In Memory Update Test
            HighScores = LoadHighScores();
            HighScores.ScoreEntries = new List<ScoreEntry>();
            HighScores.ScoreEntries.AddRange(_scoreEntryBeforeList);
            HighScores.ScoreEntries.Add(new ScoreEntry(true, "", score));
            HighScores.ScoreEntries.AddRange(_scoreEntryAfterList);
        }

        public void KeyInput(Keys key)
        {
            _textEntry.KeyInput(key);
        }

        private void ShowConfirmationDialog()
        {
            InHighScoreEntryConfirmState = true;
        }

        public void Update(GameTime gameTime)
        {
            if (HighScoreEntryState)
            {
                _textEntry.Update(gameTime);
                if (InHighScoreEntryConfirmState)
                {
                    //ToDo: Only update score entry dialog at the right time
                    _confirmationDialog.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float frameYPosition = _uiFrame.Position.Y;
            float yStart = SCORE_BOARD_VERTICAL_MARGIN + frameYPosition + SCORES_MARGIN_TOP + SCORE_VERTICAL_SPACING;

            if (HighScoreEntryState)
            {
                // insert blank named entry at correct position with new high score
                // ToDo: Handle Entry State

                //DrawScoreEntries(spriteBatch, gameTime, _scoreEntryBeforeList);
                //// Text Entry Object
                //DrawScoreEntries(spriteBatch, gameTime, _scoreEntryAfterList);
                DrawScoreEntries(spriteBatch, gameTime, HighScores.ScoreEntries);
            }
            else
            {
                DrawScoreEntries(spriteBatch, gameTime, HighScores.ScoreEntries);
            }

            if (HighScoreEntryState)
            {
                _textEntry.Draw(spriteBatch, gameTime);
                if (InHighScoreEntryConfirmState)
                {
                    //ToDo: Conditionally Draw Confirmation Dialog in Text Entry after high score name is entered and when user hits Enter|Start
                    _confirmationDialog.Draw(spriteBatch, gameTime);
                }
            }
        }

        private void DrawScoreEntries(SpriteBatch spriteBatch, GameTime gameTime, List<ScoreEntry> scoreEntries)
        {
            float frameYPosition = _uiFrame.Position.Y;
            float yStart = SCORE_BOARD_VERTICAL_MARGIN + frameYPosition + SCORES_MARGIN_TOP + SCORE_VERTICAL_SPACING;

            foreach (ScoreEntry scoreEntry in scoreEntries)
            {
                // Current Index
                var scoreEntryIndex = scoreEntries.IndexOf(scoreEntry);

                // Name Text
                string scoreEntryName = scoreEntry.Name ?? "";
                scoreEntryName = scoreEntryName.PadRight(100, ' ');

                // Score Text
                string scoreText = scoreEntry.Score.ToString();

                // Name and Score Text measurements
                Vector2 scoreEntryNameScale = _scoreBoardFont.MeasureString(scoreEntryName);
                Vector2 scoreEntryScoreScale = _scoreBoardFont.MeasureString(scoreText);

                var curX = _rightInsideEdgeOfFrame - (int)scoreEntryScoreScale.X;

                // Name Coordinates
                float scoreEntryNameX = _leftInsideEdgeOfFrame;
                float scoreEntryNameY = yStart + scoreEntryIndex * SCORE_VERTICAL_SPACING;

                // Score Coordinates
                float scoreEntryScoreX = curX;
                float scoreEntryScoreY = yStart + scoreEntryIndex * SCORE_VERTICAL_SPACING;

                // Name and Score Positions
                Vector2 scoreEntryNamePosition = new Vector2(scoreEntryNameX, scoreEntryNameY);
                Vector2 scoreEntryScorePosition = new Vector2(scoreEntryScoreX, scoreEntryScoreY);

                // Position of new high score entry
                if (scoreEntryIndex == _newHighScoreRowIndex)
                {
                    _textEntry.Position = new Vector2(scoreEntryNamePosition.X, scoreEntryScorePosition.Y + scoreEntryNameScale.Y /2);
                }

                // Draw Name
                if (scoreEntryName.Length > 0)
                    spriteBatch.DrawString
                    (
                        _scoreBoardFont,
                        scoreEntryName,
                        scoreEntryNamePosition,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );

                // Draw Score
                if (scoreEntry.Score >= 0)
                    spriteBatch.DrawString
                    (
                        _scoreBoardFont,
                        scoreText,
                        scoreEntryScorePosition,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
            }
        }
    }
}
