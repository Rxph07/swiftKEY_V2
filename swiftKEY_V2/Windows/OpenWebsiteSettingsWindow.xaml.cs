using System;
using System.Windows;

namespace swiftKEY_V2
{
    public partial class OpenWebsiteSettingsWindow : Window
    {
        private ProfileConfig config;

        private int btnIndex;
        private int selectedProfile;

        private bool closingInProgress = false;
        private bool choosingPath = false;

        public OpenWebsiteSettingsWindow(int pressedBtnIndex, int selectedProfile)
        {
            InitializeComponent();
            btnIndex = pressedBtnIndex;
            this.selectedProfile = selectedProfile;
            config = ConfigManager.LoadProfileConfig();

            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Deactivated += ModalWindow_Deactivated;
            Closing += ModalWindow_Closing;
            txt_ButtonName.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[pressedBtnIndex].Name;
            txt_URL.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[pressedBtnIndex].Function.Replace("openwebsite_", "");
            label_buttonAction.Content = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[pressedBtnIndex].Title;
        }

        private void ButtonName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Name = txt_ButtonName.Text;
            ConfigManager.SaveConfig(config);
        }

        private void URL_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function = "openwebsite_" + txt_URL.Text;
            ConfigManager.SaveConfig(config);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Title = "Button" + (btnIndex + 1);
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Name = "";
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function = "";
            ConfigManager.SaveConfig(config);

            if (closingInProgress)
                return;

            if (choosingPath)
                return;

            Close();
        }

        #region HandleClose
        private void ModalWindow_Deactivated(object sender, EventArgs e)
        {
            if (closingInProgress)
                return;

            if (choosingPath)
                return;

            if (txt_ButtonName.Text.Length == 0)
                txt_ButtonName.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Title;

            Close();
        }

        private void ModalWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closingInProgress = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (choosingPath)
                return;

            if (txt_ButtonName.Text.Length == 0)
                txt_ButtonName.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Title;

            Close();
        }
        #endregion
    }
}