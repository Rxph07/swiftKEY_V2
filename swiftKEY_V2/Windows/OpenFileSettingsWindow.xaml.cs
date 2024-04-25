﻿using Ookii.Dialogs.Wpf;
using System;
using System.Windows;

namespace swiftKEY_V2
{
    public partial class OpenFileSettingsWindow : Window
    {
        private ButtonConfig config;

        private int btnIndex;

        private bool closingInProgress = false;
        private bool choosingPath = false;
        private bool openFolder;

        public OpenFileSettingsWindow(int pressedBtnIndex)
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
            if (config.ButtonConfigurations[pressedBtnIndex].Function == "openfile"
                || config.ButtonConfigurations[pressedBtnIndex].Function.Contains("openfile_"))
            {
                openFolder = false;
                if(config.ButtonConfigurations[pressedBtnIndex].Function == "openfile")
                    txt_FilePath.Text = "Keine Datei gewählt.";
                else
                    txt_FilePath.Text = config.ButtonConfigurations[pressedBtnIndex].Function.Replace("openfile_", "");
            }
            else if (config.ButtonConfigurations[pressedBtnIndex].Function == "openfolder"
                || config.ButtonConfigurations[pressedBtnIndex].Function.Contains("openfolder_"))
            {
                openFolder = true;
                if (config.ButtonConfigurations[pressedBtnIndex].Function == "openfolder")
                    txt_FilePath.Text = "Kein Ordner gewählt.";
                else
                    txt_FilePath.Text = config.ButtonConfigurations[pressedBtnIndex].Function.Replace("openfolder_", "");
            }
            label_buttonAction.Content = config.ButtonConfigurations[pressedBtnIndex].Title;
        }

        private void ButtonName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadConfig();
            config.ButtonConfigurations[btnIndex].Name = txt_ButtonName.Text;
            ConfigManager.SaveConfig(config);
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            choosingPath = true;
            Dispatcher.Invoke(() =>
            {
                if (openFolder == true)
                {
                    VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
                    folderDialog.Multiselect = false;

                    bool? result = folderDialog.ShowDialog();
                    if (result == true)
                    {
                        txt_FilePath.Text = folderDialog.SelectedPath;
                        config = ConfigManager.LoadConfig();
                        config.ButtonConfigurations[btnIndex].Function = "openfolder_" + folderDialog.SelectedPath;
                        ConfigManager.SaveConfig(config);
                    }
                }
                else
                {
                    VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
                    fileDialog.Filter = "All files (*.*)|*.*";
                    fileDialog.Multiselect = false;

                    if (fileDialog.ShowDialog() == true)
                    {
                        txt_FilePath.Text = fileDialog.FileName;

                        config = ConfigManager.LoadConfig();
                        config.ButtonConfigurations[btnIndex].Function = "openfile_" + fileDialog.FileName;
                        ConfigManager.SaveConfig(config);
                    }
                }
            });
            choosingPath = false;
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
                txt_ButtonName.Text = config.ButtonConfigurations[btnIndex].Title;

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
                txt_ButtonName.Text = config.ButtonConfigurations[btnIndex].Title;

            Close();
        }
        #endregion
    }
}
