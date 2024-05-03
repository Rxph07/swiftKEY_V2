using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace swiftKEY_V2
{
    public partial class SpotifyVolumeSettingsWindow : Window
    {
        private ButtonConfig config;
        private int btnIndex;
        private bool closingInProgress = false;

        public SpotifyVolumeSettingsWindow(int pressedBtnIndex)
        {
            InitializeComponent();
            btnIndex = pressedBtnIndex;
            config = ConfigManager.LoadConfig();

            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Deactivated += ModalWindow_Deactivated;
            Closing += ModalWindow_Closing;
            txt_ButtonName.Text = config.ButtonConfigurations[pressedBtnIndex].Name;
            label_buttonAction.Content = config.ButtonConfigurations[pressedBtnIndex].Title;
            string[] splitFunction = config.ButtonConfigurations[pressedBtnIndex].Function.Split('_');
            if (splitFunction.Length > 1)
                slider_VolumeChange.Value = double.Parse(splitFunction[1]);
            else slider_VolumeChange.Value = 0;
        }

        private void ButtonName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadConfig();
            config.ButtonConfigurations[btnIndex].Name = txt_ButtonName.Text;
            ConfigManager.SaveConfig(config);
        }

        private void VolumeChange_ValueChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadConfig();
            string[] splitFunction = config.ButtonConfigurations[btnIndex].Function.Split('_');
            config.ButtonConfigurations[btnIndex].Function = splitFunction[0] + "_" + slider_VolumeChange.Value;
            ConfigManager.SaveConfig(config);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            SpotifySettingsWindow spotifySettingsWindow = new SpotifySettingsWindow(btnIndex);
            spotifySettingsWindow.ShowDialog();
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
