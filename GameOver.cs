using System;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class GameOver : Form
    {
        public GameOver()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;  // Remove borders for fullscreen
            this.WindowState = FormWindowState.Maximized;  // Maximize the window to fill the screen

            // Create and configure the Main Menu button
            Button mainMenuButton = new Button
            {
                Text = "Main Menu",
                Size = new System.Drawing.Size(300, 70),
                Location = new System.Drawing.Point(this.ClientSize.Width / 2 - 150, this.ClientSize.Height / 2 - 35),
                Font = new System.Drawing.Font("Arial", 16)
            };
            mainMenuButton.Click += MainMenuButton_Click;
            Controls.Add(mainMenuButton);

            // Create and configure the Try Again button
            Button tryAgainButton = new Button
            {
                Text = "Try Again",
                Size = new System.Drawing.Size(300, 70),
                Location = new System.Drawing.Point(this.ClientSize.Width / 2 - 150, this.ClientSize.Height / 2 + 100),
                Font = new System.Drawing.Font("Arial", 16)
            };
            tryAgainButton.Click += TryAgainButton_Click;
            Controls.Add(tryAgainButton);
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            MainMenu menu = new MainMenu();
            menu.Show();
            this.Close();
        }

        private void TryAgainButton_Click(object sender, EventArgs e)
        {
            Form1 gameScreen = new Form1();
            gameScreen.Show();
            this.Close();
        }

        // You can also override OnResize to reposition buttons when resizing occurs
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Recalculate button positions on resize to maintain centering
            foreach (Control control in Controls)
            {
                if (control is Button button)
                {
                    button.Left = this.ClientSize.Width / 2 - button.Width / 2;
                }
            }
        }
    }
}
