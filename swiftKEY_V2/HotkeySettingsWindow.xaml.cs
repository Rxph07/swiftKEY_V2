using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
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
        private Button currentButton;

        private string hotkey = "";
        private int hotkeyCode = -1;
        private int btnIndex;

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

        public HotkeySettingsWindow(int pressedBtnIndex)
        {
            InitializeComponent();
            _currentInstance = this;
            _proc = HookCallback;
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

        private void ChooseHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                _hookID = SetHook(_proc);
                button.Focus();
                currentButton = button;
                isShiftPressed = false;
                isCtrlPressed = false;
                isAltPressed = false;
                isWinPressed = false;
                hotkeyCode = -1;
                KeyUp += ButtonPreviewKeyUpHandler;
                button.LostKeyboardFocus += ButtonLostKeyboardFocusHandler;
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
                        currentButton.Content = "Hotkey wählen...";
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

        private void FinishInput()
        {
            if (currentButton != null)
            {
                currentButton.LostKeyboardFocus -= ButtonLostKeyboardFocusHandler;
            }

            currentButton = null;
            config.ButtonConfigurations[btnIndex].Function = hotkey;
            ConfigManager.SaveConfig(config);
            UnhookWindowsHookEx(_hookID);
        }

        private void ButtonLostKeyboardFocusHandler(object sender, KeyboardFocusChangedEventArgs e)
        {
            FinishInput();
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
