using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace swiftKEY_V2
{
    internal class ConfigManager
    {
        private const string ConfigFilePath = "config.cfg";

        public static ButtonConfig LoadConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                CreateConfig();
            }

            var json = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<ButtonConfig>(json);
        }

        public static void SaveConfig(ButtonConfig configurations)
        {
            var json = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        private static void CreateConfig()
        {
            // Standardkonfigurationen erstellen
            var defaultConfig = new ButtonConfig();
            for (int i = 1; i <= MainWindow.buttonAmount; i++)
            {
                defaultConfig.ButtonFunctions["Button" + i] = "";
            }
            SaveConfig(defaultConfig);
        }
    }

    public class ButtonConfig
    {
        public Dictionary<string, string> ButtonFunctions { get; set; } = new Dictionary<string, string>();
    }
}
