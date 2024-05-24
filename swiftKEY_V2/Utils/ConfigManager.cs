using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace swiftKEY_V2
{
    internal class ConfigManager
    {
        private const string SpotifyConfigFilePath = "spotify.cfg";
        private const string ProfileConfigFilePath = "profiles.cfg";

        public static SpotifyConfig LoadSpotifyConfig()
        {
            if (!File.Exists(SpotifyConfigFilePath))
            {
                CreateSpotifyConfig();
            }

            var json = File.ReadAllText(SpotifyConfigFilePath);
            return JsonConvert.DeserializeObject<SpotifyConfig>(json);
        }

        public static ProfileConfig LoadProfileConfig()
        {
            if (!File.Exists(ProfileConfigFilePath))
            {
                CreateProfileConfig();
            }

            var json = File.ReadAllText(ProfileConfigFilePath);
            return JsonConvert.DeserializeObject<ProfileConfig>(json);
        }

        public static void SaveConfig(SpotifyConfig configurations)
        {
            var json = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            File.WriteAllText(SpotifyConfigFilePath, json);
        }

        public static void SaveConfig(ProfileConfig configurations)
        {
            var json = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            File.WriteAllText(ProfileConfigFilePath, json);
        }

        public static void CreateProfileConfig()
        {
            var profileConfig = new ProfileConfig();

            var profileButtons = new List<ButtonConfiguration>();
            for (int i = 1; i <= 15; i++)
            {
                profileButtons.Add(new ButtonConfiguration
                {
                    Title = "Button" + i,
                    Name = "",
                    Function = ""
                });
            }

            profileConfig.ProfileConfigurations.Add(new ProfileConfiguration
            {
                Title = "Profile1",
                Name = "Profil 1",
                ButtonConfigurations = profileButtons.ToArray()
            });

            SaveConfig(profileConfig);
        }

        private static void CreateSpotifyConfig()
        {
            var spotifyConfig = new SpotifyConfig();

            spotifyConfig.SpotifyConfigurations.Add(new SpotifyConfiguration
            {
                Title = "ClientID",
                Value = "",
            });

            spotifyConfig.SpotifyConfigurations.Add(new SpotifyConfiguration
            {
                Title = "ClientSecret",
                Value = "",
            });

            spotifyConfig.SpotifyConfigurations.Add(new SpotifyConfiguration
            {
                Title = "RedirectUri",
                Value = "",
            });

            spotifyConfig.SpotifyConfigurations.Add(new SpotifyConfiguration
            {
                Title = "RefreshToken",
                Value = "",
            });

            SaveConfig(spotifyConfig);
        }

        public static void AddProfile(string profileTitle, string profileName)
        {
            var profileConfig = LoadProfileConfig();

            var profileButtons = new List<ButtonConfiguration>();
            for (int i = 1; i <= 15; i++)
            {
                profileButtons.Add(new ButtonConfiguration
                {
                    Title = "Button" + i,
                    Name = "",
                    Function = ""
                });
            }

            var newProfile = new ProfileConfiguration
            {
                Title = profileTitle,
                Name = profileName,
                ButtonConfigurations = profileButtons.ToArray()
            };

            profileConfig.ProfileConfigurations.Add(newProfile);
            SaveConfig(profileConfig);
        }

        public static void RemoveProfile(string profileTitle, MainWindow mainWindow)
        {
            var profileConfig = LoadProfileConfig();

            var profileToRemove = profileConfig.ProfileConfigurations
                .Find(profile => profile.Title.Equals(profileTitle, StringComparison.OrdinalIgnoreCase));

            if (profileToRemove != null)
            {
                profileConfig.ProfileConfigurations.Remove(profileToRemove);
                SaveConfig(profileConfig);
                MainWindow.SetSelectedProfile(0);
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.LoadButtonText();
                    mainWindow.UpdateProfiles();
                });
            }
        }
    }

    public class ButtonConfig
    {
        public List<ButtonConfiguration> ButtonConfigurations { get; set; } = new List<ButtonConfiguration>();
    }

    public class SpotifyConfig
    {
        public List<SpotifyConfiguration> SpotifyConfigurations { get; set; } = new List<SpotifyConfiguration>();
    }

    public class ProfileConfig
    {
        public List<ProfileConfiguration> ProfileConfigurations { get; set; } = new List<ProfileConfiguration>();
    }

    public class ButtonConfiguration
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
    }

    public class SpotifyConfiguration
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }

    public class ProfileConfiguration
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public ButtonConfiguration[] ButtonConfigurations { get; set; }
    }
}