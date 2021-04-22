using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MonoSnake.Infrastructure
{
    public class ScoreBoard
    {
        private const string HIGH_SCORES_FILE_NAME = "HighScores.json";
        private readonly string _highScoresStoragePath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public HighScores HighScores { get; set; }

        public ScoreBoard(string applicationPath)
        {
            _highScoresStoragePath = Path.Combine(Path.GetDirectoryName(applicationPath), HIGH_SCORES_FILE_NAME);
            _jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
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
    }
}
