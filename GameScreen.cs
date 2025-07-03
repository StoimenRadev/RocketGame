using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class Form1 : Form
    {
        Random rand = new Random();
        List<PictureBox> items = new List<PictureBox>();
        Timer spawnTimer;

        public Form1()
        {
            InitializeTimer();
        }

        // Initialize the timer to spawn asteroids every 2 seconds
        private void InitializeTimer()
        {
            spawnTimer = new Timer();
            spawnTimer.Interval = 2000; // 2000 ms = 2 seconds
            spawnTimer.Tick += TimerEvent;
            spawnTimer.Start();
        }

        // Create a new asteroid and add it to the form
        private void MakePictureBox()
        {
            PictureBox newPic = new PictureBox();
            newPic.Height = 50;
            newPic.Width = 50;
            newPic.BackColor = Color.Gray; // Gray color for asteroids
            newPic.Image = Properties.Resources.asteroid1; // Set your asteroid image resource
            newPic.SizeMode = PictureBoxSizeMode.StretchImage;

            // Randomly choose one of the three fixed positions on the right side
            int spawnPoint = rand.Next(1, 4); // Randomly choose from 1 to 3

            switch (spawnPoint)
            {
                case 1:
                    newPic.Location = new Point(this.ClientSize.Width - newPic.Width, 0); // Top-right
                    break;
                case 2:
                    newPic.Location = new Point(this.ClientSize.Width - newPic.Width, this.ClientSize.Height / 2 - newPic.Height / 2); // Middle-right
                    break;
                case 3:
                    newPic.Location = new Point(this.ClientSize.Width - newPic.Width, this.ClientSize.Height - newPic.Height); // Bottom-right
                    break;
            }

            // Add the new asteroid to the list and the form
            items.Add(newPic);
            newPic.Click += NewPic_Click; // Click event to remove the asteroid
            this.Controls.Add(newPic);
        }

        // Handle the click event to remove the asteroid from the form
        private void NewPic_Click(object sender, EventArgs e)
        {
            PictureBox clickedPic = sender as PictureBox;
            items.Remove(clickedPic);
            this.Controls.Remove(clickedPic);
        }

        // Timer event that triggers asteroid spawning every 2 seconds
        private void TimerEvent(object sender, EventArgs e)
        {
            MakePictureBox();
        }
    }
}
