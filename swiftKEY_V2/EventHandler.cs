using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace swiftKEY_V2
{
    internal class EventHandler
    {
        // Sound device
        private static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

        // Keyboard
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
                MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                device.AudioEndpointVolume.VolumeStepUp();
            }
            else if (data.Equals("volumedown"))
            {
                MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                device.AudioEndpointVolume.VolumeStepDown();
            }
            else if (data.Equals("volumemute"))
            {
                MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
            }
            else if (data.Contains("openfile_"))
            {
                try
                {
                    Process.Start(data.Replace("openfile_", ""));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Öffnen der Datei: {ex.Message}");
                }
            }
            else if (data.Contains("openfolder_"))
            {
                try
                {
                    Process.Start(data.Replace("openfolder_", ""));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Öffnen der Datei: {ex.Message}");
                }
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
