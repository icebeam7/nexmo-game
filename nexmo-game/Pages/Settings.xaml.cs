using nexmo_game.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace nexmo_game.Pages
{
    public sealed partial class Settings : Page
    {
        public Country SelectedCountry { get; set; }

        public Settings()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            txtName.Text = App.player.Nick;
            LoadCountries();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            App.player = await App.methods.UpdatePlayerInfo(txtName.Text, SelectedCountry.Code);
            Utils.GoBack(this.Frame);
        }

        void LoadCountries()
        {
            (btn.Flyout as ListPickerFlyout).ItemsSource = App.countries;
            txb2.Text = "Choose your country";
            LoadData();
        }

        void LoadData()
        {
            SelectedCountry = App.methods.GetPlayerCountry();
            CountryPicker.SelectedItem = SelectedCountry;
            ShowCountry();
        }

        void ShowCountry()
        {
            img1.Source = Utils.GetBitmap(SelectedCountry.FileName);
            txb2.Text = SelectedCountry.Name;
        }

        private void CountryPicker_ItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            SelectedCountry = sender.SelectedItem as Country;
            ShowCountry();
        }
    }
}
