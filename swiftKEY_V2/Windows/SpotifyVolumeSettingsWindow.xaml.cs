﻿using System;
using System.Windows;

namespace swiftKEY_V2
{
    public partial class SpotifyVolumeSettingsWindow : Window
    {
        private ProfileConfig config;
        private int btnIndex;
        private int selectedProfile;
        private bool closingInProgress = false;

        public SpotifyVolumeSettingsWindow(int pressedBtnIndex, int selectedProfile)
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
            string[] splitFunction = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[pressedBtnIndex].Function.Split('_');
            if (splitFunction.Length > 1)
                slider_VolumeChange.Value = double.Parse(splitFunction[1]);
            else slider_VolumeChange.Value = 0;
        }

        private void ButtonName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Name = txt_ButtonName.Text;
            ConfigManager.SaveConfig(config);
        }

        private void VolumeChange_ValueChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            string[] splitFunction = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function.Split('_');
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function = splitFunction[0] + "_" + slider_VolumeChange.Value;
            ConfigManager.SaveConfig(config);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            SpotifySettingsWindow spotifySettingsWindow = new SpotifySettingsWindow();
            spotifySettingsWindow.ShowDialog();
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