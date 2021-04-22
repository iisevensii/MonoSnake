namespace MonoSnake.Infrastructure
{
    public class ScoreEntry
    {
        // Property setters needed for deserialization
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