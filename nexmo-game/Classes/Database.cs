using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace nexmo_game.Classes
{
    public class Database
    {
        private const string DBNAME = "NexmoQuiz.db";

        public SQLiteAsyncConnection conn { get; set; }

        public Database()
        {
        }

        public async Task Init()
        {
            conn = new SQLiteAsyncConnection(DBNAME);
            await this.InitDb();
        }

        public async Task InitDb()
        {
            bool dbExist = await CheckDbAsync();

            if (!dbExist)
                await CreateDatabaseAsync();
        }

        public async Task<bool> CheckDbAsync()
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(DBNAME);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        private async Task CreateDatabaseAsync()
        {
            await conn.CreateTableAsync<Player>();
            await conn.CreateTableAsync<Carriers>();
            await conn.CreateTableAsync<Country>();
            await conn.CreateTableAsync<Network>();
            await conn.CreateTableAsync<LocalContactDB>();
            
            /*App.player = await CreatePlayer();
            App.carriers = await CreateCarriers();
            App.countries = await CreateCountries();
            App.networks = await CreateNetworks();*/
        }

        public async Task<Player> CreatePlayer()
        {
            Player player = new Player()
            {
                Id = Guid.NewGuid().ToString(),
                Nick = "player",
                Country = "US",
                Questions = 0,
                Correct = 0
            };

            await conn.InsertAsync(player);
            return player;
        }

        public async Task<List<Carriers>> CreateCarriers()
        {
            var list = await Utils.GetCarriers();

            int i = 1;

            foreach (var item in list)
            {
                item.Id = i++;
            }

            await conn.InsertAllAsync(list);
            return list;
        }

        public async Task<List<Country>> CreateCountries()
        {
            var list = await Utils.GetCountries();

            int i = 1;

            foreach (var item in list)
            {
                item.Id = i++;
            }

            await conn.InsertAllAsync(list);
            return list;
        }

        public async Task<List<Network>> CreateNetworks()
        {
            var list = await Utils.GetNetworks();
            await conn.InsertAllAsync(list);
            return list;
        }
    }
}