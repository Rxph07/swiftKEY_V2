using System;
using System.Windows;
using System.Windows.Controls;

namespace swiftKEY_V2
{
    public partial class SwitchProfileSettingsWindow : Window
    {
        private ProfileConfig config;
        private int btnIndex;
        private int selectedProfile;
        private bool closingInProgress = false;

        public SwitchProfileSettingsWindow(int pressedBtnIndex, int selectedProfile)
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
            label_buttonAction.Content = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[pressedBtnIndex].Title;

            string function = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[pressedBtnIndex].Function;
            cb_Profiles.SelectedIndex = int.Parse(function.Replace("switchprofile_", ""));
            UpdateProfiles();
        }

        private void UpdateProfiles()
        {
            config = ConfigManager.LoadProfileConfig();
            cb_Profiles.Items.Clear();

            for (int i = 0; i < config.ProfileConfigurations.Count; i++)
            {
                cb_Profiles.Items.Add(config.ProfileConfigurations[i].Name);
            }
        }

        private void cb_Profiles_DropDownOpened(object sender, EventArgs e)
        {
            UpdateProfiles();
        }

        private void cb_Profiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Profiles.SelectedItem != null)
            {
                config = ConfigManager.LoadProfileConfig();
                config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function = "switchprofile_" + cb_Profiles.SelectedIndex;
                ConfigManager.SaveConfig(config);
            }
        }

        private void ButtonName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Name = txt_ButtonName.Text;
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

            Close();
        }

        #region HandleClose
        private void ModalWindow_Deactivated(object sender, EventArgs e)
        {
            if (closingInProgress)
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
            if (txt_ButtonName.Text.Length == 0)
                txt_ButtonName.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Title;

            Close();
        }
        #endregion
    }
}