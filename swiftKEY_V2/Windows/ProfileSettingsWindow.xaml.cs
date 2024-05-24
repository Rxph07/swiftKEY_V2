using System;
using System.Windows;

namespace swiftKEY_V2
{
    public partial class ProfileSettingsWindow : Window
    {
        private ProfileConfig config;
        private int selectedProfile;
        private bool closingInProgress = false;
        private MainWindow mainWindow;

        public ProfileSettingsWindow(int selectedProfile, MainWindow mainWindow)
        {
            InitializeComponent();
            this.selectedProfile = selectedProfile;
            this.mainWindow = mainWindow;
            config = ConfigManager.LoadProfileConfig();

            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Deactivated += ModalWindow_Deactivated;
            Closing += ModalWindow_Closing;
            txt_ProfileName.Text = config.ProfileConfigurations[selectedProfile].Name;
            label_profileTitle.Content = config.ProfileConfigurations[selectedProfile].Title;
        }

        private void ProfileName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            config.ProfileConfigurations[selectedProfile].Name = txt_ProfileName.Text;
            ConfigManager.SaveConfig(config);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            ConfigManager.RemoveProfile(config.ProfileConfigurations[selectedProfile].Title, mainWindow);
            MainWindow.SetSelectedProfile(0);

            if (closingInProgress)
                return;

            Close();
        }

        #region HandleClose
        private void ModalWindow_Deactivated(object sender, EventArgs e)
        {
            if (closingInProgress)
                return;

            if (txt_ProfileName.Text.Length == 0)
                txt_ProfileName.Text = config.ProfileConfigurations[selectedProfile].Title;

            Close();
        }

        private void ModalWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closingInProgress = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_ProfileName.Text.Length == 0)
                txt_ProfileName.Text = config.ProfileConfigurations[selectedProfile].Title;

            Close();
        }
        #endregion
    }
}