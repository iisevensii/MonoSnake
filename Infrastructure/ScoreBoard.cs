﻿using System;
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
        private readonly string _highScoresStoragePath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly SpriteFont _scoreBoardFont;
        private CenteredUiFrame _uiFrame;
        private int _screenWidth;
        private int _screenHeight;
        private TextEntry _textEntry;
        private GraphicsDevice _graphicsDevice;
        public bool InHighScoreEntryMode { get; set; }

        private const int SCORE_BOARD_FONT_LEFT_PADDING = 10;

        public HighScores HighScores { get; set; }
        public bool HighScoreEntryState { get; set; }

        public ScoreBoard(string applicationPath, GraphicsDevice graphicsDevice, SpriteFont scoreBoardFont, CenteredUiFrame uiFrame, int screenWidth, int screenHeight)
        {
            _highScoresStoragePath = Path.Combine(Path.GetDirectoryName(applicationPath), HIGH_SCORES_FILE_NAME);
            _graphicsDevice = graphicsDevice;
            _scoreBoardFont = scoreBoardFont;
            _uiFrame = uiFrame;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
            // ToDo: Temporary position set on TextEntry
            _textEntry = new TextEntry(graphicsDevice, new Vector2(screenWidth /2, screenHeight /2), scoreBoardFont);

            HighScores = LoadHighScores();
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

        public void SaveHighScores()
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
            bool isNewHighScore = this.HighScores.ScoreEntries.Any(s => score > s.Score);
            return isNewHighScore;
        }

        public void KeyInput(Keys key)
        {
            _textEntry.KeyInput(key);
        }

        public void Update(GameTime gameTime)
        {
            if (HighScoreEntryState)
                _textEntry.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int scoreBoardHorizontalMargin = 20;
            int scoreBoardVerticalMargin = 20;

            int scoreVerticalSpacing = 50;
            int scoresMarginTop = 50;
            foreach (ScoreEntry scoreEntry in HighScores.ScoreEntries)
            {
                int margin = (_screenWidth - _uiFrame.ActualWidth) / 2;
                int leftInsideEdgeOfFrame = scoreBoardHorizontalMargin + margin + SCORE_BOARD_FONT_LEFT_PADDING + 40 / 2;
                int rightInsideEdgeOfFrame = _screenWidth - scoreBoardHorizontalMargin - margin - SCORE_BOARD_FONT_LEFT_PADDING - 40 / 2;

                string scoreEntryName = scoreEntry.Name ?? "";
                string scoreText = scoreEntry.Score.ToString();

                scoreEntryName = scoreEntryName.PadRight(100, ' ');
                Vector2 scoreEntryNameScale = _scoreBoardFont.MeasureString(scoreEntryName);
                Vector2 scoreEntryScoreScale = _scoreBoardFont.MeasureString(scoreText);
                Vector2 singleLetterScale = _scoreBoardFont.MeasureString("A");

                rightInsideEdgeOfFrame = rightInsideEdgeOfFrame - (int)scoreEntryScoreScale.X;

                float scoreEntryNameX = leftInsideEdgeOfFrame;
                var scoreEntryIndex = HighScores.ScoreEntries.IndexOf(scoreEntry);
                var frameYPosition = _uiFrame.Position.Y;
                float scoreEntryNameY = scoreBoardVerticalMargin + frameYPosition + scoresMarginTop + scoreVerticalSpacing + scoreEntryIndex * scoreVerticalSpacing;
                float scoreEntryScoreX = rightInsideEdgeOfFrame;
                float scoreEntryScoreY = scoreBoardVerticalMargin + frameYPosition + scoresMarginTop + scoreVerticalSpacing + scoreEntryIndex * scoreVerticalSpacing;

                Vector2 scoreEntryNamePosition = new Vector2(scoreEntryNameX, scoreEntryNameY);
                Vector2 scoreEntryScorePosition = new Vector2(scoreEntryScoreX, scoreEntryScoreY);

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

                if (scoreEntry.Score > 0)
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

            if(HighScoreEntryState)
                _textEntry.Draw(spriteBatch, gameTime);
        }
    }
}
