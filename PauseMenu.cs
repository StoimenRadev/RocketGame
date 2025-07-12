using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class PauseMenu : Form
    {
        public bool ResumeRequested { get; private set; } = false;
        public bool ReturnToMainMenuRequested { get; internal set; }

        public PauseMenu()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.ControlBox = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResumeRequested = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            Form mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
            
            

            
        }
    }
}
