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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainMenu menu = new MainMenu();
            menu.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 gameScreen = new Form1();
            gameScreen.Show();
            this.Close();
        }

        private void GameOver_Load_1(object sender, EventArgs e)
        {

        }
    }
}
