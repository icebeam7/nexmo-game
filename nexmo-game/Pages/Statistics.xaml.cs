using nexmo_game.Classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace nexmo_game.Pages
{
    public sealed partial class Statistics : Page
    {
        Share share;

        public Statistics()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            share = new Share(String.Format("My score in #NexmoQuiz for #WindowsPhone is {0} of {1}. Can you beat me? Download it now!",
                App.player.Correct, App.player.Questions));

            txtQuestions.Text = App.player.Questions.ToString();
            txtCorrect.Text = App.player.Correct.ToString();
            txtWrong.Text = (App.player.Questions - App.player.Correct).ToString();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (share != null)
                share.Kill();
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            if (share != null)
                share.Show();
        }
    }
}
