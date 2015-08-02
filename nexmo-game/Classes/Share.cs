using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;

namespace nexmo_game.Classes
{
    public class Share
    {
        DataTransferManager dtManager;
        string title;
        public string description;
        string url;

        public Share(string description)
        {
            this.title = AppInfo.Name;
            this.description = description;
            this.url = AppInfo.Link; ;

            dtManager = DataTransferManager.GetForCurrentView();
            dtManager.DataRequested += dtManager_DataRequested;
        }

        public void Kill()
        {
            dtManager.DataRequested -= dtManager_DataRequested;
        }

        private void dtManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            e.Request.Data.Properties.Title = title;
            e.Request.Data.Properties.Description = description + " " + url;
            e.Request.Data.Properties.ApplicationName = AppInfo.Name;
            e.Request.Data.Properties.Square30x30Logo = RandomAccessStreamReference.CreateFromUri(new Uri(AppInfo.Logo));
            e.Request.Data.Properties.LogoBackgroundColor = Colors.LightGreen;
            e.Request.Data.SetText(description + " " + url);
            //e.Request.Data.SetWebLink(new Uri(url));
        }

        public void Show()
        {
            DataTransferManager.ShowShareUI();
        }

        public static async void SendEmail()
        {
            EmailRecipient to = new EmailRecipient(AppInfo.Email, AppInfo.PublisherName);
            EmailMessage mail = new EmailMessage();
            mail.Subject = "About " + AppInfo.Name;
            mail.Body = "";
            mail.To.Add(to);

            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        public async void SeeAllApps()
        {
            await Launcher.LaunchUriAsync(new Uri(String.Format("ms-windows-store:search?keyword={0}&contenttype=app", AppInfo.PublisherID)));
        }
    }
}