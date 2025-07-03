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
    public partial class CosmeticsForm : Form
    {
        private int selectedRocketIndex = -1;
        private Label lblSelected;

        public CosmeticsForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            // Initialize lblSelected
            lblSelected = new Label();
            lblSelected.AutoSize = true;
            lblSelected.Font = new Font("Arial", 16, FontStyle.Bold);
            lblSelected.ForeColor = Color.White;
            lblSelected.BackColor = Color.Transparent; 
            lblSelected.Location = new Point(10, 10);
            lblSelected.Text = "Selected Rocket: None";
            this.Controls.Add(lblSelected);
        }

        private void CosmeticsForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SelectRocket(1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            SelectRocket(2);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            SelectRocket(3);
        }

        private void SelectRocket(int index)
        {
            selectedRocketIndex = index;
            lblSelected.Text = $"Selected Rocket: {index}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedRocketIndex == -1)
            {
                MessageBox.Show("Please select a rocket first.");
                return;
            }

            PlayerSettings.SelectedRocket = selectedRocketIndex;
            MessageBox.Show("Rocket saved!");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainMenu mainMenuForm = new MainMenu();
            mainMenuForm.Show(); 
        }
    }

    public static class PlayerSettings
    {
        public static int SelectedRocket { get; set; }
    }
}
