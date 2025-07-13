using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketGame
{
    internal class GameSettings
    {
        public static bool EnableExplosions = true; // Default is enabled

        private static string settingsFilePath = "options.txt"; // Path to the settings file

        // Method to save settings to a file
        public static void Save()
        {
            // Save the current setting to a text file
            System.IO.File.WriteAllText(settingsFilePath, $"{MusicManager.IsMuted},{EnableExplosions}");
        }

        // Method to load settings from a file
        public static void Load()
        {
            if (System.IO.File.Exists(settingsFilePath))
            {
                // Read the file and parse the values
                string savedSetting = System.IO.File.ReadAllText(settingsFilePath);

                // Set the EnableExplosions and IsMuted settings based on the saved values
                string[] settings = savedSetting.Split(',');

                if (settings.Length == 2)
                {
                    bool isMuted;
                    if (bool.TryParse(settings[0], out isMuted))
                    {
                        MusicManager.IsMuted = isMuted;
                    }

                    bool isExplosionsEnabled;
                    if (bool.TryParse(settings[1], out isExplosionsEnabled))
                    {
                        EnableExplosions = isExplosionsEnabled;
                    }
                }
            }
        }
    }
}
