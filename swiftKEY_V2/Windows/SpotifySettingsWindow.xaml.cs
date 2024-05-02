using swiftKEY_V2.Utils;
using System;
using System.Linq;
using System.Windows;

namespace swiftKEY_V2
{
    public partial class SpotifySettingsWindow : Window
    {
        private SpotifyAuthentificator spotifyAuth = new SpotifyAuthentificator();
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
            txt_ButtonName.Text = config.ButtonConfigurations[pressedBtnIndex].Name;
            label_buttonAction.Content = config.ButtonConfigurations[pressedBtnIndex].Title;
            txt_ClientID.Text = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientID").Value;
            txt_ClientSecret.Text = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientSecret").Value;
            txt_RedirectUri.Text = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RedirectUri").Value;
            spotifyAuth.InitializeAuth();
        }

        private void ButtonName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadConfig();
            config.ButtonConfigurations[btnIndex].Name = txt_ButtonName.Text;
            ConfigManager.SaveConfig(config);
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
            spotifyAuth.login();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadConfig();
            config.ButtonConfigurations[btnIndex].Title = "Button" + (btnIndex + 1);
            config.ButtonConfigurations[btnIndex].Name = "";
            config.ButtonConfigurations[btnIndex].Function = "";
            ConfigManager.SaveConfig(config);

            if (closingInProgress)
                return;

            Close();
        }

        #region HandleClose
        private void ModalWindow_Deactivated(object sender, EventArgs e)
        {
            if (closingInProgress)
                return;

            if (txt_ButtonName.Text.Length == 0)
                txt_ButtonName.Text = config.ButtonConfigurations[btnIndex].Title;

            Close();
        }

        private void ModalWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closingInProgress = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_ButtonName.Text.Length == 0)
                txt_ButtonName.Text = config.ButtonConfigurations[btnIndex].Title;

            Close();
        }
        #endregion
    }
}
