using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public class Game
    {
        internal EventManager eventManager = new EventManager();

        Rectangle panelBounds;

        int maxNumberOfLevels = 5;

        int[] levelScores;

        internal bool playerLives = true;

        internal int bricksPerLine = 8;

        internal int lives = 3;

        internal Paddle paddle;
        internal List<Ball> balls;

        internal int score = 0;
        internal int level = 1;

        internal List<List<Brick>> levelBrickMap;
        internal Coords recentlyHitBrick = new Coords();

        private int brickCount = 0;

        private Collider collider;
        public int brickWidth
        {
            get { return (int)((panelBounds.Width) / bricksPerLine); }
        }
        public int brickHeight = 20;

        public Game(Rectangle bounds) 
        {
            panelBounds = bounds;
            collider = new Collider(panelBounds,this);
            balls = new List<Ball>();
            levelScores = new int[maxNumberOfLevels+1];
            initializeGame();
        }

        private void initializeGame()
        {
            List<List<int>> brickMapData = loadLevelBrickInfo(1);
            levelBrickMap = initializeLevelBricks(brickMapData);

            initializePaddle();
            initializeBall();
        }

        private List<List<int>> loadLevelBrickInfo(int level)
        {
            List<List<int>> result = new List<List<int>>();
            string filename = "level" + level.ToString() + ".csv";
            using (var reader = new StreamReader(@filename))
            {
                bricksPerLine = 0;
                while (!reader.EndOfStream)
                {
                    List<int> brickLine = new List<int>();
                    var line = reader.ReadLine().Split(';');
                    if (bricksPerLine == 0) bricksPerLine = line.Length;
                    if (line.Length != bricksPerLine)
                    {
                        throw new LevelFormatException("The lines must be of the equal length.");
                    }
                    foreach (string item in line)
                    {
                        int thickness = 0;
                        if (item != "")
                        {
                            if (!int.TryParse(item, out thickness))
                            {
                                throw new LevelFormatException("Integer needed for defining the thickness of a brick.");
                            }
                        }
                        brickLine.Add(thickness);
                    }
                    result.Add(brickLine);
                }
            }
            if (result.Count < 1) throw new LevelFormatException("At least one line of blocks is required.");
            return result;
        }

        private List<List<Brick>> initializeLevelBricks(List<List<int>> brickMap)
        {
            List<List<Brick>> result = new List<List<Brick>>();
            for (int line = 0; line < brickMap.Count; line++)
            {
                List<Brick> brickLine = new List<Brick>();
                for (int column = 0; column < brickMap[line].Count; column++)
                {
                    int thickness = brickMap[line][column];
                    if (thickness != 0)
                    {
                        brickLine.Add(new Brick(thickness));
                        brickCount++;
                    }
                    else brickLine.Add(null);
                }
                result.Add(brickLine);
            }
            return result;
        }
    
        private void initializePaddle()
        {
            int paddleWidth = 100;
            int paddleHeight = 20;
            paddle = new Paddle(panelBounds.X + panelBounds.Width/2,
                panelBounds.Y + panelBounds.Height - 20,
                paddleWidth, paddleHeight);
        }

        private void initializeBall()
        {
            int ballRadius = 9;
            balls.Add(new Ball(ballRadius,paddle));
            paddle.holdsBall = true;
        }

        public void resetPosition(int level = 1)
        {
            levelBrickMap.Clear();
            List<List<int>> brickMapData = loadLevelBrickInfo(level);
            levelBrickMap = initializeLevelBricks(brickMapData);

            resetPaddleAndBallPosition();

            this.level = level;
            score = levelScores[level];
        }

        public void resetPaddleAndBallPosition()
        {
            while (balls.Count > 1)
            {
                balls.RemoveAt(balls.Count - 1);
            }

            paddle.x = panelBounds.X + panelBounds.Width / 2;
            balls[0].x = panelBounds.X + panelBounds.Width / 2;
            balls[0].y = paddle.y - paddle.Height / 2 - balls[0].radius;

            paddle.holdsBall = true;
        }

        public void resetGame()
        {
            eventManager.reset();
            resetPosition(1);
            score = 0;
        }

        private void levelUp()
        {
            //eventManager.reset();
            levelScores[level] = score;
            resetPosition(++level);
        }

        public void gameTick()
        {
            if (paddle.holdsBall) return;
            for (int index = 0; index < balls.Count; index++)
            {
                Ball ball = balls[index];

                // ball is falling down
                if (ball.Angle <= 0)
                {
                    if (collider.ballFallsDown(ball))
                    {
                        if (balls.Count > 1)
                        {
                            balls.RemoveAt(index);
                            continue;
                        }
                        else
                        {
                            // ball falls down, heart down or game over
                            lives--;
                            if (lives > 0)
                            {
                                resetPaddleAndBallPosition();
                                eventManager.gameStopper = EventManager.GameStopper.LIFEDOWN;
                                //score = levelScores[level-1];
                                return;
                            }
                            else
                            {
                                eventManager.gameStopper = EventManager.GameStopper.GAMEOVER;
                                playerLives = false;
                                resetGame();
                                return;
                            }
                        }
                    }
                    if (collider.ballHitsPaddle(ball,paddle))
                    {
                        collider.bounceOnPaddle(ball);
                    }
                }
                // ball is going up
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
                        eventManager.brickHit = true;
                        recentlyHitBrick.row = row;
                        recentlyHitBrick.column = column;

                        score += brick.Score;
                        
                        // brick is destroyed
                        if (!brick.isAlive)
                        {
                            brickCount--;
                            // level up
                            if (brickCount == 0)
                            {
                                eventManager.gameStopper = EventManager.GameStopper.LEVELUP;
                                recentlyHitBrick.update(-1, -1);
                                levelUp();
                            }
                        }
                    }
                    collider.bounceVertically(ball);
                }
                ball.move();
            }
        }
    }
}
