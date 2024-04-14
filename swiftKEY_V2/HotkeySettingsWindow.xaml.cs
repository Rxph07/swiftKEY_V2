using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput.Native;

namespace swiftKEY_V2
{
    public partial class HotkeySettingsWindow : Window
    {
        public List<string> FKeys { get; set; } = new List<string>();
        public bool NewBinding = false;

        private ButtonConfig config;
        private bool closingInProgress = false;
        private Button currentButton;
        private int hotkeyCode;
        private int specialKeyCode = -1;
        private Key specialKey = Key.None;
        private int btnIndex;

        public HotkeySettingsWindow(int pressedBtnIndex)
        {
            InitializeComponent();
            config = ConfigManager.LoadConfig();

            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Deactivated += ModalWindow_Deactivated;
            Closing += ModalWindow_Closing;
            txtButtonName.Text = config.ButtonConfigurations[pressedBtnIndex].Name;
            btnIndex = pressedBtnIndex;
        }

        private void ChooseHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Focus();
                currentButton = button;
                specialKey = Key.None;
                specialKeyCode = -1;
                PreviewKeyDown += ButtonPreviewKeyDownHandler;
                button.LostKeyboardFocus += ButtonLostKeyboardFocusHandler;
            }
        }

        private void ButtonPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (currentButton != null)
            {
                if (HandleSpecialKey(e))
                    return;

                hotkeyCode = KeyInterop.VirtualKeyFromKey(e.Key);
                if (specialKeyCode != -1 && specialKey != Key.None)
                {
                    currentButton.Content = specialKey + " + " + e.Key;
                }
                else
                {
                    currentButton.Content = e.Key;
                }
            }
        }

        private bool HandleSpecialKey(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl
                || e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.LWin || e.Key == Key.RWin
                || e.Key == Key.Escape || e.Key == Key.Delete || e.Key == Key.Tab || e.Key == Key.Back
                || e.Key == Key.Return || e.Key == Key.Insert || e.Key == Key.Prior || e.Key == Key.Next
                || e.Key == Key.System || e.Key == Key.Capital || e.Key == Key.CapsLock)
            {
                specialKeyCode = KeyInterop.VirtualKeyFromKey(e.Key);
                currentButton.Content = e.Key + " + ";
                specialKey = e.Key;
                return true;
            }
            return false;
        }

        private void ButtonLostKeyboardFocusHandler(object sender, KeyboardFocusChangedEventArgs e)
        {
            PreviewKeyDown -= ButtonPreviewKeyDownHandler;

            if (sender is Button button)
            {
                button.LostKeyboardFocus -= ButtonLostKeyboardFocusHandler;
            }

            currentButton = null;
            config.ButtonConfigurations[btnIndex].Function = ("hotkey_" + specialKeyCode + "_" + hotkeyCode).ToLower();
            ConfigManager.SaveConfig(config);
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
