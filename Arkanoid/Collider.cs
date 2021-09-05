using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public class Collider
    {
        Rectangle panelBounds;

        Game game;

        public Collider(Rectangle panel, Game game)
        {
            panelBounds = panel;
            this.game = game;
        }

        public bool ballHitsWall(Ball ball)
        {
            return (ball.x - ball.radius <= panelBounds.X + 1 && (ball.Angle > Math.PI/2 || ball.Angle < - Math.PI/2 ) 
                || (ball.x + ball.radius >= panelBounds.X + panelBounds.Width - 1 && (ball.Angle <= Math.PI/2 && ball.Angle >= -Math.PI/2)));
        }

        public void bounceHorizontally(Ball ball)
        {
            ball.Angle = Math.PI - ball.Angle;
        }

        public void bounceVertically(Ball ball)
        {
            ball.Angle = - ball.Angle;
        }

        public void bounceOnPaddle(Ball ball)
        {
            double xDist = Math.Abs(ball.x - game.paddle.x);
            double theta = Math.Acos(xDist / game.paddle.internalCurvatureRadius);
            double newAlpha = 2 * theta - ball.Angle - Math.PI;

            double minAngle = 5 * Math.PI / 180;
            if (newAlpha > 0 && newAlpha < minAngle) newAlpha = minAngle;
            else if (newAlpha > 0 && newAlpha > Math.PI - minAngle) newAlpha = Math.PI - minAngle;
            else if (newAlpha < 0 && newAlpha > -minAngle) newAlpha = -minAngle;
            else if (newAlpha < 0 && newAlpha < -Math.PI + minAngle) newAlpha = -Math.PI - minAngle;

            ball.Angle = newAlpha;
        }

        public bool ballHitsBrick(Ball ball, out int row, out int column)
        {
            row = -1;
            column = -1;

            if (ball.y - ball.radius > game.levelBrickMap.Count * game.brickHeight) return false;

            double innerSquareMinX = ball.x - ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMaxX = ball.x + ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMinY = ball.y - ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMaxY = ball.y + ball.radius * Math.Cos(Math.PI / 4);

            int minRow = ((int)(innerSquareMinY - 1) / game.brickHeight);
            if (minRow < 0) minRow = 0;
            if (minRow >= game.levelBrickMap.Count) minRow = game.levelBrickMap.Count - 1;

            int maxRow = ((int)(innerSquareMaxY + 1) / game.brickHeight);
            if (maxRow < 0) maxRow = 0;
            if (maxRow >= game.levelBrickMap.Count) maxRow = game.levelBrickMap.Count - 1;

            int minColumn = ((int)(innerSquareMinX - 1) / game.brickWidth);
            if (minColumn < 0) minColumn = 0;
            if (minColumn >= game.levelBrickMap[0].Count) minColumn = game.levelBrickMap[0].Count - 1;

            int maxColumn = ((int)(innerSquareMaxX + 1) / game.brickWidth);
            if (maxColumn < 0) minRow = 0;
            if (maxColumn >= game.levelBrickMap[0].Count) maxColumn = game.levelBrickMap[0].Count - 1;

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
                                    row = rowIterator;
                                    column = columnIterator;
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
                                    row = rowIterator;
                                    column = columnIterator;
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
                                    row = rowIterator;
                                    column = columnIterator;
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
                                    row = rowIterator;
                                    column = columnIterator;
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

        public bool ballFallsDown(Ball ball)
        {
            return ball.y + ball.radius >= panelBounds.Y+panelBounds.Height - 1;
        }

        public bool ballHitsUpperBound(Ball ball)
        {
            return ball.y - ball.radius - 1 <= 0;
        }

        public bool ballHitsPaddle(Ball ball, Paddle paddle)
        {
            return (ball.y + ball.radius + 1 >= paddle.y - paddle.Height / 2
                && ball.y < paddle.y - paddle.Height / 2
                && ball.x + ball.radius >= paddle.x - paddle.Width / 2
                && ball.x - ball.radius <= paddle.x + paddle.Width / 2);
        }

        public bool paddleHitsPowerUp(Paddle paddle, PowerUp powerUp)
        {
            return paddle.y - paddle.Height / 2 <= powerUp.y + powerUp.radius
                    && paddle.x - paddle.Width / 2 <= powerUp.x
                    && paddle.x + paddle.Width / 2 >= powerUp.x;
        }
    }
}
