using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Newtonsoft.Json.Linq;

namespace nexmo_game.Classes
{
    public class Methods
    {
        private Database db;
        private Random random;

        public Methods()
        {
            
        }

        public async Task MethodsInit()
        {
            random = new Random();

            db = new Database();
            await db.Init();
        }

        public async Task<Player> GetPlayer()
        {
            Player player = await db.conn.Table<Player>().FirstOrDefaultAsync();
            return (player != null) ? player : await db.CreatePlayer();
        }

        public async Task<List<Country>> GetCountries()
        {
            var list = await db.conn.Table<Country>().ToListAsync();
            return (list.Count > 0) ? list : await db.CreateCountries();
        }

        public async Task<List<Network>> GetNetworks()
        {
            var list = await db.conn.Table<Network>().ToListAsync();
            return (list.Count > 0) ? list : await db.CreateNetworks();
        }

        public async Task<List<Carriers>> GetCarriers()
        {
            var list = await db.conn.Table<Carriers>().ToListAsync();
            return (list.Count > 0) ? list : await db.CreateCarriers();
        }

        public string GetPlayerMessage()
        {
            return string.Format("Hello, {0}!", App.player.Nick);
        }

        public async Task<Player> UpdatePlayerInfo(string nick, string country)
        {
            Player player = await GetPlayer();
            player.Nick = nick;
            player.Country = country;

            await db.conn.UpdateAsync(player);
            return player;
        }

        public Country GetPlayerCountry()
        {
            return App.countries.Where(x => x.Code == App.player.Country).FirstOrDefault();
        }

        public Carriers GetCarrier(string code)
        {
            return App.carriers.Where(x => x.mcc == code.Substring(0, 3) && x.mnc == code.Substring(3)).FirstOrDefault();
        }

        public Carriers GetRandomCarrier()
        {
            int random = App.random.Next(1, App.carriers.Count);
            return App.carriers.Where(x => x.Id == random).FirstOrDefault();
        }

        public async Task<List<LocalContact>> GetLocalContacts()
        {
            ContactStore store = await ContactManager.RequestStoreAsync();
            IReadOnlyList<Contact> contacts = await store.FindContactsAsync();
            var contactsList = contacts.ToList();
            List<LocalContact> localContacts = new List<LocalContact>();

            foreach (var contact in contactsList)
            {
                localContacts.Add(new LocalContact() 
                {
                    Name = contact.FirstName + " " + contact.LastName,
                    PhoneNumber = GetPhoneNumber(contact),
                    Thumbnail = GetThumbnail(contact)
                });
            }

            return localContacts;
        }

        public string GetRandomContactNumber()
        {
            int aleat = Utils.GetRandomValue(App.contacts.Count);
            return App.contacts[aleat].PhoneNumber;
        }

        public LocalContact GetRandomContact()
        {
            int i = 0;
            LocalContact contacto;

            do
            {
                contacto = App.contacts[Utils.GetRandomValue(App.contacts.Count)];

                if (contacto.PhoneNumber == "")
                {
                    i++;
                    continue;
                }

                contacto.NexmoInfo = GetNexmoInfo(contacto.PhoneNumber, App.player.Country);

                if (contacto.NexmoInfo == null)
                {
                    i++;
                    continue;
                }
            } while (i < 5);

            return contacto;
        }

        private NexmoInfo GetNexmoInfo(string phone, string country)
        {
            NexmoFormat format = (NexmoFormat)GetNexmoFormatLookup(phone, country, "format");

            if (format == null)
                return null;

            NexmoLookup lookup = (NexmoLookup)GetNexmoFormatLookup(phone, country, "lookup");

            if (lookup == null)
                return null;

            return new NexmoInfo() { NexmoFormat = format, NexmoLookup = lookup };
        }

        private INexmo GetNexmoFormatLookup(string phone, string country, string service)
        {
            INexmo nexmo = RequestNumberInsight(phone, "", service);

            if (nexmo == null)
                nexmo = RequestNumberInsight(phone, country, service);

            return nexmo;
        }

        private INexmo RequestNumberInsight(string phone, string country, string service)
        {
            string extraParam = (country != "") ? String.Format("&country={0}", country) : "";
            string url = String.Format("https://api.nexmo.com/number/{4}/json?api_key={0}&api_secret={1}&number={2}{3}", 
                AppInfo.nexmoApiKey, AppInfo.nexmoApiSecret, phone, extraParam, service);

            JObject datos = Utils.RequestJson(url);

            if ((string)datos["status"] == "0") // code: success
            {
                if (service == "format")
                {
                    return new NexmoFormat()
                    {
                        InternationalNumber = (string)datos["international_format_number"],
                        Country = (string)datos["country_name"],
                        Prefix = (string)datos["country_prefix"]
                    };
                }
                else
                {
                    return new NexmoLookup()
                    {
                        Carrier = GetCarrier((string)datos["current_carrier"]["network_code"]).network,
                        Network = (string)datos["current_carrier"]["network_type"]
                    };
                }
            }
            else
            {
                return null;
            }
        }

        private string GetPhoneNumber(Contact contact)
        {
            return (contact.Phones.Count > 0) ? contact.Phones[0].Number : "";
        }

        private string GetThumbnail(Contact contact)
        {
            return (contact.Thumbnail != null) ? ((StorageFile)contact.Thumbnail).Path : "ms-appx:///Images/default.png";
        }

        public Network GetRandomNetwork()
        {
            int random = App.random.Next(1, App.networks.Count);
            return App.networks.Where(x => x.Id == random).FirstOrDefault();
        }
        public bool IsCorrect(Question question, int option)
        {
            return option == question.CorrectOption;
        }

        public async Task<Question> GenerateQuestion()
        {
            int type = Utils.GetRandomValue(6, 1);
            int correct = Utils.GetRandomValue(4);
            var contact = GetRandomContact();

            if (contact == null)
                return null;

            Question question = new Question()
            {
                Contact = contact,
                Type = type, 
                CorrectOption = correct,
                Options = GetOptions(type, correct, contact)
            };

            App.player = await IncrementQuestion();
            return question;
        }

        private string[] GetOptions(int type, int correct, LocalContact contact)
        {
            string[] options = new string[4];

            for (int i = 0; i < 4; i++)
            {
                if (i != correct)
                {
                    string option = "";

                    do
                    {
                        option = (type == 1) ? GetRandomCarrier().network
                            : (type == 2) ? GetRandomNetwork().network
                            : (type == 3) ? GetRandomCarrier().country_code
                            : (type == 4) ? GetRandomCarrier().country
                            : GetRandomContactNumber();
                    } while (options.Contains(option));

                    options[i] = option;
                }
                else
                {
                    options[i] = (type == 1) ? contact.NexmoInfo.NexmoLookup.Carrier
                            : (type == 2) ? contact.NexmoInfo.NexmoLookup.Network
                            : (type == 3) ? contact.NexmoInfo.NexmoFormat.Prefix
                            : (type == 4) ? contact.NexmoInfo.NexmoFormat.Country
                            : contact.PhoneNumber;
                }
            }

            return options;
        }

        public async Task<Player> UpdateCorrect()
        {
            Player player = await GetPlayer();
            player.Correct++;

            await db.conn.UpdateAsync(player);
            return player;
        }

        public async Task<Player> IncrementQuestion()
        {
            Player player = await GetPlayer();
            player.Questions++;

            await db.conn.UpdateAsync(player);
            return player;
        }
    }
}
