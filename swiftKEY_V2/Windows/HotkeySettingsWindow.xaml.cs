﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace swiftKEY_V2
{
    public partial class HotkeySettingsWindow : Window
    {
        private ProfileConfig config;
        private Button currentButton;

        private string hotkey = "";
        private int hotkeyCode = -1;
        private int btnIndex;
        private int selectedProfile;

        private bool closingInProgress = false;
        private bool isShiftPressed = false;
        private bool isCtrlPressed = false;
        private bool isAltPressed = false;
        private bool isWinPressed = false;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;
        private static HotkeySettingsWindow _currentInstance;

        public HotkeySettingsWindow(int pressedBtnIndex, int selectedProfile)
        {
            InitializeComponent();
            _currentInstance = this;
            _proc = HookCallback;
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
            Loaded += HotkeySettingsWindow_Loaded;

            for (int i = 1; i < 25; i++)
            {
                cb_chooseHotkey.Items.Add("F" + i);
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

        private void cb_chooseHotkey_SelectionChanged(object sender, EventArgs e)
        {
            if (cb_chooseHotkey != null && cb_chooseHotkey.SelectedItem != null)
            {
                currentButton = cb_chooseHotkey.Template.FindName("btn_chooseHotkey", cb_chooseHotkey) as Button;
                if (currentButton != null)
                {
                    currentButton.Content = cb_chooseHotkey.SelectedItem.ToString();
                    config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function = "hotkey_" + GetFKeyCode(cb_chooseHotkey.SelectedItem.ToString());
                    ConfigManager.SaveConfig(config);
                    currentButton = null;
                }
            }
        }

        private void ChooseHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                currentButton = button;
                _hookID = SetHook(_proc);
                currentButton.Focus();
                isShiftPressed = false;
                isCtrlPressed = false;
                isAltPressed = false;
                isWinPressed = false;
                hotkeyCode = -1;
                KeyUp += ButtonPreviewKeyUpHandler;
                currentButton.LostKeyboardFocus += ButtonLostKeyboardFocusHandler;
                currentButton.Content = "Warte auf Eingabe...";
            }
        }

        private void ButtonPreviewKeyUpHandler(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (currentButton != null)
            {
                if (HandleSpecialKey(KeyInterop.VirtualKeyFromKey(e.Key), false))
                {
                    currentButton.Content = BuildKeyCombination(isShiftPressed, isCtrlPressed, isAltPressed, isWinPressed);
                    if (!isShiftPressed && !isCtrlPressed && !isAltPressed && !isWinPressed)
                        currentButton.Content = "Warte auf Eingabe...";
                }
            }
        }

        private bool HandleSpecialKey(int keyCode, bool value)
        {
            if (keyCode == KeyInterop.VirtualKeyFromKey(Key.LeftShift) || keyCode == KeyInterop.VirtualKeyFromKey(Key.RightShift))
            {
                isShiftPressed = value;
                return true;
            }
            else if (keyCode == KeyInterop.VirtualKeyFromKey(Key.LeftCtrl) || keyCode == KeyInterop.VirtualKeyFromKey(Key.RightCtrl))
            {
                isCtrlPressed = value;
                return true;
            }
            else if (keyCode == KeyInterop.VirtualKeyFromKey(Key.LeftAlt) || keyCode == KeyInterop.VirtualKeyFromKey(Key.RightAlt))
            {
                isAltPressed = value;
                return true;
            }
            else if (keyCode == KeyInterop.VirtualKeyFromKey(Key.LWin) || keyCode == KeyInterop.VirtualKeyFromKey(Key.RWin))
            {
                isWinPressed = value;
                return true;
            }
            return false;
        }
        private string BuildKeyCombination(bool shiftPressed, bool ctrlPressed, bool altPressed, bool winPressed)
        {
            StringBuilder builder = new StringBuilder();

            if (shiftPressed)
                builder.Append("Shift + ");
            if (ctrlPressed)
                builder.Append("Ctrl + ");
            if (altPressed)
                builder.Append("Alt + ");
            if (winPressed)
                builder.Append("Win + ");

            return builder.ToString();
        }
        private string BuildKeyCode(bool shiftPressed, bool ctrlPressed, bool altPressed, bool winPressed)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("hotkey_");
            if (shiftPressed)
                builder.Append("160_");
            if (ctrlPressed)
                builder.Append("162_");
            if (altPressed)
                builder.Append("164_");
            if (winPressed)
                builder.Append("91_");

            builder.Append(hotkeyCode);
            return builder.ToString();
        }
        private int GetFKeyCode(string keyName)
        {
            return KeyInterop.VirtualKeyFromKey(KeyInterop.KeyFromVirtualKey(KeyInterop.VirtualKeyFromKey((Key)Enum.Parse(typeof(Key), keyName))));
        }
        private string BuildKeyString()
        {
            config = ConfigManager.LoadProfileConfig();
            string[] splitFunction = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function.Split('_');
            StringBuilder builder = new StringBuilder();

            for(int i = 1; i < splitFunction.Length; i++)
            {
                if (splitFunction[i].Equals("160"))
                    builder.Append("Shift + ");
                else if (splitFunction[i].Equals("162"))
                    builder.Append("Ctrl + ");
                else if (splitFunction[i].Equals("164"))
                    builder.Append("Alt + ");
                else if (splitFunction[i].Equals("91"))
                    builder.Append("Win + ");
                else
                    builder.Append(KeyInterop.KeyFromVirtualKey(int.Parse(splitFunction[i])).ToString());
            }

            if (splitFunction[0] == "" || splitFunction.Length == 1)
            {
                return "Hotkey wählen...";
            }

            return builder.ToString();
        }
        private void FinishInput()
        {
            if (currentButton != null)
            {
                currentButton.LostKeyboardFocus -= ButtonLostKeyboardFocusHandler;
            }

            currentButton = null;
            config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Function = hotkey;
            ConfigManager.SaveConfig(config);
            UnhookWindowsHookEx(_hookID);
        }

        private void ButtonLostKeyboardFocusHandler(object sender, KeyboardFocusChangedEventArgs e)
        {
            FinishInput();
        }

        private void HotkeySettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            currentButton = cb_chooseHotkey.Template.FindName("btn_chooseHotkey", cb_chooseHotkey) as Button;
            if (currentButton != null) {
                currentButton.Content = BuildKeyString();
            }
        }

        #region Handle Keypress Supression
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //_currentInstance.currentButton.Content += KeyInterop.KeyFromVirtualKey(vkCode).ToString();
                    if (_currentInstance.currentButton != null)
                    {
                        if (_currentInstance.HandleSpecialKey(vkCode, true))
                        {
                            _currentInstance.currentButton.Content = _currentInstance.BuildKeyCombination(_currentInstance.isShiftPressed, _currentInstance.isCtrlPressed, _currentInstance.isAltPressed, _currentInstance.isWinPressed);
                        }
                        else
                        {
                            _currentInstance.hotkeyCode = vkCode;
                            _currentInstance.currentButton.Content = _currentInstance.BuildKeyCombination(_currentInstance.isShiftPressed, _currentInstance.isCtrlPressed, _currentInstance.isAltPressed, _currentInstance.isWinPressed) + KeyInterop.KeyFromVirtualKey(vkCode).ToString();
                            _currentInstance.hotkey = _currentInstance.BuildKeyCode(_currentInstance.isShiftPressed, _currentInstance.isCtrlPressed, _currentInstance.isAltPressed, _currentInstance.isWinPressed);
                            _currentInstance.FinishInput();
                        }
                    }
                });
                return (IntPtr)1;
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            UnhookWindowsHookEx(_hookID);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

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