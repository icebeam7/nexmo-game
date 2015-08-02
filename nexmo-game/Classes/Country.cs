using SQLite;
using System;

namespace nexmo_game.Classes
{
    [Table("Country")]

    public class Country
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        [Ignore]
        public string FileName
        {
            get
            {
                return String.Format("ms-appx:///Images/Countries/{0}.png", Code);
            }
        }

        public override string ToString()
        {
            return Name + " " + Code;
        }

    }
}
