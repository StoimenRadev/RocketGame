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
        private int spawnStep;
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
        private Timer astronautSpawnTimer;

        int asteroidsDodged = 0;
        int lasersFired = 0;
        int enemiesDestroyed = 0;
        int survivalTime = 0;
        int gamesPlayed = 0;
        int highScore = 0;
        int astronautsSaved = 0;


        private Timer shieldTimer;
        private bool shieldActive = false;
        private int ufoDestroyedThisGame;



        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;


            maxHeight = this.ClientSize.Height;

            astronautSpawnTimer = new Timer { Interval = 30000 };
            astronautSpawnTimer.Tick += AstronautSpawnTimer_Tick;
            astronautSpawnTimer.Start();

            asteroidMovementTimer = new Timer { Interval = 16 };
            asteroidMovementTimer.Tick += AsteroidMovementTimer_Tick;
            asteroidMovementTimer.Start();

            asteroidSpawnTimer = new Timer { Interval = 1000 };
            asteroidSpawnTimer.Tick += AsteroidSpawnTimer_Tick;

            shieldTimer = new Timer { Interval = 3000 };
            shieldTimer.Tick += ShieldTimer_Tick;

            this.Load += GameScreen_Load;
            this.KeyDown += GameScreen_KeyDown;
            this.KeyUp += GameScreen_KeyUp;
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            ResetRocket();
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
            spawningAsteroids = true;
            asteroidSpawnTimer.Start();

            ufoImage = Properties.Resources.ufo_removebg_preview;
            ufoLaserImage = Properties.Resources.laser2_removebg_preview;
            ufoSpawnTimer = new Timer { Interval = 15000 };
            ufoSpawnTimer.Tick += UfoSpawnTimer_Tick;
            ufoSpawnTimer.Start();

            ufoMovementTimer = new Timer { Interval = 16 };
            ufoMovementTimer.Tick += UfoMovementTimer_Tick;

            ufoShootingTimer = new Timer { Interval = 5000 };
            ufoShootingTimer.Tick += UfoShootingTimer_Tick;

        }

        private void AstronautSpawnTimer_Tick(object sender, EventArgs e)
        {
            SpawnAstronaut();
        }
        private void MakeAsteroid()
        {
            int[] possiblePositions = new int[]
    {
        scoreLabelHeight + 100,
        this.ClientSize.Height / 2,
        2 * this.ClientSize.Height / 3,
        this.ClientSize.Height - 100
    };

            List<int> blockedPositions = new List<int>();
            if (ufoAlive && ufo != null)
            {
                foreach (int pos in possiblePositions)
                {
                    int ufoCenter = ufo.Top + ufo.Height / 2;
                    if (Math.Abs(pos - ufoCenter) < 120)
                        blockedPositions.Add(pos);
                }
            }

            var availablePositions = possiblePositions.Except(blockedPositions).ToList();
            if (availablePositions.Count == 0) return;

            int maxAttempts = 5;
            bool created = false;

            while (maxAttempts-- > 0 && !created)
            {
                int selectedPosition = availablePositions[rand.Next(availablePositions.Count)];
                bool positionOccupied = currentAsteroids.Any(a => Math.Abs(a.Top - selectedPosition) < a.Height);

                if (!positionOccupied)
                {
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
                        Location = new Point(ClientSize.Width, selectedPosition)
                    };

                    Controls.Add(asteroid);
                    currentAsteroids.Add(asteroid);
                    created = true;
                }
            }
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
                    c.Left -= 27;
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

            ufoYPosition = rand.Next(0, this.ClientSize.Height - 130);

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
            ufoShootingTimer.Start();
        }

        private void UfoMovementTimer_Tick(object sender, EventArgs e)
        {
            if (ufo != null && ufo.Left > this.ClientSize.Width - 500)
                ufo.Left -= 4;
            else
                ufoMovementTimer.Stop();
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
                    ufoLaser.Left -= 120;
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
                if (shieldActive) return;

                if (c.Tag as string == "ufoLaser" && rocket.Bounds.IntersectsWith(c.Bounds))
                {
                    SimulateHeartClick();
                    CreateLaserExplosion(c.Location);
                    c.Visible = false;
                }
            }

            foreach (var asteroid in currentAsteroids.ToList())
            {
                if (rocket.Bounds.IntersectsWith(asteroid.Bounds))
                {
                    asteroid.Visible = false;
                    SimulateHeartClick();
                    CreateAsteroidExplosion(asteroid.Location);
                    break;
                }
            }

            foreach (var laser in lasers.ToList())
            {
                if (ufo != null && ufo.Bounds.IntersectsWith(laser.Bounds))
                {
                    score += 10;
                    scoreLabel.Text = $"Score: {score}";

                    // UFO explosion when hit by laser
                    CreateUFOExplosion(ufo.Location);

                    // Remove UFO and laser
                    Controls.Remove(ufo);
                    ufo.Dispose();
                    ufo = null;
                    ufoAlive = false;
                    ufoShootingTimer.Stop();

                    Controls.Remove(laser);
                    lasers.Remove(laser);
                    laser.Dispose();

                    ufoDestroyedThisGame++;
                }
            }
        }
        private void CreateUFOExplosion(Point location)
        {
            PictureBox explosion = new PictureBox
            {
                Size = new Size(100, 100),
                BackColor = Color.Transparent,
                Image = Properties.Resources.big_explosion_removebg_preview,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = location
            };

            explosion.BringToFront();
            Controls.Add(explosion);

            Timer explosionTimer = new Timer { Interval = 2500 };
            explosionTimer.Tick += (sender, e) =>
            {
                explosionTimer.Stop();
                Controls.Remove(explosion);
                explosion.Dispose();
            };
            explosionTimer.Start();
        }


        private void CreateAsteroidExplosion(Point location)
        {
            PictureBox explosion = new PictureBox
            {
                Size = new Size(100, 94),
                BackColor = Color.Transparent,
                Image = Properties.Resources.big_explosion_removebg_preview, 
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = location
            };

            explosion.BringToFront();
            Controls.Add(explosion);

            Timer explosionTimer = new Timer { Interval = 2500 };
            explosionTimer.Tick += (sender, e) =>
            {
                explosionTimer.Stop();
                Controls.Remove(explosion);
                explosion.Dispose();
            };
            explosionTimer.Start();
        }

        private void CreateLaserExplosion(Point location)
        {
            PictureBox explosion = new PictureBox
            {
                Size = new Size(150, 150),
                BackColor = Color.Transparent,
                Image = Properties.Resources.small_explosion_removebg_preview,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = location
            };

            explosion.BringToFront();
            Controls.Add(explosion);

            Timer explosionTimer = new Timer { Interval = 2500 };
            explosionTimer.Tick += (sender, e) =>
            {
                explosionTimer.Stop();
                Controls.Remove(explosion);
                explosion.Dispose();
            };
            explosionTimer.Start();
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
            shieldTimer.Stop();

            ResetRocket();
            Statistics.GamesPlayed++;

            if (score > Statistics.HighestScore)
                Statistics.HighestScore = score;

            Statistics.TotalUFOsDestroyed += ufoDestroyedThisGame;

            Statistics.Save();

            GameOver go = new GameOver();
            go.Show();
            this.Close();
        }


        private void ResetRocket()
        {
            if (rocket != null)
            {
                Controls.Remove(rocket);
                rocket.Dispose();
            }
            rocket = new PictureBox();
            rocket.Size = new Size(100, 100);
            rocket.Location = new Point(100, this.ClientSize.Height / 2 - 50);
            rocket.BackColor = Color.Transparent;
            Image rocketImage;
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
            else if (e.KeyCode == Keys.Escape)
            {
                PauseGame();
                
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
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            if (pictureBox9.Visible)
            {
                bool collisionDetected = false;

                foreach (var asteroid in currentAsteroids.ToList())
                {
                    if (rocket.Bounds.IntersectsWith(asteroid.Bounds))
                    {
                        collisionDetected = true;
                        break;
                    }
                }

                if (!collisionDetected)
                {
                    foreach (Control c in Controls)
                    {
                        if (c.Tag as string == "ufoLaser" && rocket.Bounds.IntersectsWith(c.Bounds))
                        {
                            collisionDetected = true;
                            break;
                        }
                    }
                }

                if (collisionDetected)
                {
                    pictureBox9.Visible = false;
                }
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (pictureBox8.Visible)
            {
                bool collisionDetected = false;

                foreach (var asteroid in currentAsteroids.ToList())
                {
                    if (rocket.Bounds.IntersectsWith(asteroid.Bounds))
                    {
                        collisionDetected = true;
                        break;
                    }
                }

                if (!collisionDetected)
                {
                    foreach (Control c in Controls)
                    {
                        if (c.Tag as string == "ufoLaser" && rocket.Bounds.IntersectsWith(c.Bounds))
                        {
                            collisionDetected = true;
                            break;
                        }
                    }
                }

                if (collisionDetected)
                {
                    pictureBox8.Visible = false;
                }
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            bool collisionDetected = false;

            foreach (var asteroid in currentAsteroids.ToList())
            {
                if (rocket.Bounds.IntersectsWith(asteroid.Bounds))
                {
                    collisionDetected = true;
                    break;
                }
            }

            if (!collisionDetected)
            {
                foreach (Control c in Controls)
                {
                    if (c.Tag as string == "ufoLaser" && rocket.Bounds.IntersectsWith(c.Bounds))
                    {
                        collisionDetected = true;
                        break;
                    }
                }
            }

            if (collisionDetected)
            {
                EndGame();
            }
        }
        private void SimulateHeartClick()
        {
            if (shieldActive) return;

            if (pictureBox9.Visible)
            {
                pictureBox9_Click(pictureBox9, EventArgs.Empty);
            }
            else if (pictureBox8.Visible)
            {
                pictureBox8_Click(pictureBox8, EventArgs.Empty);
            }
            else if (pictureBox7.Visible)
            {
                pictureBox7_Click(pictureBox7, EventArgs.Empty);
            }

            shieldActive = true;
            rocket.BackColor = Color.Transparent;
            shieldTimer.Start();
        }

        private void ShieldTimer_Tick(object sender, EventArgs e)
        {
            if (shieldActive)
            {
                shieldTimer.Stop();
                shieldActive = false;
                rocket.BackColor = Color.Transparent;
            }
        }

        private void PauseGame()
        {
            asteroidMovementTimer.Stop();
            asteroidSpawnTimer.Stop();
            laserTimer.Stop();
            scoreTimer.Stop();
            ufoSpawnTimer.Stop();
            ufoMovementTimer.Stop();
            ufoShootingTimer.Stop();
            shieldTimer.Stop();

            PauseMenu pauseMenu = new PauseMenu();

            pauseMenu.FormClosed += (s, e) =>
            {
                if (pauseMenu.ReturnToMainMenuRequested)
                {
                    asteroidMovementTimer.Stop();
                    if (spawningAsteroids) asteroidSpawnTimer.Stop();
                    laserTimer.Stop();
                    scoreTimer.Stop();
                    ufoSpawnTimer.Stop();
                    if (ufo != null && ufo.Left > this.ClientSize.Width - 500)
                        ufoMovementTimer.Stop();
                    if (ufoAlive) ufoShootingTimer.Stop();
                    if (shieldActive) shieldTimer.Stop();
                    this.Close();

                    Application.Exit();
                    MainMenu mainMenu = new MainMenu();
                    mainMenu.Show();
                }
                else if (pauseMenu.ResumeRequested)
                {
                    asteroidMovementTimer.Start();
                    if (spawningAsteroids) asteroidSpawnTimer.Start();
                    laserTimer.Start();
                    scoreTimer.Start();
                    ufoSpawnTimer.Start();
                    if (ufo != null && ufo.Left > this.ClientSize.Width - 500)
                        ufoMovementTimer.Start();
                    if (ufoAlive) ufoShootingTimer.Start();
                    if (shieldActive) shieldTimer.Start();
                }
            };
            pauseMenu.ShowDialog();
            scoreTimer.Tick += (s, e2) =>
            {
                score++;
                scoreLabel.Text = $"Score: {score}";
                if (score == 50)
                {
                    SpawnAstronaut();
                }
            };

            
        }

        private void SpawnAstronaut()
        {
            PictureBox astronaut = new PictureBox
            {
                Size = new Size(70, 70),
                BackColor = Color.Transparent,
                Image = Properties.Resources.astronaut_removebg_preview,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(ClientSize.Width, rand.Next(scoreLabelHeight + 100, ClientSize.Height - 100))
            };

            astronaut.BringToFront();
            Controls.Add(astronaut);

            Timer astronautTimer = new Timer { Interval = 16 };
            astronautTimer.Tick += (sender, e) =>
            {
                astronaut.Left -= 40; 
                astronaut.Invalidate();

                if (astronaut.Left < -astronaut.Width)
                {
                    astronautTimer.Stop();
                    Controls.Remove(astronaut);
                    astronaut.Dispose();
                }

                if (rocket.Bounds.IntersectsWith(astronaut.Bounds))
                {
                    astronautTimer.Stop();
                    Controls.Remove(astronaut);
                    astronaut.Dispose();
                    Statistics.AstronautsSaved++;
                }
            };
            astronautTimer.Start();
        }

        
    }
}   

