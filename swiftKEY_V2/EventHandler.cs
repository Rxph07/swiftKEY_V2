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
        private static InputSimulator simulator = new InputSimulator();

        // Lock
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void LockWorkStation();

        public static void FetchFunction(string data)
        {
            if (data.Contains("hotkey_"))
            {
                string[] splitData = data.Split('_');
                if (int.Parse(splitData[1]) != -1)
                {
                    simulator.Keyboard.ModifiedKeyStroke((VirtualKeyCode) int.Parse(splitData[1]), (VirtualKeyCode) int.Parse(splitData[2]));
                }
                else
                {
                    VirtualKeyCode key = (VirtualKeyCode) int.Parse(splitData[2]);
                    simulator.Keyboard.KeyPress(key);
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
            simulator.Keyboard.KeyPress(key);
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
