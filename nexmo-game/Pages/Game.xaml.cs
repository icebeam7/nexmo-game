using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using nexmo_game.Classes;

namespace nexmo_game.Pages
{
    public sealed partial class Game : Page
    {
        Question question;

        public Game()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await GenerateQuestion();
        }

        private async Task GenerateQuestion()
        {
            if (Utils.IsInternetAvailable)
            {
                mainContainer.Visibility = Visibility.Collapsed;
                bsyMessage.IsBusy = true;

                question = await App.methods.GenerateQuestion();

                if (question == null)
                {
                    await Utils.ShowMessage("The question could not be generated. Add more contacts with valid numbers", "Error");
                    return;
                }

                txtNumber.Text = (question.Type != 5) ? question.Contact.PhoneNumber : "";
                txtCorrectAnswer.Text = question.Options[question.CorrectOption];
                
                txtName.Text = question.Contact.Name;
                txtMessage.Foreground = new SolidColorBrush(Colors.White); 
                txtMessage.Text = GetMessage(question.Type);
                imgContact.Source = Utils.GetBitmap(question.Contact.Thumbnail);
                rbn0.Content = question.Options[0];
                rbn1.Content = question.Options[1];
                rbn2.Content = question.Options[2];
                rbn3.Content = question.Options[3];

                EnableRadioButton(rbn0, true);
                EnableRadioButton(rbn1, true);
                EnableRadioButton(rbn2, true);
                EnableRadioButton(rbn3, true);

                ShowHideTextBlock(txtCorrectAnswer, false, "");

                bsyMessage.IsBusy = false;
                mainContainer.Visibility = Visibility.Visible;
            }
            else
            {
                await Utils.ShowMessageNoInternetConnection();
                Utils.GoBack(this.Frame);
            }
        }

        private string GetMessage(int type)
        {
            switch (type)
            {
                case 1: return "Which carrier does your contact use?";
                case 2: return "Which kind of network does the number use?";
                case 3: return "Which country prefix does the number use?";
                case 4: return "In which country has the number been registered?";
                case 5: return "Which is your contact's phone number?";
            }

            return "";
        }

        private void ShowHideTextBlock(TextBlock txb, bool show, string message)
        {
            txb.Text = (show) ? message : "";
        }

        private void EnableRadioButton(RadioButton rbn, bool enabled)
        {
            rbn.IsEnabled = enabled;

            if (enabled)
                rbn.IsChecked = false;
        }

        private void rbn3_Checked(object sender, RoutedEventArgs e)
        {
            CheckOption(3);
        }

        private void rbn2_Checked(object sender, RoutedEventArgs e)
        {
            CheckOption(2);
        }

        private void rbn1_Checked(object sender, RoutedEventArgs e)
        {
            CheckOption(1);
        }

        private void rbn0_Checked(object sender, RoutedEventArgs e)
        {
            CheckOption(0);
        }

        private async void CheckOption(int option)
        {
            if (App.methods.IsCorrect(question, option))
            {
                ShowHideTextBlock(txtMessage, true, "Correct!");
                txtMessage.Foreground = new SolidColorBrush(Colors.Green);
                ShowHideTextBlock(txtCorrectAnswer, true, "You really know your contact!");
                App.player = await App.methods.UpdateCorrect();
            }
            else
            {
                ShowHideTextBlock(txtMessage, true, "Wrong!");
                txtMessage.Foreground = new SolidColorBrush(Colors.Red);
                ShowHideTextBlock(txtCorrectAnswer, true, "Correct answer: " + question.Options[question.CorrectOption]);
            }

            EnableRadioButton(rbn0, false);
            EnableRadioButton(rbn1, false);
            EnableRadioButton(rbn2, false);
            EnableRadioButton(rbn3, false);
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            await GenerateQuestion();
        }
    }
}