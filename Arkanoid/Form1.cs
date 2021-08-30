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
        List<BallUI> ballsUI;
        BrickMapUI brickMapUI;
        
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
                Size = new Size(640, 480),
                Location = new Point(0, 50),
                BackColor = Color.Black,
            };
            Controls.Add(panel);

            game = new Game(panel.Bounds);

            this.levelLabel.Text = "level: " + game.level.ToString();
            this.scoreLabel.Text = String.Format("score: {0:D6}", game.score);

            initializeHearts();


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
            Controls.Add(paddleUI.pictureBox);
            paddleUI.pictureBox.BringToFront();
        }

        //ok
        private void initializeBallUI()
        {
            ballsUI = new List<BallUI>();
            foreach(Ball ball in game.balls)
            {
                BallUI ballUI = new BallUI(panel, ball, game.paddle);
                Controls.Add(ballUI.pictureBox);
                ballsUI.Add(ballUI);
                ballUI.pictureBox.BringToFront();
            }
        }

        //ok
        private void initializeBrickMapUI()
        {
            brickMapUI = new BrickMapUI(panel, resourceManager, game);
            AddBrickMapUIToControls();
        }

        private void AddBrickMapUIToControls()
        {
            for (int line = 0; line < brickMapUI.bricksPicBoxes.Count; line++)
            {
                for (int column = 0; column < brickMapUI.bricksPicBoxes[0].Count; column++)
                {
                    PictureBox pBox = brickMapUI.bricksPicBoxes[line][column];
                    Controls.Add(pBox);
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
                    Controls.Remove(pBox);
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
                        paddleUI.updatePosition();
                        clearBrickMapUI();
                        brickMapUI.updatePicBoxes(game);
                        AddBrickMapUIToControls();
                        break;
                }
            }
            if(game.eventManager.brickHit == true)
            {
                brickMapUI.update();
                updateScoreLabel();
            }
            paddleUI.updatePosition();
            foreach(BallUI ballUI in ballsUI)
            {
                ballUI.updatePosition();
            }
            game.eventManager.reset();
        }

        private void showInfoLabel(string text)
        {
            infoLabel.Visible = true;
            infoLabel.BringToFront();
            infoLabel.Text = text;
            foreach(BallUI ballUI in ballsUI)
            {
                ballUI.pictureBox.BringToFront();
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
                timer.Start();
            }
        }

    }
}
