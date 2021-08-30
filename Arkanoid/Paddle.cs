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
        internal int x;

        // the y coordinate of the center of the paddle
        internal int y;

        internal bool holdsBall;

        private int height;
        private int width;

        internal int speed = 10;

        public int Height { get => height; }
        public int Width { get => width; }

        internal int internalCurvatureRadius;

        public Paddle(int centerX, int centerY, int width, int height) {
            this.height = height;
            this.width = width;
            x = centerX;
            y = centerY;
            internalCurvatureRadius = 3*width;
        }

        public void resize(int newWidth)
        {
            width = newWidth;
        }

    }
}
