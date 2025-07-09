using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class StatisticsForm : Form
    {
        private MainMenu mainMenu;

        public StatisticsForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.Load += StatisticsForm_Load;

            mainMenu = new MainMenu();
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            Label gamesPlayedLabel = new Label
            {
                Text = $"Games Played: {Statistics.GamesPlayed}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(650, 50),
                AutoSize = true
            };

            Label highestScoreLabel = new Label
            {
                Text = $"Highest Score: {Statistics.HighestScore}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(650, 120),
                AutoSize = true
            };

            Label ufosDestroyedLabel = new Label
            {
                Text = $"UFOs Destroyed: {Statistics.TotalUFOsDestroyed}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(650, 190),
                AutoSize = true
            };

            Label backLabel = new Label
            {
                Text = "Press ESC to go back",
                Font = new Font("Arial", 18, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(650, 300),
                AutoSize = true
            };

            this.Controls.Add(gamesPlayedLabel);
            this.Controls.Add(highestScoreLabel);
            this.Controls.Add(ufosDestroyedLabel);
            this.Controls.Add(backLabel);

            this.KeyDown += (s, eArgs) =>
            {
                if (eArgs.KeyCode == Keys.Escape)
                {
                    mainMenu.Show();
                    this.Hide();
                }
            };
            this.KeyPreview = true;
        }
    }

    public static class Statistics
    {
        public static int GamesPlayed = 0;
        public static int HighestScore = 0;
        public static int TotalUFOsDestroyed = 0;

        private static string savePath = "stats.txt";

        public static void Save()
        {
            System.IO.File.WriteAllText(savePath, $"{GamesPlayed},{HighestScore},{TotalUFOsDestroyed}");
        }

        public static void Load()
        {
            if (System.IO.File.Exists(savePath))
            {
                string[] data = System.IO.File.ReadAllText(savePath).Split(',');
                if (data.Length == 3)
                {
                    GamesPlayed = int.Parse(data[0]);
                    HighestScore = int.Parse(data[1]);
                    TotalUFOsDestroyed = int.Parse(data[2]);
                }


            }
        }
    }
}
