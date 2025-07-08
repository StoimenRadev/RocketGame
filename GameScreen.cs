// Add this code to your Form1.cs

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
        private int rocketSpeed = 35;
        private List<PictureBox> lasers = new List<PictureBox>();
        private Timer laserTimer;
        private Image laserImage;
        private int laserSpeed = 80;
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
        private int scoreLabelHeight;
        private int score = 0;
        private Timer scoreTimer;
        private Label scoreLabel;

        private PictureBox ufo;
        private Timer ufoSpawnTimer;
        private Timer ufoMovementTimer;
        private Timer ufoShootingTimer;
        private bool ufoAlive = false;
        private Image ufoImage;
        private Image ufoLaserImage;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;

            maxHeight = this.ClientSize.Height;

            asteroidMovementTimer = new Timer { Interval = 16 };
            asteroidMovementTimer.Tick += AsteroidMovementTimer_Tick;
            asteroidMovementTimer.Start();

            asteroidSpawnTimer = new Timer { Interval = 1 };
            asteroidSpawnTimer.Tick += AsteroidSpawnTimer_Tick;

            this.Load += GameScreen_Load;
            this.KeyDown += GameScreen_KeyDown;
            this.KeyUp += GameScreen_KeyUp;
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            rocket = new PictureBox
            {
                Size = new Size(120, 100),
                Location = new Point(100, this.ClientSize.Height / 2 - 50),
                BackColor = Color.Transparent
            };
            Image rocketImage = Properties.Resources.katyusha_le_hawken_11x_removebg_preview;
            rocketImage = (Image)rocketImage.Clone();
            rocketImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            rocket.Image = rocketImage;
            rocket.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(rocket);

            laserImage = (Image)Properties.Resources.laser1.Clone();
            laserTimer = new Timer { Interval = 16 };
            laserTimer.Tick += LaserTimer_Tick;
            laserTimer.Start();

            scoreLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Text = "Score: 0",
                Location = new Point(this.ClientSize.Width - 220, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Controls.Add(scoreLabel);
            scoreLabel.BringToFront();

            scoreTimer = new Timer { Interval = 1000 };
            scoreTimer.Tick += (s, e2) =>
            {
                score++;
                scoreLabel.Text = $"Score: {score}";
            };
            scoreTimer.Start();

            scoreLabelHeight = scoreLabel.Height;
            int adjustedHeight = this.ClientSize.Height - scoreLabelHeight;
            topPositions = new int[] { scoreLabelHeight + 100 };
            middlePositions = new int[] { adjustedHeight / 2 };
            bottomPositions = new int[] { 2 * adjustedHeight / 3, adjustedHeight - 100 };

            spawningAsteroids = true;
            asteroidSpawnTimer.Start();

            ufoImage = Properties.Resources.ufo_removebg_preview;
            ufoLaserImage = Properties.Resources.laser2_removebg_preview;
            ufoSpawnTimer = new Timer { Interval = 10000 };
            ufoSpawnTimer.Tick += UfoSpawnTimer_Tick;
            ufoSpawnTimer.Start();

            ufoMovementTimer = new Timer { Interval = 64 };
            ufoMovementTimer.Tick += UfoMovementTimer_Tick;

            ufoShootingTimer = new Timer { Interval = 7000 };
            ufoShootingTimer.Tick += UfoShootingTimer_Tick;
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
                Size = new Size(100, 94),
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

        private void UfoSpawnTimer_Tick(object sender, EventArgs e)
        {
            if (ufoAlive) return;
            int[] allPositions = topPositions.Concat(middlePositions).Concat(bottomPositions).ToArray();
            int y = allPositions[rand.Next(allPositions.Length)];

            ufo = new PictureBox
            {
                Tag = "ufo",
                Size = new Size(160, 130),
                BackColor = Color.Transparent,
                Image = (Image)ufoImage.Clone(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(ClientSize.Width, y)
            };
            Controls.Add(ufo);
            ufoAlive = true;
            ufoMovementTimer.Start();

            var stopTimer = new Timer { Interval = 5000 };
            stopTimer.Tick += (s, args) =>
            {
                ufoMovementTimer.Stop();
                ufoShootingTimer.Start();
                stopTimer.Stop();
                stopTimer.Dispose();
            };
            stopTimer.Start();
        }

        private void UfoMovementTimer_Tick(object sender, EventArgs e)
        {
            if (ufo != null && ufo.Left > 0)
                ufo.Left -= 20;
        }

        private void UfoShootingTimer_Tick(object sender, EventArgs e)
        {
            if (ufo != null && ufoAlive)
            {
                PictureBox ufoLaser = new PictureBox
                {
                    Size = new Size(100, 20),
                    BackColor = Color.Transparent,
                    Image = ufoLaserImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(ufo.Left, ufo.Top + ufo.Height / 2 - 6),
                    Tag = "ufoLaser"
                };
                Controls.Add(ufoLaser);
                ufoLaser.BringToFront();

                var laserTimer = new Timer { Interval = 16 };
                laserTimer.Tick += (s, args) =>
                {
                    ufoLaser.Left -= 50;
                    if (ufoLaser.Right < 0)
                    {
                        laserTimer.Stop();
                        Controls.Remove(ufoLaser);
                        ufoLaser.Dispose();
                        laserTimer.Dispose();
                    }
                };
                laserTimer.Start();
            }
        }

        private void CheckCollision()
        {
            foreach (Control c in Controls)
            {
                if (c.Tag as string == "ufoLaser" && rocket.Bounds.IntersectsWith(c.Bounds))
                    EndGame();
            }

            foreach (var asteroid in currentAsteroids.ToList())
            {
                if (rocket.Bounds.IntersectsWith(asteroid.Bounds))
                {
                    EndGame();
                    break;
                }
            }

            foreach (var laser in lasers.ToList())
            {
                if (ufo != null && ufo.Bounds.IntersectsWith(laser.Bounds))
                {
                    score += 10;
                    scoreLabel.Text = $"Score: {score}";

                    Controls.Remove(ufo);
                    ufo.Dispose();
                    ufo = null;
                    ufoAlive = false;
                    ufoShootingTimer.Stop();

                    Controls.Remove(laser);
                    lasers.Remove(laser);
                    laser.Dispose();
                }
            }
        }

        private void EndGame()
        {
            asteroidMovementTimer.Stop();
            asteroidSpawnTimer.Stop();
            laserTimer.Stop();
            scoreTimer.Stop();
            ufoSpawnTimer.Stop();
            ufoMovementTimer.Stop();
            ufoShootingTimer.Stop();
            GameOver go = new GameOver();
            go.Show();
            this.Close();
        }

        private void GameScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && rocket.Top - rocketSpeed >= scoreLabelHeight)
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
    }
}