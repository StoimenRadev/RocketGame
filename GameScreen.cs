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

        private PictureBox heart;


        private Timer asteroidMovementTimer;
        private Timer asteroidSpawnTimer;
        private int spawnStep = 0;
        private bool spawningAsteroids = false;
        private List<PictureBox> currentAsteroids = new List<PictureBox>();
        private int maxHeight;
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
        private int ufoYPosition;

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

            asteroidSpawnTimer = new Timer { Interval = 1000 };
            asteroidSpawnTimer.Tick += AsteroidSpawnTimer_Tick;

            this.Load += GameScreen_Load;
            this.KeyDown += GameScreen_KeyDown;
            this.KeyUp += GameScreen_KeyUp;
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            // Initialize the rocket
            ResetRocket();

            // Load Laser Image from Resources
            laserImage = (Image)Properties.Resources.laser1.Clone();

            // Timer for moving lasers
            laserTimer = new Timer();
            laserTimer.Interval = 16; // 60 FPS (1000ms / 60 = 16.67ms per frame)
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

            spawningAsteroids = true;
            asteroidSpawnTimer.Start();

            ufoImage = Properties.Resources.ufo_removebg_preview;
            ufoLaserImage = Properties.Resources.laser2_removebg_preview;
            ufoSpawnTimer = new Timer { Interval = 15000 };
            ufoSpawnTimer.Tick += UfoSpawnTimer_Tick;
            ufoSpawnTimer.Start();

            ufoMovementTimer = new Timer { Interval = 64 };
            ufoMovementTimer.Tick += UfoMovementTimer_Tick;

            ufoShootingTimer = new Timer { Interval = 7000 };
            ufoShootingTimer.Tick += UfoShootingTimer_Tick;
        }

        private void ResetRocket()
        {
            // Remove the old rocket if it exists
            if (rocket != null)
            {
                Controls.Remove(rocket);
                rocket.Dispose();
            }

            // Create a new rocket
            rocket = new PictureBox();
            rocket.Size = new Size(100, 100);
            rocket.Location = new Point(100, this.ClientSize.Height / 2 - 50);
            rocket.BackColor = Color.Transparent;

            Image rocketImage;

            // Set the image of the rocket based on selected rocket
            switch (PlayerSettings.SelectedRocket)
            {
                case 1:
                    rocketImage = (Image)Properties.Resources.katyusha_le_hawken_11x_removebg_preview.Clone();
                    break;
                case 2:
                    rocketImage = (Image)Properties.Resources.spaceship2.Clone();
                    break;
                case 3:
                    rocketImage = (Image)Properties.Resources.spaceship3_removebg_preview.Clone();
                    break;
                default:
                    rocketImage = (Image)Properties.Resources.katyusha_le_hawken_11x_removebg_preview.Clone();
                    break;
            }

            rocketImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            rocket.Image = rocketImage;
            rocket.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(rocket);
        }

        private void MakeAsteroid()
        {
            int[] possiblePositions = new int[]
            {
                scoreLabelHeight + 100,  // Top position
                this.ClientSize.Height / 2, // Middle position
                2 * this.ClientSize.Height / 3, // Bottom position
                this.ClientSize.Height - 100 // Near bottom position
            };

            // Select a random position for the asteroid
            int selectedPosition = possiblePositions[rand.Next(possiblePositions.Length)];

            // Prevent spawning at the same position if another asteroid is already there
            bool positionOccupied = currentAsteroids.Any(a => Math.Abs(a.Top - selectedPosition) < a.Height);

            // If the position is occupied, retry until we find a free spot
            if (positionOccupied)
            {
                MakeAsteroid();  // Recursive call to retry
                return;
            }

            var asteroidImages = new List<Image>
            {
                Properties.Resources.asteroid1_removebg_preview,
                Properties.Resources.asteroid2_removebg_preview,
                Properties.Resources.asteroid3_removebg_preview
            };

            var asteroid = new PictureBox
            {
                Tag = "asteroid",
                Size = new Size(100, 94),
                Image = (Image)asteroidImages[rand.Next(asteroidImages.Count)].Clone(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Location = new Point(ClientSize.Width, selectedPosition) // Set asteroid at random position
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
                    c.Left -= 30;
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

            ufoYPosition = rand.Next(0, this.ClientSize.Height - 130); // Random UFO Y position

            ufo = new PictureBox
            {
                Tag = "ufo",
                Size = new Size(160, 130),
                BackColor = Color.Transparent,
                Image = (Image)ufoImage.Clone(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(ClientSize.Width, ufoYPosition)
            };
            Controls.Add(ufo);
            ufoAlive = true;
            ufoMovementTimer.Start();
        }

        private void UfoMovementTimer_Tick(object sender, EventArgs e)
        {
            if (ufo != null && ufo.Left > 0)
                ufo.Left -= 15;
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
                    ufoLaser.Left -= 100;
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

            // Reset the rocket when the game ends
            ResetRocket();

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
                lasers[i].Left += 100;
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
