using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();


            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            label1.TextAlign = ContentAlignment.TopCenter;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form gameScreen = new GameScreen();
            gameScreen.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form cosmeticsForm = new CosmeticsForm();
            cosmeticsForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StatisticsForm statisticsForm = new StatisticsForm();
            statisticsForm.Show();
            this.Hide();
        }
    }
}