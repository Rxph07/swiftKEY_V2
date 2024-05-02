using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace swiftKEY_V2
{
    internal class ConfigManager
    {
        private const string ConfigFilePath = "config.cfg";
        private const string SpotifyConfigFilePath = "spotify.cfg";

        public static ButtonConfig LoadConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                CreateConfig();
            }

            var json = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<ButtonConfig>(json);
        }

        public static SpotifyConfig LoadSpotifyConfig()
        {
            if (!File.Exists(SpotifyConfigFilePath))
            {
                CreateConfig();
            }

            var json = File.ReadAllText(SpotifyConfigFilePath);
            return JsonConvert.DeserializeObject<SpotifyConfig>(json);
        }

        public static void SaveConfig(ButtonConfig configurations)
        {
            var json = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        public static void SaveConfig(SpotifyConfig configurations)
        {
            var json = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            File.WriteAllText(SpotifyConfigFilePath, json);
        }

        private static void CreateConfig()
        {
            var defaultConfig = new ButtonConfig();
            var spotifyConfig = new SpotifyConfig();

            for (int i = 1; i <= MainWindow.buttonAmount; i++)
            {
                defaultConfig.ButtonConfigurations.Add(new ButtonConfiguration
                {
                    Title = "Button" + i,
                    Name = "",
                    Function = ""
                });
            }

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

            SaveConfig(defaultConfig);
            SaveConfig(spotifyConfig);
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
}
