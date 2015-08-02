namespace nexmo_game.Classes
{
    public class Question
    {
        public int Type { get; set; }

        public int CorrectOption { get; set; }

        public string[] Options { get; set; }

        public LocalContact Contact { get; set; }
    }
}
