using SQLite;

namespace nexmo_game.Classes
{
    [Table("Carriers")]
    public class Carriers
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string network { get; set; }

        public string country { get; set; }

        public string mcc { get; set; }

        public string iso { get; set; }

        public string country_code { get; set; }

        public string mnc { get; set; }
    }
}
