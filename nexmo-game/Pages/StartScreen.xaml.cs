using nexmo_game.Classes;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace nexmo_game.Pages
{
    public sealed partial class StartScreen : Page
    {
        public StartScreen()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            ShowHide(false);

            if (App.methods == null)
                await Utils.InitializeMethods();

            if (App.player != null)
                txtTitle.Text = App.methods.GetPlayerMessage();

            ShowHide(true);
        }

        void ShowHide(bool showMain)
        {
            main.Visibility = showMain ? Visibility.Visible : Visibility.Collapsed;
            bsyMessage.IsBusy = !showMain;
            cmdBar.IsEnabled = showMain;
        }

        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.IsInternetAvailable)
                Frame.Navigate(typeof(Game));
            else
                await Utils.ShowMessageNoInternetConnection();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Statistics));
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }

        private void btnFeedback_Click(object sender, RoutedEventArgs e)
        {
            Share.SendEmail();
        }
    }
}