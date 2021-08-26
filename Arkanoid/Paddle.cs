using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public class Paddle
    {

        // The x coordinate of the center of the paddle
        private int x;
        public int X
        {
            get => x;
            set
            {
                x = value;
                pictureBox.Location = new System.Drawing.Point(x - width / 2, y - height / 2);
            }     
        }

        // the y coordinate of the center of the paddle
        private int y;
        public int Y
        {
            get => y;
            set
            {
                y = value;
                pictureBox.Location = new System.Drawing.Point(x - width/2, y - height/2);
            }
        }

        internal bool holdsBall;

        private int height;
        private int width;

        internal int speed = 10;

        public int Height { get => height; }
        public int Width { get => width; }

        PictureBox pictureBox;

        internal int internalCurvatureRadius;

        public Paddle(PictureBox picBox) {
            pictureBox = picBox;
            height = picBox.Height;
            width = picBox.Width;
            x = picBox.Location.X + width / 2;
            y = picBox.Location.Y + height / 2;
            internalCurvatureRadius = 3*width;
        }

        public void resize(int newWidth)
        {
            width = newWidth;
            pictureBox.Location = new System.Drawing.Point(x - width / 2, y - height / 2);
        }

    }
}
