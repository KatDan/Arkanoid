using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    /// <summary>
    /// A class that represents a ball in the game.
    /// </summary>
    public class Ball
    {
        /// <summary>
        /// The x coordinate of a ball
        /// </summary>
        internal double x;
        /// <summary>
        /// The y coordinate of a ball
        /// </summary>
        internal double y;

        /// <summary>
        /// The ball's velocity in pixels per tick.
        /// </summary>
        internal int speed = 3;
        /// <summary>
        /// The ball's original velocity.
        /// After the effect of a powerup that changes the ball's velocity,
        /// its velocity is set to the originalSpeed.
        /// </summary>
        internal int originalSpeed = 3;

        /// <summary>
        /// The horizontal component of the ball's velocity based on its angle.
        /// </summary>
        private double speedX;
        /// <summary>
        /// The vertical component of the ball's velocity based on its angle.
        /// </summary>
        private double speedY;

        /// <summary>
        /// The angle of a ball in radians in range from -Math.PI to +Math.PI.
        /// </summary>
        private double angle;
        /// <summary>
        /// The angle property.
        /// When the Angle is set, speedX and speedY are automatically updated.
        /// </summary>
        public double Angle
        {
            get => angle;
            set
            {
                angle = value;
                while (angle > Math.PI) angle -= 2*Math.PI;
                while (angle < -Math.PI) angle += 2 * Math.PI;
                speedX = Math.Cos(angle)*speed;
                speedY = Math.Sin(angle)*speed;
            }
        }

        /// <summary>
        /// A property that returns the quadrant in which the ball's angle is.
        /// The possible values are 1 to 4, starting from 0 radians and counting counterclockwise.
        /// </summary>
        public double angleQuadrant
        {
            get
            {
                if(angle > 0)
                {
                    if (angle <= Math.PI / 2) return 1;
                    else return 2;
                }
                else
                {
                    if (angle >= - Math.PI / 2) return 4;
                    else return 3;
                }
            }
        }

        /// <summary>
        /// The ball's radius in pixels.
        /// </summary>
        internal int radius;
        /// <summary>
        /// The ball's original radius in pixels.
        /// After modifying the radius with a pwoerUp, this attribute 
        /// is used for setting the original radius to the ball.
        /// </summary>
        internal int originalRadius;

        /// <summary>
        /// System.Random attribute. It is used for randomly selecting the angle
        /// of a ball at the start of the level.
        /// </summary>
        Random random = new Random();

        /// <summary>
        /// Constructor of the Ball class.
        /// The angle is chosen randomly from 45 to 135 degrees.
        /// </summary>
        /// <param name="radius"> radius of the ball in pixels. </param>
        /// <param name="paddle"> Arkanoid.Paddle instance for which the ball is generated.</param>
        public Ball(int radius, Paddle paddle)
        {
            this.radius = radius;
            originalRadius = radius;
            this.x = paddle.x;
            this.y = paddle.y - paddle.Height/2 - radius;
            
            double angleDeg = 45 + 90*random.NextDouble();
            Angle = angleDeg * Math.PI / 180;
        }

        /// <summary>
        /// Returns a shallow copy of this instance.
        /// </summary>
        /// <returns>A shallow copy of this instance.</returns>
        public Ball copy()
        {
            return (Ball)this.MemberwiseClone();
        }

        /// <summary>
        /// Moves a ball to the new position based on its speed.
        /// </summary>
        public void move()
        {
            x = x + speedX;
            y = y - speedY;
        }

        /// <summary>
        /// Sets the ball's radius.
        /// </summary>
        /// <param name="newRadius"></param>
        public void resize(int newRadius)
        {
            radius = newRadius;
        }

        /// <summary>
        /// Sets the velocity and radius of the ball to its original state.
        /// </summary>
        public void resetAttributes()
        {
            radius = originalRadius;
            changeSpeed(originalSpeed);
        }

        /// <summary>
        /// Changes the ball's velocity and recomputes speedX and speedY.
        /// </summary>
        /// <param name="newSpeed"></param>
        public void changeSpeed(int newSpeed)
        {
            speed = newSpeed;
            speedX = Math.Cos(angle) * speed;
            speedY = Math.Sin(angle) * speed;
        }

    }
}
