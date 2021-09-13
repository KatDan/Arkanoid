using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    /// <summary>
    /// A class that detects collisions of the objects
    /// and modifies them when needed according to the
    /// collission type.
    /// </summary>
    public class Collider
    {
        /// <summary>
        /// Rectangle in which the game is played.
        /// </summary>
        Rectangle panelBounds;

        /// <summary>
        /// An instance of System.Game that calls the collider. 
        /// </summary>
        Game game;

        /// <summary>
        /// Constructor for Collider.
        /// </summary>
        /// <param name="panel"> System.Drawing.Rectangl in which the game is played. </param>
        /// <param name="game"> Arkanoid.Game that calls the collider.</param>
        public Collider(Rectangle panel, Game game)
        {
            panelBounds = panel;
            this.game = game;
        }

        /// <summary>
        /// Checks whether the ball hits wall at the moment.
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance that might potentially hit the wall. </param>
        /// <returns> true if the ball hits wall, false otherwise. </returns>
        public bool ballHitsWall(Ball ball)
        {
            return (ball.x - ball.radius <= panelBounds.X + 1 && (ball.Angle > Math.PI/2 || ball.Angle < - Math.PI/2 ) 
                || (ball.x + ball.radius >= panelBounds.X + panelBounds.Width - 1 && (ball.Angle <= Math.PI/2 && ball.Angle >= -Math.PI/2)));
        }

        /// <summary>
        /// Changes the attributes of the ball after bouncing on a horizontal surface ( \/ ).
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance that bounces. </param>
        public void bounceHorizontally(Ball ball)
        {
            ball.Angle = Math.PI - ball.Angle;
        }

        /// <summary>
        /// Changes the attributes of the ball after bouncing on a vertical surface ( > ).
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance that bounces. </param>
        public void bounceVertically(Ball ball)
        {
            ball.Angle = - ball.Angle;
        }

        /// <summary>
        /// Changes the attributes of the ball after bouncing on the paddle.
        /// The surface in computation is an imaginary circle, which center is located under
        /// the center of the paddle.
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance that bounces. </param>
        public void bounceOnPaddle(Ball ball)
        {
            // horizontal distance between the ball's center and center of the circle
            double xDist = Math.Abs(ball.x - game.paddle.x);

            // the angle between the ball's center, the circle's center and a horizontal line
            // that intersects the circle's center
            double theta = Math.Asin(xDist / game.paddle.internalCurvatureRadius);
            if (ball.x - game.paddle.x <= 0) theta += Math.PI / 2;
            else theta = Math.PI / 2 - theta;

            // New angle of the ball after bouncing
            double newAlpha = 2 * theta - ball.Angle - Math.PI;

            // if the angle is too small, it is set to minAngle (depends on the quadrant)
            double minAngle = 5 * Math.PI / 180;
            if (newAlpha > 0 && newAlpha < minAngle) newAlpha = minAngle;
            else if (newAlpha > 0 && newAlpha > Math.PI - minAngle) newAlpha = Math.PI - minAngle;
            else if (newAlpha < 0 && newAlpha > -minAngle) newAlpha = -minAngle;
            else if (newAlpha < 0 && newAlpha < -Math.PI + minAngle) newAlpha = -Math.PI - minAngle;

            ball.Angle = newAlpha;
        }

        /// <summary>
        /// Checks whether the ball hits any brick.
        /// If ball intersects more than just one brick,
        /// the first brick from the ball's direction is chosen as hit.
        /// 
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance.</param>
        /// <param name="row"> row of the hit brick in the brick map.
        /// If any brick isn't hit, it is set to -1.
        /// </param>
        /// <param name="column">column of the hit brick in the brick map.
        /// If any brick isn't hit, it is set to -1.
        /// </param>
        /// <param name="isFrontHit"> bool whether the ball hit the side of the brick or its back/front. </param>
        /// <returns>true, if ball hits any brick, false otherwise.</returns>
        public bool ballHitsBrick(Ball ball, out int row, out int column, out bool isFrontHit)
        {
            row = -1;
            column = -1;
            isFrontHit = true;

            // if upper bound of the ball is lower than the lowest bricks, returns false
            if (ball.y - ball.radius > game.levelBrickMap.Count * game.brickHeight) return false;

            // the bounds of the square box around the ball that determine the ball's vertical
            // and horizontal borders.
            double innerSquareMinX = ball.x - ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMaxX = ball.x + ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMinY = ball.y - ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMaxY = ball.y + ball.radius * Math.Cos(Math.PI / 4);

            // the highest row that the ball might intersect
            int minRow = ((int)(innerSquareMinY - 1) / game.brickHeight);
            if (minRow < 0) minRow = 0;
            if (minRow >= game.levelBrickMap.Count) minRow = game.levelBrickMap.Count - 1;

            // the lowest row that the ball might intersect
            int maxRow = ((int)(innerSquareMaxY + 1) / game.brickHeight);
            if (maxRow < 0) maxRow = 0;
            if (maxRow >= game.levelBrickMap.Count) maxRow = game.levelBrickMap.Count - 1;

            // the lowest column that the ball might intersect
            int minColumn = ((int)(innerSquareMinX - 1) / game.brickWidth);
            if (minColumn < 0) minColumn = 0;
            if (minColumn >= game.levelBrickMap[0].Count) minColumn = game.levelBrickMap[0].Count - 1;

            // the highest column that the ball might intersect
            int maxColumn = ((int)(innerSquareMaxX + 1) / game.brickWidth);
            if (maxColumn < 0) minRow = 0;
            if (maxColumn >= game.levelBrickMap[0].Count) maxColumn = game.levelBrickMap[0].Count - 1;

            // chooses the nearest hit brick according to the direction of the ball, i.e. the
            // quadrant of the ball's angle
            switch (ball.angleQuadrant)
            {
                case 1:
                    {
                        // first alive brick in left-right down-up direction
                        for(int rowIterator = maxRow; rowIterator >= minRow; rowIterator--)
                        {
                            for(int columnIterator = minColumn; columnIterator <= maxColumn; columnIterator++)
                            {
                                Brick brick = game.levelBrickMap[rowIterator][columnIterator];
                                if(brick != null && brick.isAlive)
                                {
                                    // brick is hit!
                                    row = rowIterator;
                                    column = columnIterator;
                                    isFrontHit = innerSquareMaxX - column * game.brickWidth >= (row + 1) * game.brickHeight - innerSquareMinY;

                                    return true;
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        // first alive brick in right-left down-up direction
                        for (int rowIterator = maxRow; rowIterator >= minRow; rowIterator--)
                        {
                            for (int columnIterator = maxColumn; columnIterator >= minColumn; columnIterator--)
                            {
                                Brick brick = game.levelBrickMap[rowIterator][columnIterator];
                                if (brick != null && brick.isAlive)
                                {
                                    // brick is hit!
                                    row = rowIterator;
                                    column = columnIterator;
                                    isFrontHit = (column+1) * game.brickWidth - innerSquareMinX  >= (row + 1) * game.brickHeight - innerSquareMinY;
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        // first alive brick in right-left up-down direction
                        for (int rowIterator = minRow; rowIterator <= maxRow; rowIterator++)
                        {
                            for (int columnIterator = maxColumn; columnIterator >= minColumn; columnIterator--)
                            {
                                Brick brick = game.levelBrickMap[rowIterator][columnIterator];
                                if (brick != null && brick.isAlive)
                                {
                                    // brick is hit!
                                    row = rowIterator;
                                    column = columnIterator;
                                    isFrontHit = (column+1) * game.brickWidth - innerSquareMinX >= innerSquareMaxY - row * game.brickHeight;
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        // first alive brick in left-right up-down direction
                        for (int rowIterator = minRow; rowIterator <= maxRow; rowIterator++)
                        {
                            for (int columnIterator = minColumn; columnIterator <= maxColumn; columnIterator++)
                            {
                                Brick brick = game.levelBrickMap[rowIterator][columnIterator];
                                if (brick != null && brick.isAlive)
                                {
                                    // brick is hit!
                                    row = rowIterator;
                                    column = columnIterator;
                                    isFrontHit = innerSquareMaxX - column * game.brickWidth >= innerSquareMaxY - row * game.brickHeight;
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                default: break;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the brick was hit from its side or from its front
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance.</param>
        /// <param name="row"> row of the brick. </param>
        /// <param name="column"> column of the brick. </param>
        /// <param name="game"> Arkanoid.Game instance containing brick size. </param>
        /// <returns> true if brick was hit from the front, false otherwise. </returns>
        private bool isBallBrickHitFront(Ball ball, int row, int column, Game game)
        {
            int verticalCrossLength = 0;
            int horizontalCrossLength = 0;
            //ball is falling down
            if(ball.angleQuadrant >= 3)
            {
                verticalCrossLength = (int)(ball.y + ball.radius - row * game.brickHeight);
            }
            //ball is going up
            else
            {
                verticalCrossLength = (int)((row + 1) * game.brickHeight - (ball.y - ball.radius));
            }

            // ball is going to the left
            if(ball.angleQuadrant == 2 || ball.angleQuadrant == 3)
            {
                verticalCrossLength = (int)((column + 1) * game.brickWidth - (ball.x - ball.radius));
            }
            //ball is going to the right
            else
            {
                verticalCrossLength = (int)(ball.x + ball.radius - column * game.brickWidth);
            }

            return verticalCrossLength >= horizontalCrossLength;
        }

        /// <summary>
        /// Checks whether the ball is falling down behind the paddle into the void.
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance that is falling down.</param>
        /// <returns> true if the ball is under the paddle, false otherwise. </returns>
        public bool ballFallsDown(Ball ball)
        {
            return ball.y + ball.radius >= panelBounds.Y+panelBounds.Height - 1;
        }

        /// <summary>
        /// Checks whether the ball hits the upper bound of a rectangle in which the game is played.
        /// </summary>
        /// <param name="ball">Arkanoid.Ball instance. </param>
        /// <returns>true if the ball hits the upper bound of the rectangle, false otherwise.</returns>
        public bool ballHitsUpperBound(Ball ball)
        {
            return ball.y - ball.radius - 1 <= 0;
        }

        /// <summary>
        /// Checks whether the ball hits paddle.
        /// </summary>
        /// <param name="ball"> Arkanoid.Ball instance.</param>
        /// <param name="paddle">Arkanoid.Paddle instance. </param>
        /// <returns> true if the ball hits the paddle, false otherwise.</returns>
        public bool ballHitsPaddle(Ball ball, Paddle paddle)
        {
            return (ball.y + ball.radius + 1 >= paddle.y - paddle.Height / 2
                && ball.y < paddle.y - paddle.Height / 2
                && ball.x + ball.radius >= paddle.x - paddle.Width / 2
                && ball.x - ball.radius <= paddle.x + paddle.Width / 2);
        }

        /// <summary>
        /// Checks whether the paddle hits falling power up.
        /// </summary>
        /// <param name="paddle"> Arkanoid.Paddle instance.</param>
        /// <param name="powerUp"> Arkanoid.PowerUp instance.</param>
        /// <returns>true if the paddle catches power up, false otherwise.</returns>
        public bool paddleHitsPowerUp(Paddle paddle, PowerUp powerUp)
        {
            return paddle.y - paddle.Height / 2 <= powerUp.y + powerUp.radius
                    && paddle.x - paddle.Width / 2 <= powerUp.x
                    && paddle.x + paddle.Width / 2 >= powerUp.x;
        }
    }
}
