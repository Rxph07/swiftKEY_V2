using swiftKEY_V2.Utils;
using System;
using System.Linq;
using System.Windows;

namespace swiftKEY_V2
{
    public partial class SpotifySettingsWindow : Window
    {
        private ButtonConfig config;
        private SpotifyConfig spotifyConfig;
        private int btnIndex;
        private bool closingInProgress = false;

        public SpotifySettingsWindow(int pressedBtnIndex)
        {
            InitializeComponent();
            btnIndex = pressedBtnIndex;
            config = ConfigManager.LoadConfig();
            spotifyConfig = ConfigManager.LoadSpotifyConfig();

            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Deactivated += ModalWindow_Deactivated;
            Closing += ModalWindow_Closing;
            txt_ClientID.Text = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientID").Value;
            txt_ClientSecret.Text = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientSecret").Value;
            txt_RedirectUri.Text = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RedirectUri").Value;
        }

        private void ClientID_TextChanged(object sender, RoutedEventArgs e)
        {
            spotifyConfig = ConfigManager.LoadSpotifyConfig();
            spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientID").Value = txt_ClientID.Text;
            ConfigManager.SaveConfig(spotifyConfig);
        }

        private void ClientSecret_TextChanged(object sender, RoutedEventArgs e)
        {
            spotifyConfig = ConfigManager.LoadSpotifyConfig();
            spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientSecret").Value = txt_ClientSecret.Text;
            ConfigManager.SaveConfig(spotifyConfig);
        }

        private void RedirectUri_TextChanged(object sender, RoutedEventArgs e)
        {
            spotifyConfig = ConfigManager.LoadSpotifyConfig();
            spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RedirectUri").Value = txt_RedirectUri.Text;
            ConfigManager.SaveConfig(spotifyConfig);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.spotifyAuth.login();
        }

        #region HandleClose
        private void ModalWindow_Deactivated(object sender, EventArgs e)
        {
            if (closingInProgress)
                return;

            Close();
        }

        private void ModalWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closingInProgress = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
