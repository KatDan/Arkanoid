using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public class Ball
    {
        internal double x;
        internal double y;

        internal int speed = 3;
        internal int originalSpeed = 3;

        private double speedX;
        private double speedY;

        // angle of the ball in radians
        private double angle;

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

        internal int originalRadius;
        internal int radius;

        Random random = new Random();

        public Ball(int radius, Paddle paddle)
        {
            this.radius = radius;
            originalRadius = radius;
            this.x = paddle.x;
            this.y = paddle.y - paddle.Height/2 - radius;
            
            double angleDeg = 45 + 90*random.NextDouble();
            Angle = angleDeg * Math.PI / 180;
        }

        public Ball copy()
        {
            return (Ball)this.MemberwiseClone();
        }

        public void move()
        {
            x = x + speedX;
            y = y - speedY;
        }

        public void resize(int newRadius)
        {
            radius = newRadius;
        }

        public void resetAttributes()
        {
            radius = originalRadius;
            changeSpeed(originalSpeed);
        }

        public void changeSpeed(int newSpeed)
        {
            speed = newSpeed;
            speedX = Math.Cos(angle) * speed;
            speedY = Math.Sin(angle) * speed;
        }

    }
}
