using System;
using System.Media;
using System.IO;

namespace RocketGame
{
    public static class MusicManager
    {
        private static SoundPlayer player;

        public static bool IsMuted = false;

        static MusicManager()
        {
            string musicFilePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "C:\\Users\\HOME\\Desktop\\Project\\Resources\\Кино - Спокойная ночь.wav");

            // Ensure the file exists
            if (File.Exists(musicFilePath))
            {
                player = new SoundPlayer(musicFilePath);
            }
            else
            {
                Console.WriteLine($"Error: Music file not found at {musicFilePath}");
                player = null;
            }
        }

        public static void Start()
        {
            if (!IsMuted && player != null)
            {
                player.PlayLooping();
            }
        }

        public static void Stop()
        {
            if (player != null)
            {
                player.Stop();
            }
        }

        public static void Mute()
        {
            IsMuted = true;
            Stop();
        }

        public static void Unmute()
        {
            IsMuted = false;
            Start();
        }
    }
}
