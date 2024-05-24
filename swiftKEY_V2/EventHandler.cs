using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Windows;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using System.Linq;

namespace swiftKEY_V2
{
    internal class EventHandler
    {
        #region Declarations
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

        private static EventHandler eventHandler = new EventHandler();
        private SpotifyConfig spotifyConfig;
        private static int _selectedProfile;
        #endregion

        public async static void FetchFunction(string data, MainWindow mainWindow)
        {
            if (data.Contains("hotkey_"))
            {
                #region Hotkey
                string[] splitData = data.Split('_');
                if (splitData.Length == 2)
                {
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event((byte)int.Parse(splitData[1]), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                else if (splitData.Length == 3)
                {
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
                #endregion
            }
            else if (data.Equals("volumeup"))
            {
                #region Increase Volume
                MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                device.AudioEndpointVolume.VolumeStepUp();
                #endregion
            }
            else if (data.Equals("volumedown"))
            {
                #region Decrease Volume
                MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                device.AudioEndpointVolume.VolumeStepDown();
                #endregion
            }
            else if (data.Equals("volumemute"))
            {
                #region Toggle Volume Mute
                MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                #endregion
            }
            else if (data.Contains("openfile_"))
            {
                #region Open File
                try
                {
                    Process.Start(data.Replace("openfile_", ""));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Öffnen der Datei: {ex.Message}");
                }
                #endregion
            }
            else if (data.Contains("openfolder_"))
            {
                #region Open Folder
                try
                {
                    Process.Start(data.Replace("openfolder_", ""));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Öffnen der Datei: {ex.Message}");
                }
                #endregion
            }
            else if (data.Equals("shutdown"))
            {
                #region Shutdown
                Shutdown();
                #endregion
            }
            else if (data.Equals("restart"))
            {
                #region Restart
                Restart();
                #endregion
            }
            else if (data.Equals("lock"))
            {
                #region Lock
                Lock();
                #endregion
            }
            else if(data.Contains("openwebsite_"))
            {
                #region Open Website
                try
                {
                    Process.Start(data.Replace("openwebsite_", ""));
                }
                catch (Exception)
                {
                    MessageBox.Show($"Fehler beim Öffnen der Website. Stelle sicher, dass der eingegebene Link korrekt ist. (Bsp: https://google.com)");
                }
                #endregion
            }
            else if (data.Equals("spotifyplaypause"))
            {
                #region Spotify Play / Pause
                if (!await eventHandler.isLoggedIn())
                {
                    MessageBox.Show("Du bist nicht angemeldet. Bitte melde dich an.");
                    return;
                }

                var spotify = new SpotifyClient(await MainWindow.spotifyAuth.GetAccessTokenAsync());
                var playbackStatus = await spotify.Player.GetCurrentPlayback();

                if (playbackStatus != null)
                {
                    if (playbackStatus.IsPlaying)
                    {
                        try
                        {
                            await spotify.Player.PausePlayback();
                        }
                        catch (APIException ex)
                        {
                            MessageBox.Show($"{ex.Message}");
                        }
                    }
                    else
                    {
                        try
                        {
                            await spotify.Player.ResumePlayback();
                        }
                        catch (APIException)
                        {
                        }
                    }
                }
                #endregion
            }
            else if (data.Equals("spotifyprevious"))
            {
                #region Spotify Play Previous
                if (!await eventHandler.isLoggedIn())
                {
                    MessageBox.Show("Du bist nicht angemeldet. Bitte melde dich an.");
                    return;
                }

                var spotify = new SpotifyClient(await MainWindow.spotifyAuth.GetAccessTokenAsync());
                try
                {
                    await spotify.Player.SkipPrevious();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
                #endregion
            }
            else if (data.Equals("spotifynext"))
            {
                #region Spotify Play Next
                if (!await eventHandler.isLoggedIn())
                {
                    MessageBox.Show("Du bist nicht angemeldet. Bitte melde dich an.");
                    return;
                }

                var spotify = new SpotifyClient(await MainWindow.spotifyAuth.GetAccessTokenAsync());
                try
                {
                    await spotify.Player.SkipPrevious();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
                #endregion
            }
            else if (data.Contains("spotifyvolumedown_"))
            {
                #region Spotify Volume Down
                if (!await eventHandler.isLoggedIn())
                {
                    MessageBox.Show("Du bist nicht angemeldet. Bitte melde dich an.");
                    return;
                }

                string[] splitData = data.Split('_');
                var spotify = new SpotifyClient(await MainWindow.spotifyAuth.GetAccessTokenAsync());
                var currentlyPlaying = await spotify.Player.GetCurrentPlayback();

                if (currentlyPlaying?.IsPlaying == true)
                {
                    var currentVolume = currentlyPlaying.Device.VolumePercent;

                    if (currentVolume.HasValue)
                    {
                        try
                        {
                            var newVolume = currentVolume.Value - int.Parse(splitData[1]);
                            newVolume = Math.Max(0, newVolume);
                            await spotify.Player.SetVolume(new PlayerVolumeRequest((sbyte)newVolume));
                        }
                        catch (APIException ex)
                        {
                            MessageBox.Show($"{ex.Message}");
                        }
                    }
                }
                #endregion
            }
            else if (data.Contains("spotifyvolumeup_"))
            {
                #region Spotify Volume Up
                if (!await eventHandler.isLoggedIn())
                {
                    MessageBox.Show("Du bist nicht angemeldet. Bitte melde dich an.");
                    return;
                }

                string[] splitData = data.Split('_');
                var spotify = new SpotifyClient(await MainWindow.spotifyAuth.GetAccessTokenAsync());
                var currentlyPlaying = await spotify.Player.GetCurrentPlayback();

                if (currentlyPlaying?.IsPlaying == true)
                {
                    var currentVolume = currentlyPlaying.Device.VolumePercent;

                    if (currentVolume.HasValue)
                    {
                        try
                        {
                            var newVolume = currentVolume.Value + int.Parse(splitData[1]);
                            newVolume = Math.Min(100, newVolume);
                            await spotify.Player.SetVolume(new PlayerVolumeRequest((sbyte)newVolume));
                        }
                        catch (APIException ex)
                        {
                            MessageBox.Show($"{ex.Message}");
                        }
                    }
                }
                #endregion
            }
            else if (data.Contains("spotifyrepeatmode"))
            {
                #region Spotify Repeat Mode
                if (!await eventHandler.isLoggedIn())
                {
                    MessageBox.Show("Du bist nicht angemeldet. Bitte melde dich an.");
                    return;
                }

                var spotify = new SpotifyClient(await MainWindow.spotifyAuth.GetAccessTokenAsync());
                var currentlyPlaying = await spotify.Player.GetCurrentPlayback();
                var repeatMode = currentlyPlaying.RepeatState;

                PlayerSetRepeatRequest.State newRepeatState;
                switch (repeatMode)
                {
                    case "off":
                        newRepeatState = PlayerSetRepeatRequest.State.Context;
                        break;
                    case "track":
                        newRepeatState = PlayerSetRepeatRequest.State.Off;
                        break;
                    case "context":
                        newRepeatState = PlayerSetRepeatRequest.State.Track;
                        break;
                    default:
                        newRepeatState = PlayerSetRepeatRequest.State.Off;
                        break;
                }

                await spotify.Player.SetRepeat(new PlayerSetRepeatRequest(newRepeatState));
                #endregion
            }
            else if (data.Contains("spotifyshufflemode"))
            {
                #region Spotify Shuffle Mode
                if (!await eventHandler.isLoggedIn())
                {
                    MessageBox.Show("Du bist nicht angemeldet. Bitte melde dich an.");
                    return;
                }

                var spotify = new SpotifyClient(await MainWindow.spotifyAuth.GetAccessTokenAsync());
                var currentlyPlaying = await spotify.Player.GetCurrentPlayback();
                var shuffleMode = currentlyPlaying.ShuffleState;
                await spotify.Player.SetShuffle(new PlayerShuffleRequest(!shuffleMode));
                #endregion
            }
            else if (data.Contains("switchprofile_"))
            {
                #region Switch Profile
                int profileIndex = int.Parse(data.Replace("switchprofile_", ""));

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetSelectedProfile(profileIndex);
                    mainWindow.LoadButtonText();
                    mainWindow.UpdateProfiles();
                });
                #endregion
            }
        }

        #region Shutdown / Restart / Lock
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
        #endregion

        #region Spotify isLoggedIn
        private async Task<bool> isLoggedIn()
        {
            spotifyConfig = ConfigManager.LoadSpotifyConfig();

            string _clientId = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientID").Value;
            string _clientSecret = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientSecret").Value;
            string _redirectUri = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RedirectUri").Value;
            string _refreshToken = spotifyConfig.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RefreshToken").Value;

            if (_clientId == "" || _clientSecret == "" || _redirectUri == "" || _refreshToken == "")
                return false;

            if (string.IsNullOrEmpty(await MainWindow.spotifyAuth.GetAccessTokenAsync()))
                return false;

            return true;
        }
        #endregion
    }
}