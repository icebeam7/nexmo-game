using nexmo_game.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


namespace nexmo_game.Pages
{
    public sealed partial class About : Page
    {
        Share share;

        public About()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            share = new Share(AppInfo.Motto);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (share != null)
                share.Kill();
        }


        private void txtEmail_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Share.SendEmail();
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            share.Show();
        }

        private void btnAllApps_Click(object sender, RoutedEventArgs e)
        {
            share.SeeAllApps();
        }
    }
}
