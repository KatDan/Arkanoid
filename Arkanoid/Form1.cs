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

        internal bool playerLives = true;

        internal int bricksPerLine = 8;

        int lives = 3;

        internal int brickUpperIndentation = 50;
        
        public int brickWidth
        {
            get { return (int)((Width-18) / bricksPerLine); }
        }
        public int brickHeight = 20;

        internal Paddle paddle;
        internal List<Ball> balls;

        int score = 0;
        int level = 1;

        internal List<List<Brick>> levelBrickMap;

        private int brickCount;

        private Collider collider;

        public static ResourceManager resourceManager = new System.Resources.ResourceManager("Arkanoid.Properties.Resources", typeof(Resources).Assembly);
        public GameForm()
        {
            InitializeComponent();
            balls = new List<Ball>();
            collider = new Collider(this);
            initializeGame();
        }

        private void initializeGame()
        {
            this.levelLabel.Text = "level: " + level.ToString();
            this.scoreLabel.Text = String.Format("score: {0:D6}", score);
            List<List<int>> brickMapData = loadLevelBrickInfo(1);
            levelBrickMap = initializeLevelBricks(brickMapData);

            panel = new Panel
            {
                Size = new Size(640, 480),
                Location = new Point(0, 50),
                BackColor = Color.Black,
            };
            Controls.Add(panel);

            initializePaddle();
            initializeBall();
            paddle.holdsBall = true;
        }

        private void resetPosition(int level = 1)
        {
            List<List<int>> brickMapData = loadLevelBrickInfo(level);
            levelBrickMap = initializeLevelBricks(brickMapData);

            paddle.X = (Width - 18) / 2;
            balls[0].X = (Width - 18) / 2;
            balls[0].Y = paddle.Y - paddle.Height / 2 - balls[0].radius;

            paddle.holdsBall = true;

            if(level == 1)
            {
                score = 0;
                level = 1;
                scoreLabel.Text = String.Format("score: {0:D6}", score);
            }
            levelLabel.Text = "level: " + level.ToString();
        }

        private List<List<int>> loadLevelBrickInfo(int level)
        {
            List<List<int>> result = new List<List<int>>();
            string filename = "level" + level.ToString() + ".csv";
            using(var reader = new StreamReader(@filename))
            {
                bricksPerLine = 0;
                while (!reader.EndOfStream)
                {
                    List<int> brickLine = new List<int>();
                    var line = reader.ReadLine().Split(';');
                    if (bricksPerLine == 0) bricksPerLine = line.Length;
                    if(line.Length != bricksPerLine)
                    {
                        throw new LevelFormatException();
                    }
                    foreach (string item in line)
                    {
                        int thickness = 0;
                        if (item != "")
                        {
                            if (!int.TryParse(item, out thickness))
                            {
                                throw new LevelFormatException();
                            }
                        }
                        brickLine.Add(thickness);
                    }
                    result.Add(brickLine);
                }
            }
            if (result.Count < 1) throw new LevelFormatException();
            return result;
        }

        private List<List<Brick>> initializeLevelBricks(List<List<int>> brickMap)
        {
            List<List<Brick>> result = new List<List<Brick>>();
            for(int line = 0; line < brickMap.Count; line++)
            {
                List<Brick> brickLine = new List<Brick>();
                for(int column = 0; column < brickMap[line].Count; column++)
                {
                    int thickness = brickMap[line][column];
                    if(thickness != 0)
                    {
                        PictureBox brickPictureBox = new PictureBox
                        {
                            Size = new Size(brickWidth, brickHeight),
                            Location = new Point(column * brickWidth, brickUpperIndentation + line * brickHeight),
                            BackgroundImageLayout = ImageLayout.Stretch,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Parent = panel
                        };
                        object obj = resourceManager.GetObject("brick" + thickness.ToString());
                        brickPictureBox.BackgroundImage = (System.Drawing.Bitmap)obj;

                        Controls.Add(brickPictureBox);
                        brickPictureBox.BringToFront();
                        brickLine.Add(new Brick(brickPictureBox, thickness));
                        brickCount++;
                    }
                    else
                    {
                        brickLine.Add(null);
                    }
                }
                result.Add(brickLine);
            }
            return result;
        }

        private void initializePaddle()
        {
            int paddleWidth = 100;
            int paddleHeight = 20;
            PictureBox paddlePicBox = new PictureBox
            {
                Size = new Size(paddleWidth, paddleHeight),
                Location = new Point(panel.Width/2 - paddleWidth/2 , Height - 50 - paddleHeight),
                Image = Properties.Resources.paddle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Parent = panel
            };
            Controls.Add(paddlePicBox);
            paddlePicBox.BringToFront();
            paddle = new Paddle(paddlePicBox);
        }

        private void initializeBall()
        {
            int ballSize = 18;
            PictureBox ballPicBox = new PictureBox
            {
                Size = new Size(ballSize,ballSize),
                Location = new Point(panel.Width / 2 - ballSize/2, Height - 50 - paddle.Height - ballSize),
                Image = Properties.Resources.ball,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Parent = panel
            };
            Controls.Add(ballPicBox);
            ballPicBox.BringToFront();
            balls.Add(new Ball(ballPicBox));
        }

        private void tick(object sender, EventArgs e)
        {
            if (paddle.holdsBall) return;
            for(int index = 0; index < balls.Count; index++)
            {
                Ball ball = balls[index];
                if(ball.Angle <= 0)
                {
                    if (collider.ballFallsDown(ball))
                    {
                        if(balls.Count > 1)
                        {
                            balls.RemoveAt(index);
                            continue;
                        }
                        else
                        {
                            //game over
                            timer.Stop();
                            infoLabel.BringToFront();
                            infoLabel.Visible = true;
                            infoLabel.Text = "Game Over";
                            playerLives = false;
                            break;
                        }
                    }
                    if (collider.ballHitsPaddle(ball))
                    {
                        collider.bounceOnPaddle(ball);
                    }
                }
                else
                {
                    if (collider.ballHitsUpperBound(ball)) collider.bounceVertically(ball);
                }
                
                if (collider.ballHitsWall(ball)) collider.bounceHorizontally(ball);
                if (collider.ballHitsBrick(ball, out int row, out int column))
                { 
                    if (row == -1 || column == -1) break;
                    Brick brick = levelBrickMap[row][column];
                    if (brick != null && brick.isAlive)
                    {
                        brick.hit();
                        if (!brick.isAlive)
                        {
                            brickCount--;
                            score += brick.Score;
                            scoreLabel.Text = String.Format("score: {0:D6}", score);

                            if(brickCount == 0)
                            {
                                timer.Stop();
                                //going to the next level
                                showInfoLabel("level up!");
                                resetPosition(++level);
                                showInfoLabel("level " + level.ToString());
                            }
                        }
                    }
                    collider.bounceVertically(ball);
                }
                ball.move();
                
            }
        }

        private void showInfoLabel(string text)
        {
            infoLabel.Visible = true;
            infoLabel.BringToFront();
            infoLabel.Text = text;
        }

        private void keyIsDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Right)
            {
                if (paddle.X + paddle.Width / 2 >= Width - 18) return;

                paddle.X += paddle.speed;
                if (paddle.holdsBall) balls[0].X += paddle.speed;
            }
            else if(e.KeyData == Keys.Left)
            {
                if (paddle.X - paddle.Width / 2 <= 1) return;

                paddle.X -= paddle.speed;
                if (paddle.holdsBall) balls[0].X -= paddle.speed;
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
                if (!playerLives)
                {
                    playerLives = true;
                    resetPosition();
                    return;
                }
                if (paddle.holdsBall) paddle.holdsBall = false;
                timer.Start();
            }
        }

    }
}
