using SQLite;

namespace nexmo_game.Classes
{
    [Table("Network")]
    public class Network
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string network { get; set; }
    }
}