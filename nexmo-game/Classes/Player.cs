using SQLite;

namespace nexmo_game.Classes
{
    [Table("Player")]
    public class Player
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Nick { get; set; }

        public string Country { get; set; }

        public int Questions { get; set; }

        public int Correct { get; set; }
    }
}
