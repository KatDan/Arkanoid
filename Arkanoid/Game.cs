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

        int maxNumberOfLevels = 3;

        int[] levelScores;

        internal bool playerLives = true;

        internal int bricksPerLine = 8;

        internal int maxLives = 3;
        internal int lives = 3;

        internal Paddle paddle;

        internal Ball[] balls;
        internal int maxBallsCount = 3;

        internal PowerUp powerUp;
        internal bool powerUpPresent = false;
        Random random;
        private int powerUpAppearanceCounter;

        private int powerUpEffectLength = 500;
        private bool powerUpActive = false;
        private int powerUpActiveTime = 0;

        internal int score = 0;
        internal int level = 1;

        internal List<List<Brick>> levelBrickMap;
        internal Coords[] recentlyHitBricks;

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
            balls = new Ball[maxBallsCount];
            levelScores = new int[maxNumberOfLevels+1];
            random = new Random();
            recentlyHitBricks = new Coords[maxBallsCount];
            for(int i = 0; i < recentlyHitBricks.Length; i++)
            {
                recentlyHitBricks[i] = new Coords();
            }

            powerUpAppearanceCounter = random.Next(500, 1000);
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
            paddle = new Paddle(panelBounds.Width/2,
                panelBounds.Height - 20,
                paddleWidth, paddleHeight);
        }

        private void initializeBall()
        {
            int ballRadius = 9;
            balls[0] = new Ball(ballRadius,paddle);
            paddle.holdsBall = true;
        }

        public void resetPosition(int level = 1)
        {
            levelBrickMap.Clear();
            List<List<int>> brickMapData = loadLevelBrickInfo(level);
            levelBrickMap = initializeLevelBricks(brickMapData);

            resetPaddleAndBallPosition();
            deactivatePowerUp();
            removePowerUp();

            this.level = level;
            score = levelScores[level];
        }

        public void resetPaddleAndBallPosition()
        {
            removeRedundantBalls();

            paddle.x = panelBounds.X + panelBounds.Width / 2;
            balls[0].x = panelBounds.X + panelBounds.Width / 2;
            balls[0].y = paddle.y - paddle.Height / 2 - balls[0].radius;

            paddle.holdsBall = true;
        }

        private void removeRedundantBalls()
        {
            Ball realBall = null;
            for (int i = 0; i < balls.Length; i++)
            {
                if(balls[i] != null)
                {
                    realBall = balls[i].copy();
                    break;
                }
            }
            balls = new Ball[maxBallsCount];
            balls[0] = realBall;
        }

        private int countBalls()
        {
            int count = 0;
            for(int i = 0; i < balls.Length; i++)
            {
                if (balls[i] != null) count++;
            }
            return count;
        }

        public void resetGame()
        {
            resetPosition(1);
            score = 0;
            playerLives = true;
            lives = maxLives;
            level = 1;
        }

        private void levelUp()
        {
            if(level == maxNumberOfLevels)
            {
                eventManager.gameStopper = EventManager.GameStopper.WIN;
                return;
            }

            levelScores[level] = score;
            resetPosition(++level);
            score = levelScores[level - 1];
            powerUp = null;
        }

        private void clearCoords()
        {
            for(int i = 0; i < recentlyHitBricks.Length; i++)
            {
                recentlyHitBricks[i] = new Coords();
            }
        }

        public void gameTick()
        {
            if (paddle.holdsBall) return;

            clearCoords();

            for (int index = 0; index < balls.Length; index++)
            {
                Ball ball = balls[index];
                if (ball == null) continue;

                // ball is falling down
                if (ball.Angle <= 0)
                {
                    if (collider.ballFallsDown(ball))
                    {
                        if (countBalls() > 1)
                        {
                            balls[index] = null;
                            continue;
                        }
                        else
                        {
                            // ball falls down, heart down or game over
                            lives--;
                            if (lives > 0)
                            {
                                resetPaddleAndBallPosition();
                                deactivatePowerUp();
                                removePowerUp();
                                eventManager.gameStopper = EventManager.GameStopper.LIFEDOWN;
                                return;
                            }
                            else
                            {
                                eventManager.gameStopper = EventManager.GameStopper.GAMEOVER;
                                playerLives = false;
                                resetPaddleAndBallPosition();
                                deactivatePowerUp();
                                removePowerUp();
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
                        recentlyHitBricks[index].row = row;
                        recentlyHitBricks[index].column = column;

                        score += brick.Score;
                        
                        // brick is destroyed
                        if (!brick.isAlive)
                        {
                            brickCount--;
                            // level up
                            if (brickCount == 0)
                            {
                                eventManager.gameStopper = EventManager.GameStopper.LEVELUP;
                                recentlyHitBricks[index].update(-1, -1);
                                levelUp();
                            }
                        }
                    }
                    collider.bounceVertically(ball);
                }
                ball.move();
            }
            if (!powerUpPresent)
            {
                powerUpAppearanceCounter--;
            }
            if (powerUpAppearanceCounter == 0)
            {
                if (!powerUpPresent) makePowerUp();
                powerUp.tick();
                eventManager.powerupState = EventManager.PowerUpState.EXISTS;
                if (powerUp.isFalling) eventManager.powerupState = EventManager.PowerUpState.FALLING;
                //check if paddle hits powerup
                if(collider.paddleHitsPowerUp(paddle, powerUp))
                {
                    activatePowerUp();
                    eventManager.powerupState = EventManager.PowerUpState.TAKEN;
                    removePowerUp();
                }
                else if(powerUp.y + powerUp.radius >= panelBounds.Bottom)
                {
                    removePowerUp();
                    eventManager.powerupState = EventManager.PowerUpState.NONE;
                }
                if (powerUpActive)
                {
                    powerUpActiveTime++;
                }
                if(powerUpActiveTime == powerUpEffectLength)
                {
                    deactivatePowerUp();
                    eventManager.powerupState = EventManager.PowerUpState.NONE;
                }
            }
        }

        private void makePowerUp()
        {
            powerUp = new PowerUp(panelBounds, this);
            powerUpPresent = true;
            eventManager.powerupState = EventManager.PowerUpState.EXISTS;
        }

        private void removePowerUp()
        {
            powerUp = null;
            powerUpPresent = false;
            powerUpAppearanceCounter = random.Next(500, 1000);
        }

        private void activatePowerUp()
        {
            if (powerUp == null) return;
            switch (powerUp.type)
            {
                case PowerUpType.FASTBALL:
                    foreach(Ball ball in balls)
                    {
                        if (ball != null) ball.changeSpeed(ball.speed + 2);
                    }
                    break;
                case PowerUpType.SLOWBALL:
                    foreach (Ball ball in balls)
                    {
                        if (ball != null) ball.changeSpeed(ball.speed - 2);
                    }
                    break;
                case PowerUpType.SUPERBALL:
                    foreach (Ball ball in balls)
                    {
                        if(ball != null) ball.radius += 8;
                    }
                    break;
                case PowerUpType.TRIPLEBALL:
                    Ball aliveBall = balls.FirstOrDefault(s => s != null);
                    int aliveBallIndex = Array.IndexOf(balls, aliveBall);
                    for (int i = 0; i < balls.Length && i < 3; i++)
                    {
                        if (balls[i] != null || aliveBallIndex == i) continue;

                        Ball newBall = balls[aliveBallIndex].copy();
                        newBall.Angle = random.NextDouble() * Math.PI;
                        balls[i] = newBall;
                    }
                    break;
            }
            powerUpActive = true;
            powerUpActiveTime = 0;
        }

        private void deactivatePowerUp()
        {
            for(int i = 0; i < balls.Length; i++)
            {
                if(balls[i] != null) balls[i].resetAttributes();
            }
            
            paddle.resetAttributes();
        }

    }
}
