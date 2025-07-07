using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RocketGame
{
    public partial class Form1 : Form
    {
        private PictureBox rocket;
        private int rocketSpeed = 20;

        private List<PictureBox> lasers = new List<PictureBox>();
        private Timer laserTimer;
        private Image laserImage;
        private int laserSpeed = 50;

        private DateTime lastShotTime = DateTime.MinValue;
        private bool spacePressed = false;

        private Random rand = new Random();
        private Timer asteroidMovementTimer;
        private Timer asteroidSpawnTimer;
        private int spawnStep = 0;
        private bool spawningAsteroids = false;
        private List<PictureBox> currentAsteroids = new List<PictureBox>();

        private int maxHeight;
        private int[] topPositions;
        private int[] middlePositions;
        private int[] bottomPositions;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;

            maxHeight = this.ClientSize.Height;
            topPositions = new int[] { maxHeight / 6, maxHeight / 4 };
            middlePositions = new int[] { maxHeight / 2 };
            bottomPositions = new int[] { 2 * maxHeight / 3, maxHeight - 100 };

            asteroidMovementTimer = new Timer();
            asteroidMovementTimer.Interval = 16;
            asteroidMovementTimer.Tick += AsteroidMovementTimer_Tick;
            asteroidMovementTimer.Start();

            asteroidSpawnTimer = new Timer();
            asteroidSpawnTimer.Interval = 1000;
            asteroidSpawnTimer.Tick += AsteroidSpawnTimer_Tick;

            this.Load += GameScreen_Load;
            this.KeyDown += GameScreen_KeyDown;
            this.KeyUp += GameScreen_KeyUp;
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            rocket = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(100, this.ClientSize.Height / 2 - 50),
                BackColor = Color.Transparent
            };

            Image rocketImage;
            switch (PlayerSettings.SelectedRocket)
            {
                case 1:
                    rocketImage = Properties.Resources.katyusha_le_hawken_11x_removebg_preview;
                    break;
                case 2:
                    rocketImage = Properties.Resources.spaceship2;
                    break;
                case 3:
                    rocketImage = Properties.Resources.spaceship3;
                    break;
                default:
                    rocketImage = Properties.Resources.katyusha_le_hawken_11x_removebg_preview;
                    break;
            }

            rocketImage = (Image)rocketImage.Clone();
            rocketImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            rocket.Image = rocketImage;
            rocket.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(rocket);

            laserImage = (Image)Properties.Resources.laser1.Clone();
            laserTimer = new Timer { Interval = 16 };
            laserTimer.Tick += LaserTimer_Tick;
            laserTimer.Start();

            spawningAsteroids = true;
            asteroidSpawnTimer.Start();
        }

        private void GameScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && rocket.Top - rocketSpeed >= 0)
                rocket.Top -= rocketSpeed;
            else if (e.KeyCode == Keys.Down && rocket.Bottom + rocketSpeed <= ClientSize.Height)
                rocket.Top += rocketSpeed;
            else if (e.KeyCode == Keys.Space && !spacePressed)
            {
                spacePressed = true;
                if ((DateTime.Now - lastShotTime).TotalMilliseconds >= 1000)
                {
                    ShootLaser();
                    lastShotTime = DateTime.Now;
                }
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) spacePressed = false;
        }

        private void ShootLaser()
        {
            var laser = new PictureBox
            {
                Size = new Size(100, 20),
                Image = laserImage,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Location = new Point(rocket.Right - 10, rocket.Top + rocket.Height / 2 - 2)
            };
            lasers.Add(laser);
            Controls.Add(laser);
            laser.BringToFront();
        }

        private void LaserTimer_Tick(object sender, EventArgs e)
        {
            for (int i = lasers.Count - 1; i >= 0; i--)
            {
                lasers[i].Left += laserSpeed;
                if (lasers[i].Left > ClientSize.Width)
                {
                    Controls.Remove(lasers[i]);
                    lasers[i].Dispose();
                    lasers.RemoveAt(i);
                }
            }
        }

        private void MakeAsteroid()
        {
            var asteroidImages = new List<Image>
            {
                Properties.Resources.asteroid1_removebg_preview,
                Properties.Resources.asteroid2_removebg_preview,
                Properties.Resources.asteroid3_removebg_preview
            };

            var available = new List<int>();
            available.AddRange(topPositions);
            available.AddRange(middlePositions);
            available.AddRange(bottomPositions);

            int idx = rand.Next(available.Count);
            int y = available[idx];

            var asteroid = new PictureBox
            {
                Tag = "asteroid",
                Size = new Size(70, 64),
                Image = (Image)asteroidImages[rand.Next(asteroidImages.Count)].Clone(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Location = new Point(ClientSize.Width, y)
            };

            Controls.Add(asteroid);
            currentAsteroids.Add(asteroid);
        }

        private void AsteroidSpawnTimer_Tick(object sender, EventArgs e)
        {
            if (spawningAsteroids)
            {
                if (spawnStep < 3)
                {
                    MakeAsteroid();
                    spawnStep++;
                }
                else
                {
                    spawningAsteroids = false;
                    asteroidSpawnTimer.Stop();
                }
            }
        }

        private void AsteroidMovementTimer_Tick(object sender, EventArgs e)
        {
            foreach (Control c in Controls)
            {
                if (c.Tag as string == "asteroid")
                {
                    c.Left -= 10;
                    if (c.Left < -c.Width)
                    {
                        Controls.Remove(c);
                        (c as PictureBox).Dispose();
                        currentAsteroids.Remove(c as PictureBox);
                    }
                }
            }

            CheckCollision();

            if (currentAsteroids.Count == 0 && !spawningAsteroids)
            {
                spawnStep = 0;
                spawningAsteroids = true;
                asteroidSpawnTimer.Start();
            }
        }

        private void CheckCollision()
        {
            foreach (var asteroid in currentAsteroids.ToList())
            {
                if (rocket.Bounds.IntersectsWith(asteroid.Bounds))
                {
                    EndGame();
                    break;
                }
            }
        }

        private void EndGame()
        {
            asteroidMovementTimer.Stop();
            asteroidSpawnTimer.Stop();
            laserTimer.Stop();

            GameOver gameOverForm = new GameOver();
            gameOverForm.Show();
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
}
