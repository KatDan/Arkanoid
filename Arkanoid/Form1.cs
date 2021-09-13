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
    /// <summary>
    /// A class for managing Form
    /// </summary>
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
        PaddleGUI paddleGUI;
        /// <summary>
        /// Manages visualization of Arkanoid.Ball instances.
        /// </summary>
        BallGUI[] ballsGUI;
        /// <summary>
        /// Manages visualization of the brick map in the level.
        /// </summary>
        BrickMapGUI brickMapGUI;
        /// <summary>
        /// Manages visualization of Arkanoid.PowerUp instance.
        /// </summary>
        PowerUpGUI powerUpGUI;
        
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
        /// the important components of the GUI.
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

            initializePowerUpGUI();
            initializePaddleGUI();
            initializeBallGUI();
            initializeBrickMapGUI();
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
        /// Initializes PaddleGUI and adds its PictureBox to Panel.Controls.
        /// </summary>
        private void initializePaddleGUI()
        {
            paddleGUI = new PaddleGUI(panel, game.paddle);
            panel.Controls.Add(paddleGUI.pictureBox);
            paddleGUI.pictureBox.BringToFront();
            //paddleGUI.pictureBox.Parent = panel;
        }

        /// <summary>
        /// Initializes BallGUI and adds its PictureBox to Panel.Controls.
        /// </summary>
        private void initializeBallGUI()
        {
            ballsGUI = new BallGUI[game.maxBallsCount];
            for(int i = 0; i < game.balls.Length; i++)
            {
                Ball ball = game.balls[i];
                if (ball == null) continue;

                BallGUI ballGUI = new BallGUI(panel, ball);
                panel.Controls.Add(ballGUI.pictureBox);
                ballsGUI[i] = ballGUI;
                ballGUI.pictureBox.BringToFront();
            }
        }

        /// <summary>
        /// Initializes BrickMapGUI and adds its PictureBoxes to Panel.Controls.
        /// </summary>
        private void initializeBrickMapGUI()
        {
            brickMapGUI = new BrickMapGUI(panel, resourceManager, game);
            AddBrickMapGUIToControls();
        }

        /// <summary>
        /// Initializes PowerUpGUI and adds its PictureBox to Panel.Controls.
        /// </summary>
        private void initializePowerUpGUI()
        {
            if (game.powerUp == null) return;

            powerUpGUI = new PowerUpGUI(panel, game.powerUp);
            panel.Controls.Add(powerUpGUI.pictureBox);
            powerUpGUI.pictureBox.BringToFront();
        }


        /// <summary>
        /// Removes PictureBox representing PowerUp from Panel.Controls and clears PowerUpGUI.
        /// </summary>
        private void clearPowerUpGUI()
        {
            if (powerUpGUI == null) return;
            panel.Controls.Remove(powerUpGUI.pictureBox);
            powerUpGUI = null;
        }

        /// <summary>
        /// Adds PictureBoxes representing instances of Arkanoid.Brick in the brick map to Panel.Controls.
        /// </summary>
        private void AddBrickMapGUIToControls()
        {
            for (int line = 0; line < brickMapGUI.bricksPicBoxes.Count; line++)
            {
                for (int column = 0; column < brickMapGUI.bricksPicBoxes[0].Count; column++)
                {
                    PictureBox pBox = brickMapGUI.bricksPicBoxes[line][column];
                    if (pBox == null) continue;

                    panel.Controls.Add(pBox);
                    pBox.BringToFront();
                }
            }
        }

        /// <summary>
        /// Removes PictureBoxes representing Bricks from Panel.Controls and clears BrickMapGUI.
        /// </summary>
        private void clearBrickMapGUI()
        {
            for (int line = 0; line < brickMapGUI.bricksPicBoxes.Count; line++)
            {
                for (int column = 0; column < brickMapGUI.bricksPicBoxes[0].Count; column++)
                {
                    PictureBox pBox = brickMapGUI.bricksPicBoxes[line][column];
                    panel.Controls.Remove(pBox);
                }
            }
            brickMapGUI.bricksPicBoxes.Clear();
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
                        clearBrickMapGUI();
                        brickMapGUI.updatePicBoxes(game);
                        AddBrickMapGUIToControls();
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
                        clearBrickMapGUI();
                        brickMapGUI.updatePicBoxes(game);
                        AddBrickMapGUIToControls();
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
            // if brick was hit in gameTick, update BrickMapGUI
            if(game.eventManager.brickHit == true)
            {
                brickMapGUI.update();
                updateScoreLabel();
            }
            
            paddleGUI.updatePosition();
            
            for(int i = 0; i < ballsGUI.Length; i++)
            {
                BallGUI ballGUI = ballsGUI[i];

                //some change in number of balls has occured
                if((ballGUI == null && game.balls[i] != null) | (ballGUI != null && !ReferenceEquals(ballGUI.ball, game.balls[i])))
                {
                    //ballGUI no longer represents alive ball
                    if(ballGUI != null && game.balls[i] == null)
                    {
                        panel.Controls.Remove(ballGUI.pictureBox);
                        ballsGUI[i] = null;
                        continue;
                    }
                    //ballGUI needs to create a GUI for new ball
                    if(ballGUI == null && game.balls[i] != null)
                    {
                        ballsGUI[i] = new BallGUI(panel, game.balls[i]);
                        panel.Controls.Add(ballsGUI[i].pictureBox);
                    }
                    // a change has occured in the order of the balls and the GUI needs to be refreshed
                    if(ballGUI != null && !ReferenceEquals(ballGUI.ball, game.balls[i]))
                    {
                        ballsGUI[i].ball = game.balls[i];
                    }
                    
                }
                
                if(ballGUI != null) ballGUI.updatePosition();
            }
            // managing power ups
            if (game.powerUpPresent)
            {
                if (powerUpGUI == null) powerUpGUI = new PowerUpGUI(panel, game.powerUp);
                if (game.eventManager.powerupState == EventManager.PowerUpState.EXISTS
                    || game.eventManager.powerupState == EventManager.PowerUpState.FALLING)
                {
                    powerUpGUI.pictureBox.Visible = true;
                }
                else if(game.eventManager.powerupState == EventManager.PowerUpState.TAKEN
                    || game.eventManager.powerupState == EventManager.PowerUpState.NONE)
                {
                    powerUpGUI.pictureBox.Visible = false;
                }
                
                powerUpGUI.updatePosition();
            }
            else clearPowerUpGUI();
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
            foreach(BallGUI ballGUI in ballsGUI)
            {
                if(ballGUI != null) ballGUI.pictureBox.BringToFront();
            }
        }

        /// <summary>
        /// Manages keyboard events when the key is down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyIsDown(object sender, KeyEventArgs e)
        {

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Right)
            {
                if (game.paddle.x + game.paddle.Width / 2 >= Width - 18) return true;

                game.paddle.x += game.paddle.speed;
                if (game.paddle.holdsBall)
                {
                    game.balls[0].x += game.paddle.speed;
                    ballsGUI[0].updatePosition();
                }
                paddleGUI.updatePosition();
            }
            else if (keyData == Keys.Left)
            {
                if (game.paddle.x - game.paddle.Width / 2 <= 1) return true;

                game.paddle.x -= game.paddle.speed;
                if (game.paddle.holdsBall)
                {
                    game.balls[0].x -= game.paddle.speed;
                    ballsGUI[0].updatePosition();
                }
                paddleGUI.updatePosition();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
