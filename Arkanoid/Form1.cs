using Arkanoid.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public partial class GameForm : Form
    {
        /// <summary>
        /// System.Windows.Forms.Panel instance that displays the game level.
        /// </summary>
        internal Panel panel;

        /// <summary>
        /// Manages resources in the project.
        /// </summary>
        public static ResourceManager resourceManager = new ResourceManager("Arkanoid.Properties.Resources", typeof(Resources).Assembly);

        /// <summary>
        /// Label that contains information about the state of the game, e.g. game over, 2 lives left etc.
        /// </summary>
        public Label infoLabel = new Label
        {
            Size = new Size(400, 100),
            Location = new Point(120, 200),
            Font = new Font("Press Start 2P", 15),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
        };

        /// <summary>
        /// Array of picture boxes displaying how many lives the player has left.
        /// </summary>
        public PictureBox[] hearts;

        /// <summary>
        /// Arkanoid.Game instance that contains all the logic of the game.
        /// </summary>
        Game game;

        /// <summary>
        /// Manages visualization of Arkanoid.Paddle instance.
        /// </summary>
        PaddleUI paddleUI;
        /// <summary>
        /// Manages visualization of Arkanoid.Ball instances.
        /// </summary>
        BallUI[] ballsUI;
        /// <summary>
        /// Manages visualization of the brick map in the level.
        /// </summary>
        BrickMapUI brickMapUI;
        /// <summary>
        /// Manages visualization of Arkanoid.PowerUp instance.
        /// </summary>
        PowerUpUI powerUpUI;
        
        /// <summary>
        /// Constructor that initializes Windows Form.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            
            infoLabel.Parent = this;
            Controls.Add(infoLabel);

            initializeGame();
        }

        /// <summary>
        /// Creates Panel that will visualize the game and initializes all
        /// the important components of the UI.
        /// </summary>
        private void initializeGame()
        {
            panel = new Panel
            {
                Size = new Size(640, 470),
                Location = new Point(0, 50),
                BackColor = Color.Black,
            };
            Controls.Add(panel);

            game = new Game(panel.Bounds);

            this.levelLabel.Text = "level: " + game.level.ToString();
            this.scoreLabel.Text = String.Format("score: {0:D6}", game.score);

            initializeHearts();

            initializePowerUpUI();
            initializePaddleUI();
            initializeBallUI();
            initializeBrickMapUI();
        }

        /// <summary>
        /// Initializes hearts that show how many lives the player has left.
        /// </summary>
        public void initializeHearts()
        {
            hearts = new PictureBox[game.lives];
            for(int index = 0; index < hearts.Length; index++)
            {
                PictureBox life = new PictureBox
                {
                    Size = new Size(20,20),
                    Location = new Point(10 + index*22,25),
                    Image = Properties.Resources.heart,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                Controls.Add(life);
                hearts[index] = life;
            }
        }

        /// <summary>
        /// Initializes PaddleUI and adds its PictureBox to Panel.Controls.
        /// </summary>
        private void initializePaddleUI()
        {
            paddleUI = new PaddleUI(panel, game.paddle);
            panel.Controls.Add(paddleUI.pictureBox);
            paddleUI.pictureBox.BringToFront();
            //paddleUI.pictureBox.Parent = panel;
        }

        /// <summary>
        /// Initializes BallUI and adds its PictureBox to Panel.Controls.
        /// </summary>
        private void initializeBallUI()
        {
            ballsUI = new BallUI[game.maxBallsCount];
            for(int i = 0; i < game.balls.Length; i++)
            {
                Ball ball = game.balls[i];
                if (ball == null) continue;

                BallUI ballUI = new BallUI(panel, ball);
                panel.Controls.Add(ballUI.pictureBox);
                ballsUI[i] = ballUI;
                ballUI.pictureBox.BringToFront();
            }
        }

        /// <summary>
        /// Initializes BrickMapUI and adds its PictureBoxes to Panel.Controls.
        /// </summary>
        private void initializeBrickMapUI()
        {
            brickMapUI = new BrickMapUI(panel, resourceManager, game);
            AddBrickMapUIToControls();
        }

        /// <summary>
        /// Initializes PowerUpUI and adds its PictureBox to Panel.Controls.
        /// </summary>
        private void initializePowerUpUI()
        {
            if (game.powerUp == null) return;

            powerUpUI = new PowerUpUI(panel, game.powerUp);
            panel.Controls.Add(powerUpUI.pictureBox);
            powerUpUI.pictureBox.BringToFront();
        }


        /// <summary>
        /// Removes PictureBox representing PowerUp from Panel.Controls and clears PowerUpUI.
        /// </summary>
        private void clearPowerUpUI()
        {
            if (powerUpUI == null) return;
            panel.Controls.Remove(powerUpUI.pictureBox);
            powerUpUI = null;
        }

        /// <summary>
        /// Adds PictureBoxes representing instances of Arkanoid.Brick in the brick map to Panel.Controls.
        /// </summary>
        private void AddBrickMapUIToControls()
        {
            for (int line = 0; line < brickMapUI.bricksPicBoxes.Count; line++)
            {
                for (int column = 0; column < brickMapUI.bricksPicBoxes[0].Count; column++)
                {
                    PictureBox pBox = brickMapUI.bricksPicBoxes[line][column];
                    if (pBox == null) continue;

                    panel.Controls.Add(pBox);
                    pBox.BringToFront();
                }
            }
        }

        /// <summary>
        /// Removes PictureBoxes representing Bricks from Panel.Controls and clears BrickMapUI.
        /// </summary>
        private void clearBrickMapUI()
        {
            for (int line = 0; line < brickMapUI.bricksPicBoxes.Count; line++)
            {
                for (int column = 0; column < brickMapUI.bricksPicBoxes[0].Count; column++)
                {
                    PictureBox pBox = brickMapUI.bricksPicBoxes[line][column];
                    panel.Controls.Remove(pBox);
                }
            }
            brickMapUI.bricksPicBoxes.Clear();
        }

        /// <summary>
        /// Updates score label.
        /// </summary>
        private void updateScoreLabel()
        {
            scoreLabel.Text = String.Format("score: {0:D6}", game.score);
        }

        /// <summary>
        /// Updates level label.
        /// </summary>
        private void updateLevelLabel()
        {
            levelLabel.Text = "level: " + game.level.ToString();
        }

        /// <summary>
        /// Updates the number of hearts according to player's lives left.
        /// </summary>
        private void updateHearts()
        {
            for(int i = 0; i < hearts.Length; i++)
            {
                if (i < game.lives) hearts[i].Visible = true;
                else hearts[i].Visible = false;
            }
        }

        /// <summary>
        /// Tick of Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tick(object sender, EventArgs e)
        {
            // tick in Game
            game.gameTick();
            // if there is reason to stop the game, stop the game
            if(game.eventManager.gameStopper != EventManager.GameStopper.NONE)
            {
                timer.Stop();
                switch (game.eventManager.gameStopper)
                {
                    // level up
                    case EventManager.GameStopper.LEVELUP:
                        showInfoLabel("level " + game.level.ToString());
                        updateLevelLabel();
                        clearBrickMapUI();
                        brickMapUI.updatePicBoxes(game);
                        AddBrickMapUIToControls();
                        break;
                    // life down
                    case EventManager.GameStopper.LIFEDOWN:
                        updateHearts();
                        showInfoLabel(game.lives.ToString() + (game.lives == 1 ? " life left" : " lives left"));
                        updateScoreLabel();
                        break;
                    // game over
                    case EventManager.GameStopper.GAMEOVER:
                        showInfoLabel("Game Over");
                        updateHearts();
                        updateLevelLabel();
                        updateScoreLabel();
                        clearBrickMapUI();
                        brickMapUI.updatePicBoxes(game);
                        AddBrickMapUIToControls();
                        game.eventManager.reset();
                        break;
                    // win!
                    case EventManager.GameStopper.WIN:
                        updateScoreLabel();
                        showInfoLabel("YOU WON!\n\nFINAL SCORE:" + game.score.ToString());
                        timer.Stop();
                        return;
                }
            }
            // if brick was hit in gameTick, update BrickMapUI
            if(game.eventManager.brickHit == true)
            {
                brickMapUI.update();
                updateScoreLabel();
            }
            
            paddleUI.updatePosition();
            
            for(int i = 0; i < ballsUI.Length; i++)
            {
                BallUI ballUI = ballsUI[i];

                //some change in number of balls has occured
                if((ballUI == null && game.balls[i] != null) | (ballUI != null && !ReferenceEquals(ballUI.ball, game.balls[i])))
                {
                    //ballUI no longer represents alive ball
                    if(ballUI != null && game.balls[i] == null)
                    {
                        panel.Controls.Remove(ballUI.pictureBox);
                        ballsUI[i] = null;
                        continue;
                    }
                    //ballUI needs to create a UI for new ball
                    if(ballUI == null && game.balls[i] != null)
                    {
                        ballsUI[i] = new BallUI(panel, game.balls[i]);
                        panel.Controls.Add(ballsUI[i].pictureBox);
                    }
                    // a change has occured in the order of the balls and the UI needs to be refreshed
                    if(ballUI != null && !ReferenceEquals(ballUI.ball, game.balls[i]))
                    {
                        ballsUI[i].ball = game.balls[i];
                    }
                    
                }
                
                if(ballUI != null) ballUI.updatePosition();
            }
            // managing power ups
            if (game.powerUpPresent)
            {
                if (powerUpUI == null) powerUpUI = new PowerUpUI(panel, game.powerUp);
                if (game.eventManager.powerupState == EventManager.PowerUpState.EXISTS
                    || game.eventManager.powerupState == EventManager.PowerUpState.FALLING)
                {
                    powerUpUI.pictureBox.Visible = true;
                }
                else if(game.eventManager.powerupState == EventManager.PowerUpState.TAKEN
                    || game.eventManager.powerupState == EventManager.PowerUpState.NONE)
                {
                    powerUpUI.pictureBox.Visible = false;
                }
                
                powerUpUI.updatePosition();
            }
            else clearPowerUpUI();
            // resets eventManager from previous gameTick
            game.eventManager.reset();
        }

        /// <summary>
        /// Displays given text in central infoLabel.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        private void showInfoLabel(string text)
        {
            infoLabel.Visible = true;
            infoLabel.BringToFront();
            infoLabel.Text = text;
            foreach(BallUI ballUI in ballsUI)
            {
                if(ballUI != null) ballUI.pictureBox.BringToFront();
            }
        }

        /// <summary>
        /// Manages keyboard events when the key is down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyIsDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Right)
            {
                if (game.paddle.x + game.paddle.Width / 2 >= Width - 18) return;

                game.paddle.x += game.paddle.speed;
                if (game.paddle.holdsBall)
                {
                    game.balls[0].x += game.paddle.speed;
                    ballsUI[0].updatePosition();
                }
                paddleUI.updatePosition();
            }
            else if(e.KeyData == Keys.Left)
            {
                if (game.paddle.x - game.paddle.Width / 2 <= 1) return;

                game.paddle.x -= game.paddle.speed;
                if (game.paddle.holdsBall)
                {
                    game.balls[0].x -= game.paddle.speed;
                    ballsUI[0].updatePosition();
                }
                paddleUI.updatePosition();
            }
            
        }

        /// <summary>
        /// Manages keyboard events when the key is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyIsPressed(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 'p' || e.KeyChar == 'P')
            {
                if (timer.Enabled)
                {
                    timer.Stop();
                    showInfoLabel("paused");
                }
                else
                {
                    infoLabel.Visible = false;
                    timer.Start();
                }
            }
            if(e.KeyChar == ' ')
            {
                infoLabel.Visible = false;
                if (game.paddle.holdsBall) game.paddle.holdsBall = false;
                if(game.eventManager.gameStopper != EventManager.GameStopper.WIN) timer.Start();
            }
            
        }
    }
}
