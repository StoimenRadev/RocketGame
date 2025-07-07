using System.Windows.Forms;
using System;

namespace RocketGame
{
    partial class GameOver
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1 (Main Menu)
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent; // Transparent background
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Size = new System.Drawing.Size(229, 77);
            this.button1.Text = "Main Menu";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2 (Try Again)
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent; // Transparent background
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Size = new System.Drawing.Size(229, 77);
            this.button2.Text = "Try Again";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // GameOver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::RocketGame.Properties.Resources.gameover_background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GameOver";
            this.Text = "GameOver";
            this.Load += new System.EventHandler(this.GameOver_Load);
            this.Resize += new System.EventHandler(this.GameOver_Resize);
            this.ResumeLayout(false);

            // Positioning buttons on the form.
            this.button1.Left = 2;  // Left position for Main Menu button
            this.button1.Top = this.ClientSize.Height - this.button1.Height - 3;  // 10 pixels from the bottom

            this.button2.Left = this.ClientSize.Width - this.button2.Width - 10;  // Right position for Try Again button
            this.button2.Top = this.ClientSize.Height - this.button2.Height - 10;  // 10 pixels from the bottom
        }

        // Adding the missing GameOver_Load method to handle the Load event  
        private void GameOver_Load(object sender, EventArgs e)
        {
            // Initialization logic can be added here if needed
        }

        // Event to reposition buttons when resizing
        private void GameOver_Resize(object sender, EventArgs e)
        {
            // Recalculate button positions on resize to maintain bottom-left and bottom-right positioning
            button1.Left = 10;  // Left position for Main Menu button
            button1.Top = this.ClientSize.Height - button1.Height - 10;  // 10 pixels from the bottom

            button2.Left = this.ClientSize.Width - button2.Width - 10;  // Right position for Try Again button
            button2.Top = this.ClientSize.Height - button2.Height - 10;  // 10 pixels from the bottom
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;

        // Event for Main Menu button click
        private void button1_Click(object sender, EventArgs e)
        {
            // Code for returning to the Main Menu
            // Example: new MainMenuForm().Show(); this.Hide();
        }

        // Event for Try Again button click
        private void button2_Click(object sender, EventArgs e)
        {
            // Code for restarting the game
            // Example: new GameForm().Show(); this.Hide();
        }
    }
}
