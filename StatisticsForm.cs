using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class StatisticsForm : Form
    {
        public StatisticsForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            // Label for Games Played
            Label GamesPlayedLabel = new Label
            {
                Text = $"Games Played: {Statistics.GamesPlayed}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,  // Changed color to white for visibility
                Location = new Point(50, 50),
                AutoSize = true
            };

            // Label for Highest Score
            Label HighestScoreLabel = new Label
            {
                Text = $"Highest Score: {Statistics.HighestScore}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White, // White color for visibility
                Location = new Point(50, 120), // Lower to avoid overlap
                AutoSize = true
            };

            // Label for UFOs Destroyed
            Label lblUFOsKilledLabel = new Label
            {
                Text = $"UFOs Destroyed: {Statistics.TotalUFOsDestroyed}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White, // White color for visibility
                Location = new Point(50, 190), // Lower again
                AutoSize = true
            };

            // Add labels to the form
            this.Controls.Add(GamesPlayedLabel);
            this.Controls.Add(HighestScoreLabel);
            this.Controls.Add(lblUFOsKilledLabel);
        }
    }

    public static class Statistics
    {
        public static int GamesPlayed = 0;
        public static int HighestScore = 0;
        public static int TotalUFOsDestroyed = 0;
    }

    public partial class GameForm : Form
    {
        private int score = 0;
        private int ufoDestroyedThisGame = 0;

        public GameForm()
        {
            
            // Additional game initialization if needed
        }

        // Simulating game over
        private void EndGame()
        {
            // Increment games played
            Statistics.GamesPlayed++;

            // Update highest score
            if (score > Statistics.HighestScore)
            {
                Statistics.HighestScore = score;
            }

            // Update total UFOs destroyed
            Statistics.TotalUFOsDestroyed += ufoDestroyedThisGame;

            // Open the statistics form when the game ends
            StatisticsForm statisticsForm = new StatisticsForm();
            statisticsForm.Show();

            // Optionally close the current game window if needed
            this.Hide();  // Hide the current form instead of closing it
        }

        // Example of a method where UFOs are destroyed during the game
        private void DestroyUFO()
        {
            ufoDestroyedThisGame++;
        }

        // This method will simulate a game where UFOs are destroyed and the score increases
        private void GamePlay()
        {
            // Example: Player destroys a UFO
            DestroyUFO();

            // Example: Player gains score
            score += 200;

            // Check for game-over condition (e.g., player losing all lives or health)
            if (score >= 1000) // Example condition for game over
            {
                EndGame(); // End the game when the condition is met
            }
        }
    }
}
