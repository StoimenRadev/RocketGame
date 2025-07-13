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
            Form gameScreen = new Form1();
            gameScreen.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form cosmeticsForm = new CosmeticsForm();
            cosmeticsForm.Show();
        }

        

        private void button4_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Game", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form statisticsForm = new StatisticsForm();
            statisticsForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form optionsForm = new ControlsForm();
            optionsForm.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }


     
}