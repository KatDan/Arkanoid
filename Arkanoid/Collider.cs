using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    public class Collider
    {
        GameForm form;

        public Collider(GameForm form)
        {
            this.form = form;
        }

        public bool ballHitsWall(Ball ball)
        {
            return (ball.X - ball.radius <= 1 && (ball.Angle > Math.PI/2 || ball.Angle < - Math.PI/2 ) 
                || (ball.X + ball.radius >= form.panel.Width - 1 && (ball.Angle <= Math.PI/2 && ball.Angle >= -Math.PI/2)));
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
            double xDist = Math.Abs(ball.X - form.paddle.X);
            double theta = Math.Acos(xDist / form.paddle.internalCurvatureRadius);
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

            if (ball.Y - ball.radius > form.brickUpperIndentation + form.levelBrickMap.Count * form.brickHeight) return false;

            double innerSquareMinX = ball.X - ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMaxX = ball.X + ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMinY = ball.Y - ball.radius * Math.Cos(Math.PI / 4);
            double innerSquareMaxY = ball.Y + ball.radius * Math.Cos(Math.PI / 4);

            int minRow = ((int)(innerSquareMinY - form.brickUpperIndentation - 1) / form.brickHeight);
            if (minRow < 0) minRow = 0;
            if (minRow >= form.levelBrickMap.Count) minRow = form.levelBrickMap.Count - 1;

            int maxRow = ((int)(innerSquareMaxY - form.brickUpperIndentation + 1) / form.brickHeight);
            if (maxRow < 0) maxRow = 0;
            if (maxRow >= form.levelBrickMap.Count) maxRow = form.levelBrickMap.Count - 1;

            int minColumn = ((int)(innerSquareMinX - 1) / form.brickWidth);
            if (minColumn < 0) minColumn = 0;
            if (minColumn >= form.levelBrickMap[0].Count) minColumn = form.levelBrickMap[0].Count - 1;

            int maxColumn = ((int)(innerSquareMaxX + 1) / form.brickWidth);
            if (maxColumn < 0) minRow = 0;
            if (maxColumn >= form.levelBrickMap[0].Count) maxColumn = form.levelBrickMap[0].Count - 1;

            switch (ball.angleQuadrant)
            {
                case 1:
                    {
                        // first alive brick in left-right down-up direction
                        for(int rowIterator = maxRow; rowIterator >= minRow; rowIterator--)
                        {
                            for(int columnIterator = minColumn; columnIterator <= maxColumn; columnIterator++)
                            {
                                Brick brick = form.levelBrickMap[rowIterator][columnIterator];
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
                                Brick brick = form.levelBrickMap[rowIterator][columnIterator];
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
                                Brick brick = form.levelBrickMap[rowIterator][columnIterator];
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
                                Brick brick = form.levelBrickMap[rowIterator][columnIterator];
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
            return ball.Y + ball.radius >= form.Height - 18 - 1;
        }

        public bool ballHitsUpperBound(Ball ball)
        {
            return ball.Y - ball.radius - 1 <= form.panel.Location.Y;
        }

        public bool ballHitsPaddle(Ball ball)
        {
            return (ball.Y + ball.radius + 1 >= form.paddle.Y - form.paddle.Height / 2
                && ball.Y < form.paddle.Y - form.paddle.Height / 2
                && ball.X + ball.radius >= form.paddle.X - form.paddle.Width / 2
                && ball.X - ball.radius <= form.paddle.X + form.paddle.Width / 2);
        }
    }
}
