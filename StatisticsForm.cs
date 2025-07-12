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
            int centerX = this.ClientSize.Width / 2;

            // Define label texts and spacing
            string[] texts = {
        $"Games Played: {Statistics.GamesPlayed}",
        $"Highest Score: {Statistics.HighestScore}",
        $"UFOs Destroyed: {Statistics.TotalUFOsDestroyed}",
        $"Astronauts Saved: {Statistics.AstronautsSaved}"
    };

            int verticalSpacing = 20;
            Font labelFont = new Font("Arial", 24, FontStyle.Bold);

            // Calculate total height of all labels + spacing
            int totalHeight = texts.Length * labelFont.Height + (texts.Length - 1) * verticalSpacing;
            int startY = (this.ClientSize.Height - totalHeight) / 2;

            // Create and position labels
            for (int i = 0; i < texts.Length; i++)
            {
                Label label = new Label
                {
                    Text = texts[i],
                    Font = labelFont,
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
                    AutoSize = true
                };
                label.Location = new Point(centerX - label.PreferredWidth / 2, startY + i * (labelFont.Height + verticalSpacing));
                this.Controls.Add(label);
            }

            // ESC message
            Label backLabel = new Label
            {
                Text = "Press ESC to go back",
                Font = new Font("Arial", 18, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true
            };
            backLabel.Location = new Point(centerX - backLabel.PreferredWidth / 2, startY + texts.Length * (labelFont.Height + verticalSpacing));
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
        public static int AstronautsSaved = 0;  // ✅ New field

        private static string savePath = "stats.txt";

        public static void Save()
        {
            // Save all 4 stats
            System.IO.File.WriteAllText(savePath, $"{GamesPlayed},{HighestScore},{TotalUFOsDestroyed},{AstronautsSaved}");
        }

        public static void Load()
        {
            if (System.IO.File.Exists(savePath))
            {
                string[] data = System.IO.File.ReadAllText(savePath).Split(',');
                if (data.Length == 4)
                {
                    GamesPlayed = int.Parse(data[0]);
                    HighestScore = int.Parse(data[1]);
                    TotalUFOsDestroyed = int.Parse(data[2]);
                    AstronautsSaved = int.Parse(data[3]);
                }
            }
        }
    }
}
