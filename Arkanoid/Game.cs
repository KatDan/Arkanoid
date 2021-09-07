using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    /// <summary>
    /// A class that controls the behavior of objects in the game and simulates the game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Arkanoid.EventManager instance that is used for communication between the UI and this class.
        /// </summary>
        internal EventManager eventManager = new EventManager();

        /// <summary>
        /// System.Drawing.Rectangle representing the bounds of the window where the game is visualized.
        /// </summary>
        Rectangle panelBounds;

        /// <summary>
        /// Maximum number of levels in the game.
        /// </summary>
        int maxNumberOfLevels = 3;

        /// <summary>
        /// Array of scores saved after each level.
        /// </summary>
        int[] levelScores;

        /// <summary>
        /// A flag whether the player has any life left or not.
        /// </summary>
        internal bool playerLives = true;

        /// <summary>
        /// The default number of bricks per line. When brick map that has different number of columns,
        /// this attribute is updated.
        /// </summary>
        internal int bricksPerLine = 8;

        /// <summary>
        /// Maximum number of lives that player has at the start of the game.
        /// </summary>
        internal int maxLives = 3;
        /// <summary>
        /// Current number of lives the player has.
        /// </summary>
        internal int lives = 3;

        /// <summary>
        /// Arkanoid.Paddle instance that the player controls.
        /// </summary>
        internal Paddle paddle;

        /// <summary>
        /// Array of Arkanoid.Ball instances of the balls currently existing in the game.
        /// </summary>
        internal Ball[] balls;
        /// <summary>
        /// Maximum number of balls permitted to be in the game simultaneously. 
        /// </summary>
        internal int maxBallsCount = 3;

        /// <summary>
        /// Instance of Arkanoid.PowerUp that represents power up.
        /// </summary>
        internal PowerUp powerUp;
        /// <summary>
        /// A flag whether the power up is visible or not.
        /// </summary>
        internal bool powerUpPresent = false;
        /// <summary>
        /// System.Random instance used for randomly choosing the angle of cloned balls after hitting Tripple ball power up.
        /// </summary>
        Random random;
        /// <summary>
        /// Counter of ticks until the new power up appears. When it hits 0, new power up is created.
        /// </summary>
        private int powerUpAppearanceCounter;

        /// <summary>
        /// Number of ticks that taken power up affects the balls or the paddle after catching it.
        /// </summary>
        private int powerUpEffectLength = 500;
        /// <summary>
        /// A flag that whether the powe rup is taken by the player or not.
        /// </summary>
        private bool powerUpActive = false;
        /// <summary>
        /// Number of ticks since the power up has been activated.
        /// </summary>
        private int powerUpActiveTime = 0;

        /// <summary>
        /// Score of the player.
        /// </summary>
        internal int score = 0;
        /// <summary>
        /// Current level of the game.
        /// </summary>
        internal int level = 1;
        /// <summary>
        /// 2D list of Arkanoid.Brick instances that need to be destroyed in the current level.
        /// </summary>
        internal List<List<Brick>> levelBrickMap;
        /// <summary>
        /// Array of bricks hit by each of the existing balls most recently.
        /// </summary>
        internal Coords[] recentlyHitBricks;

        /// <summary>
        /// Number of bricks needed to be destroyed before level up.
        /// </summary>
        private int brickCount = 0;

        /// <summary>
        /// Arkanoid.Collider instance that deals with the objects' collisions and manages them.
        /// </summary>
        private Collider collider;

        /// <summary>
        /// Width of the bricks that depends on the number of bricks per line.
        /// </summary>
        public int brickWidth
        {
            get { return (int)((panelBounds.Width) / bricksPerLine); }
        }
        /// <summary>
        /// Fixed height of each brick.
        /// </summary>
        public int brickHeight = 20;

        /// <summary>
        /// A constructor that sets the initial position of objects at the very start of the game.
        /// </summary>
        /// <param name="bounds">System.Drawing.Rectangle that represents the bounds of panel used for he UI.</param>
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

        /// <summary>
        /// Initializes the first level, its brick map and the position of the paddle and the ball.
        /// </summary>
        private void initializeGame()
        {
            List<List<int>> brickMapData = loadLevelBrickInfo(1);
            levelBrickMap = initializeLevelBricks(brickMapData);

            initializePaddle();
            initializeBall();
        }

        /// <summary>
        /// Loads 2D array of thicknesses of the bricks in the current level from the file.
        /// </summary>
        /// <param name="level">Current level of the game. The data is loaded from file "level"+level+".csv".</param>
        /// <exception cref="LevelFormatException">
        /// The exception is thrown when the file the brick maps are loaded from is in wrong format.
        /// </exception>
        /// <returns>2D list of thicknesses of the bricks in the current level.</returns>
        private List<List<int>> loadLevelBrickInfo(int level)
        {
            List<List<int>> result = new List<List<int>>();
            string filename = "Arkanoid.level" + level.ToString()+".csv";
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(filename)))
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

        /// <summary>
        /// Creates 2D list of Arkanoid.Brick instances from the 2D list of their thicknesses.
        /// </summary>
        /// <param name="brickMap">2D array of bricks' thicknesses loaded from a file.</param>
        /// <returns>2D list of Arkanoid.Brick instances that represent the level's brick map.</returns>
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
    
        /// <summary>
        /// Creates new Arkanoid.Paddle instance.
        /// </summary>
        private void initializePaddle()
        {
            int paddleWidth = 100;
            int paddleHeight = 20;
            paddle = new Paddle(panelBounds.Width/2,
                panelBounds.Height - 20,
                paddleWidth, paddleHeight);
        }

        /// <summary>
        /// Creates new Arkanoid.Ball instance.
        /// </summary>
        private void initializeBall()
        {
            int ballRadius = 9;
            balls[0] = new Ball(ballRadius,paddle);
            paddle.holdsBall = true;
        }

        /// <summary>
        /// Sets the starting position of desired level, including brick map, paddle and the ball.
        /// </summary>
        /// <param name="level">level that is needed to be set.</param>
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

        /// <summary>
        /// Sets the position of the paddle and the ball at the start of each level.
        /// </summary>
        public void resetPaddleAndBallPosition()
        {
            removeRedundantBalls();

            paddle.x = panelBounds.X + panelBounds.Width / 2;
            balls[0].x = panelBounds.X + panelBounds.Width / 2;
            balls[0].y = paddle.y - paddle.Height / 2 - balls[0].radius;

            paddle.holdsBall = true;
        }

        /// <summary>
        /// Removes the balls that appeared because of power up and moves the remaining ball
        /// to the beginning of the array of balls.
        /// </summary>
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

        /// <summary>
        /// Counts the currently existing balls.
        /// </summary>
        /// <returns>Number of the balls that the player plays with.</returns>
        private int countBalls()
        {
            int count = 0;
            for(int i = 0; i < balls.Length; i++)
            {
                if (balls[i] != null) count++;
            }
            return count;
        }

        /// <summary>
        /// Sets the new game from level 1 after game over.
        /// </summary>
        public void resetGame()
        {
            resetPosition(1);
            score = 0;
            playerLives = true;
            lives = maxLives;
            level = 1;
        }

        /// <summary>
        /// Levels up, sets the new brick map, saves the score and sets the initial 
        /// position of the objects at the start of the level.
        /// </summary>
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

        /// <summary>
        /// Clears the array of coordinates of the recently hit bricks.
        /// </summary>
        private void clearCoords()
        {
            for(int i = 0; i < recentlyHitBricks.Length; i++)
            {
                recentlyHitBricks[i] = new Coords();
            }
        }

        /// <summary>
        /// Simulates one tick in the game.
        /// </summary>
        public void gameTick()
        {
            if (paddle.holdsBall) return;

            clearCoords();

            // iterating through each possibly existing ball in the game
            for (int index = 0; index < balls.Length; index++)
            {
                Ball ball = balls[index];
                if (ball == null) continue;

                // ball is falling down
                if (ball.Angle <= 0)
                {
                    if (collider.ballFallsDown(ball))
                    {
                        // if there are any other balls left, remove this ball
                        if (countBalls() > 1)
                        {
                            balls[index] = null;
                            continue;
                        }
                        // ball falls down, heart down or game over
                        else
                        {
                            lives--;
                            // heart down
                            if (lives > 0)
                            {
                                resetPaddleAndBallPosition();
                                deactivatePowerUp();
                                removePowerUp();
                                eventManager.gameStopper = EventManager.GameStopper.LIFEDOWN;
                                return;
                            }
                            //game over
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
                    // if ball hits paddle
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

                // checking for ball hitting the wall or the bricks
                if (collider.ballHitsWall(ball)) collider.bounceHorizontally(ball);
                if (collider.ballHitsBrick(ball, out int row, out int column))
                {
                    // hit of the brick!
                    if (row == -1 || column == -1) break;
                    Brick brick = levelBrickMap[row][column];
                    if (brick != null && brick.isAlive)
                    {
                        brick.hit();
                        eventManager.brickHit = true;
                        //updating the ball's recently hit brick in the array
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
            // dealing with power ups

            // if power up is not rpesent, count down another tick
            if (!powerUpPresent)
            {
                powerUpAppearanceCounter--;
            }
            // if powe rup is supposed to be visible
            if (powerUpAppearanceCounter == 0)
            {
                // create new power up if it doesn't already exist
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
                // check if power up falls down behind the paddle
                else if(powerUp.y + powerUp.radius >= panelBounds.Bottom)
                {
                    removePowerUp();
                    eventManager.powerupState = EventManager.PowerUpState.NONE;
                }
                // count another tick if the power up is active
                if (powerUpActive)
                {
                    powerUpActiveTime++;
                }
                // the time's up and power up effects no longer affect the balls or the paddle
                if(powerUpActiveTime == powerUpEffectLength)
                {
                    deactivatePowerUp();
                    eventManager.powerupState = EventManager.PowerUpState.NONE;
                }
            }
        }

        /// <summary>
        /// Creates Arkanoid.PowerUp instance when a power up appears.
        /// </summary>
        private void makePowerUp()
        {
            powerUp = new PowerUp(panelBounds, this);
            powerUpPresent = true;
            eventManager.powerupState = EventManager.PowerUpState.EXISTS;
        }

        /// <summary>
        /// Removes current power up and sets the number of ticks until the next power up appears.
        /// </summary>
        private void removePowerUp()
        {
            powerUp = null;
            powerUpPresent = false;
            powerUpAppearanceCounter = random.Next(500, 1000);
        }

        /// <summary>
        /// Activates special behavior of the balls or the paddle based on the type of caught power up.
        /// </summary>
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

        /// <summary>
        /// Deactivates special behavior of the balls or the paddle after the power up's expiration. 
        /// </summary>
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
