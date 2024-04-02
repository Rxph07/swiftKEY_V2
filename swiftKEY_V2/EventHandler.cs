using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using WindowsInput;

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
                PressKey(data.Replace("hotkey_", ""));
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

        private static void PressKey(string keys)
        {
            keys = keys.ToLower();
            simulator.Keyboard.TextEntry(keys);
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
