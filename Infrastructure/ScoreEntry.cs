namespace MonoSnake.Infrastructure
{
    public class ScoreEntry
    {
        public string Name { get; }
        public int Score { get; }

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