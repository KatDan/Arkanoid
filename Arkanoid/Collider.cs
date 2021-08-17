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

            if(ball.Y - ball.radius > form.brickUpperIndentation + form.levelBrickMap.Count * form.brickHeight) return false;
          
            // getting row of the brick hit with upper or lower bound of the ball according to its direction
            if(ball.angleQuadrant <= 2) row = ((int)(ball.Y - ball.radius - form.brickUpperIndentation - 1) / form.brickHeight);
            else row = ((int)(ball.Y + ball.radius - form.brickUpperIndentation + 1) / form.brickHeight);

            // getting column of the brick hit with right or left bound of the ball according to its direction
            if (ball.angleQuadrant == 1 || ball.angleQuadrant == 4) column = (int)((ball.X + ball.radius + 1) / form.brickWidth);
            else column = (int)((ball.X - ball.radius - 1) / form.brickWidth);
            
            if (row >= form.levelBrickMap.Count || row < 0 || column < 0 || column >= form.bricksPerLine) return false;

            if (form.levelBrickMap[row][column] == null || !form.levelBrickMap[row][column].isAlive) return false;
            return true;
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
                && ball.X >= form.paddle.X - form.paddle.Width / 2
                && ball.X <= form.paddle.X + form.paddle.Width / 2);
        }
    }
}
