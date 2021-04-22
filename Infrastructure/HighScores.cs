using System.Collections.Generic;
using System.Linq;

namespace MonoSnake.Infrastructure
{
    public class HighScores
    {
        public bool HighScoresUpdated { get; set; }
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
                HighScoresUpdated = true;
            }

            // Keep only the top 10 High Scores
            ScoreEntries = Top10OrderedScoreEntries().ToList();
        }
    }
}