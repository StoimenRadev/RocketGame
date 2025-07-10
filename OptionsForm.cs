using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            label1.Location = new Point(
                (this.ClientSize.Width - label1.Width) / 2,
                (this.ClientSize.Height - label1.Height) / 7
            );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form generalOptions = new GeneralOptionsForm();
            generalOptions.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form controlsForm = new ControlsForm();
            controlsForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainMenu mainMenuForm = new MainMenu();
            mainMenuForm.Show();
        }
    }
}
