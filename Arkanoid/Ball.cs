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
        private double x;
        private double y;

        public double X
        {
            get => x;
            set
            {
                x = value;
                pictureBox.Location = new System.Drawing.Point((int)X - radius, pictureBox.Location.Y);
            }
        }

        public double Y { get => y;
            set
            {
                y = value;
                pictureBox.Location = new System.Drawing.Point(pictureBox.Location.X, (int)Y - radius);
            }
        }

        internal int speed = 3;

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
        
        internal int radius;

        Random random = new Random();

        PictureBox pictureBox;

        public Ball(PictureBox picBox)
        {
            pictureBox = picBox;
            radius = picBox.Width/2;
            x = picBox.Location.X + radius;
            y = picBox.Location.Y + radius;
            
            double angleDeg = 45 + 90*random.NextDouble();
            Angle = angleDeg * Math.PI / 180;
        }

        public void move()
        {
            x = x + speedX;
            y = y - speedY;

            pictureBox.Location = new System.Drawing.Point((int)x - radius, (int)y - radius);
        }

    }
}
