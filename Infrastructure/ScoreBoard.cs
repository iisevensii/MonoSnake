using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MonoSnake.Infrastructure
{
    public class ScoreBoard
    {
        private const string HIGH_SCORE_FILE_NAME = "HighScores.json";
        private string _applicationPath;
        private string _highScoreStoragePath;
        public int AllTimeHighScore { get; set; }
        public int CurrentScore { get; set; }
        public HighScores HighScores { get; set; }

        public ScoreBoard(string applicationPath)
        {
            _applicationPath = applicationPath;
            _highScoreStoragePath = Path.Combine(Path.GetDirectoryName(_applicationPath), HIGH_SCORE_FILE_NAME);
            HighScores = LoadHighScores();
        }

        public HighScores LoadHighScores()
        {
            if (File.Exists(_highScoreStoragePath))
            {
                HighScores = JsonSerializer.Deserialize<HighScores>(File.ReadAllText(_highScoreStoragePath));

                return HighScores;
            }

            return new HighScores();
        }

        public void SaveHighScores()
        {
            string highScoresSerialized = JsonSerializer.Serialize(HighScores);


            File.WriteAllText(_highScoreStoragePath, highScoresSerialized);
        }
    }

    public class HighScores
    {
        public List<ScoreEntry> ScoreEntries { get; set; } = new List<ScoreEntry>();

        private IEnumerable<ScoreEntry> Top10OrderedScoreEntries()
        {
            return ScoreEntries.OrderByDescending(s => s.Score).Take(10);
        }

        public void AddHighScore(ScoreEntry scoreEntry)
        {
            if (!ScoreEntries.Any() || ScoreEntries.Any(se => scoreEntry.Score > se.Score))
            {
                // Add new High Score
                ScoreEntries.Add(scoreEntry);
            }

            // Keep only the top 10 High Scores
            ScoreEntries = Top10OrderedScoreEntries().ToList();
        }
}

    public class ScoreEntry
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public ScoreEntry()
        {
            
        }

        public ScoreEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
