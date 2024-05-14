using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace swiftKEY_V2
{
    public partial class OpenWebsiteSettingsWindow : Window
    {
        private ButtonConfig config;

        private int btnIndex;

        private bool closingInProgress = false;
        private bool choosingPath = false;
        private bool openFolder;

        public OpenWebsiteSettingsWindow(int pressedBtnIndex)
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
            txt_URL.Text = config.ButtonConfigurations[pressedBtnIndex].Function.Replace("openwebsite_", "");
            label_buttonAction.Content = config.ButtonConfigurations[pressedBtnIndex].Title;
        }

        private void ButtonName_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadConfig();
            config.ButtonConfigurations[btnIndex].Name = txt_ButtonName.Text;
            ConfigManager.SaveConfig(config);
        }

        private void URL_TextChanged(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.LoadConfig();
            config.ButtonConfigurations[btnIndex].Function = "openwebsite_" + txt_URL.Text;
            ConfigManager.SaveConfig(config);
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
