using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using WindowsInput;
using System.Collections.Generic;
using System.Windows;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using WindowsInput.Native;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace swiftKEY_V2
{
    internal class EventHandler
    {
        // Sound device
        private static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        private static MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        // Keyboard
        //private static InputSimulator simulator = new InputSimulator();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint KEYEVENTF_KEYDOWN = 0x0001; // Keydown-Flag
        private const uint KEYEVENTF_KEYUP = 0x0002; // Keyup-Flag

        // Lock
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void LockWorkStation();

        public static void FetchFunction(string data)
        {
            if (data.Contains("hotkey_"))
            {
                string[] splitData = data.Split('_');
                if(splitData.Length == 2)
                {
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                else if(splitData.Length == 3) {
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                else if (splitData.Length == 4)
                {
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[3]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[3]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                else if (splitData.Length == 5)
                {
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[3]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[4]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[3]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[4]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                else if (splitData.Length == 6)
                {
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[3]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[4]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[5]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[2]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[3]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[4]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[5]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
            }
            else if (data.Equals("volumeup"))
            {
                device.AudioEndpointVolume.VolumeStepUp();
            }
            else if (data.Equals("volumedown"))
            {
                device.AudioEndpointVolume.VolumeStepDown();
            }
            else if (data.Equals("volumemute"))
            {
                if (device.AudioEndpointVolume.Mute)
                    device.AudioEndpointVolume.Mute = false;
                else
                    device.AudioEndpointVolume.Mute = true;
            }
            else if (data.Contains("open_"))
            {
                OpenProgram(data.Replace("open_", ""));
            }
            else if (data.Equals("shutdown"))
            {
                Shutdown();
            }
            else if (data.Equals("restart"))
            {
                Restart();
            }
            else if (data.Equals("lock"))
            {
                Lock();
            }
        }

        private static void PressKey(int keyCode)
        {
            VirtualKeyCode key = (VirtualKeyCode) keyCode;
            //simulator.Keyboard.KeyPress(key);
        }

        private static void OpenProgram(string name)
        {
            try
            {
                Process.Start(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Öffnen des Programms: " + ex.Message);
            }
        }

        private static void Shutdown()
        {
            Process.Start("shutdown", "/s /t 0");
        }

        private static void Restart()
        {
            Process.Start("shutdown", "/r /t 0");
        }

        private static void Lock()
        {
            LockWorkStation();
        }
    }
}
