using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace nexmo_game.Classes
{
    public static class Utils
    {
        public static async Task<string> ReadFile(string fileName, string fileExtension = "json") 
        {
            string fileContent;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(String.Format(@"ms-appx:///Data/{0}.{1}", fileName, fileExtension)));
            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
                fileContent = await sRead.ReadToEndAsync();

            return fileContent;
        }

        public static async Task<List<Carriers>> GetCarriers()
        {
            string json = await ReadFile(@"carriers");
            return JsonConvert.DeserializeObject<List<Carriers>>(json);
        }

        public static async Task<List<Country>> GetCountries()
        {
            string json = await ReadFile(@"countrycodes");
            return JsonConvert.DeserializeObject<List<Country>>(json);
        }

        public static async Task<List<Network>> GetNetworks()
        {
            string json = await ReadFile(@"networks");
            return JsonConvert.DeserializeObject<List<Network>>(json);
        }

        public static bool IsInternetAvailable
        {
            get
            {
                var profiles = NetworkInformation.GetConnectionProfiles();
                var internetProfile = NetworkInformation.GetInternetConnectionProfile();
                return profiles.Any(s => s.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                    || (internetProfile != null
                            && internetProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
            }
        }

        public static async Task ShowMessageNoInternetConnection()
        {
            await ShowMessage("You need to be connected to the Internet in order to use this app. Please try again.", 
                "No Internet connection available.");
        }

        public static async Task InitializeMethods()
        {
            App.methods = new Methods();
            await App.methods.MethodsInit();

            App.player = await App.methods.GetPlayer();
            App.carriers = await App.methods.GetCarriers();
            App.countries = await App.methods.GetCountries();
            App.networks = await App.methods.GetNetworks();
            App.contacts = await App.methods.GetLocalContacts();
        }

        public static void GoBack(Frame Frame)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        public static ImageSource GetBitmap(string url)
        {
            return new BitmapImage(new Uri(url));
        }

        public static int GetRandomValue(int max, int min = 0)
        {
            return App.random.Next(min, max);
        }

        public static JObject RequestJson(string url)
        {
            HttpClient httpClient = new HttpClient();
            string json = httpClient.GetStringAsync(url).Result;
            return JObject.Parse(json);
        }

        public static async Task ShowMessage(string message, string title)
        {
            MessageDialog box = new MessageDialog(message, title);
            box.Commands.Add(new UICommand { Label = "Got it", Id = 0 });
            await box.ShowAsync();
        }
    }
}
