namespace MonoSnake.Infrastructure
{
    public class ScoreEntry
    {
        // Property setters needed for deserialization
        public bool IsNew { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        /// <summary>
        /// Required for serialization
        /// </summary>
        public ScoreEntry()
        {
            
        }

        public ScoreEntry(bool isNew, string name, int score)
        {
            IsNew = isNew;
            Name = name;
            Score = score;
        }
    }
}