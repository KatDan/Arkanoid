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
        internal Panel panel;

        internal int brickUpperIndentation = 50;

        public static ResourceManager resourceManager = new ResourceManager("Arkanoid.Properties.Resources", typeof(Resources).Assembly);

        public Label infoLabel = new Label
        {
            Size = new Size(400, 100),
            Location = new Point(120, 200),
            Font = new Font("Press Start 2P", 15),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
        };

        public PictureBox[] hearts;

        Game game;

        PaddleUI paddleUI;
        BallUI[] ballsUI;
        BrickMapUI brickMapUI;
        PowerUpUI powerUpUI;
        
        public GameForm()
        {
            InitializeComponent();
            
            infoLabel.Parent = this;
            Controls.Add(infoLabel);

            initializeGame();
        }

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

        //ok
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

        //ok
        private void initializePaddleUI()
        {
            paddleUI = new PaddleUI(panel, game.paddle);
            panel.Controls.Add(paddleUI.pictureBox);
            paddleUI.pictureBox.BringToFront();
            //paddleUI.pictureBox.Parent = panel;
        }

        //ok
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

        //ok
        private void initializeBrickMapUI()
        {
            brickMapUI = new BrickMapUI(panel, resourceManager, game);
            AddBrickMapUIToControls();
        }

        private void initializePowerUpUI()
        {
            if (game.powerUp == null) return;

            powerUpUI = new PowerUpUI(panel, game.powerUp);
            panel.Controls.Add(powerUpUI.pictureBox);
            powerUpUI.pictureBox.BringToFront();
        }

        private void clearPowerUpUI()
        {
            if (powerUpUI == null) return;
            panel.Controls.Remove(powerUpUI.pictureBox);
            powerUpUI = null;
        }

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

        private void updateScoreLabel()
        {
            scoreLabel.Text = String.Format("score: {0:D6}", game.score);
        }

        private void updateLevelLabel()
        {
            levelLabel.Text = "level: " + game.level.ToString();
        }

        private void updateHearts()
        {
            for(int i = 0; i < hearts.Length; i++)
            {
                if (i < game.lives) hearts[i].Visible = true;
                else hearts[i].Visible = false;
            }
        }

        private void tick(object sender, EventArgs e)
        {
            game.gameTick();
            if(game.eventManager.gameStopper != EventManager.GameStopper.NONE)
            {
                timer.Stop();
                switch (game.eventManager.gameStopper)
                {
                    case EventManager.GameStopper.LEVELUP:
                        showInfoLabel("level " + game.level.ToString());
                        updateLevelLabel();
                        clearBrickMapUI();
                        brickMapUI.updatePicBoxes(game);
                        AddBrickMapUIToControls();
                        break;
                    case EventManager.GameStopper.LIFEDOWN:
                        updateHearts();
                        showInfoLabel(game.lives.ToString() + (game.lives == 1 ? " life left" : " lives left"));
                        updateScoreLabel();
                        break;
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
                    case EventManager.GameStopper.WIN:
                        updateScoreLabel();
                        showInfoLabel("YOU WON!\n\nFINAL SCORE:" + game.score.ToString());
                        timer.Stop();
                        return;
                }
            }
            if(game.eventManager.brickHit == true)
            {
                brickMapUI.update();
                updateScoreLabel();
            }
            paddleUI.updatePosition();
            
            for(int i = 0; i < ballsUI.Length; i++)
            {
                BallUI ballUI = ballsUI[i];

                //some change has occured
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
                    //a change has occured in the order of the balls and the UI needs to be refreshed
                    if(ballUI != null && !ReferenceEquals(ballUI.ball, game.balls[i]))
                    {
                        ballsUI[i].ball = game.balls[i];
                    }
                    
                }
                
                if(ballUI != null) ballUI.updatePosition();
            }
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
            game.eventManager.reset();
        }

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

        private void GameForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            /*if(e.KeyData == Keys.Right || e.KeyData == Keys.Left)
            {
                e.IsInputKey = true;
            }*/

            if (e.KeyData == Keys.Right)
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
            else if (e.KeyData == Keys.Left)
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
    }
}
