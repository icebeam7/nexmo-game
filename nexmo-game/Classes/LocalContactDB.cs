using SQLite;

namespace nexmo_game.Classes
{
    [Table("LocalContactDB")]
    public class LocalContactDB
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public string PhoneNumber { get; set; }

        public string Country { get; set; }

        public string Prefix { get; set; }

        public string InternationalNumber { get; set; }

        public string Carrier { get; set; }

        public string Network { get; set; }
    }
}