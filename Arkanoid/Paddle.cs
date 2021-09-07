using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    /// <summary>
    /// A class that represents the paddle in the game.
    /// </summary>
    public class Paddle
    {
        /// <summary>
        /// The x coordinate of the center of the paddle
        /// </summary>
        internal int x;

        /// <summary>
        /// The y coordinate of the center of the paddle
        /// </summary>
        internal int y;

        /// <summary>
        /// A flag that determines whether the ball is still
        /// on the paddle and needs to be ejected, or not.
        /// At the start of the level, the flag is set to true.
        /// </summary>
        internal bool holdsBall;

        /// <summary>
        /// Height of the paddle in pixels.
        /// </summary>
        private int height;
        /// <summary>
        /// Width of the paddle in pixels.
        /// </summary>
        private int width;
        /// <summary>
        /// The default width of the paddle.
        /// </summary>
        private int originalWidth;


        /// <summary>
        /// Speed of the paddle.
        /// </summary>
        internal int speed = 10;

        /// <summary>
        /// A property that returns height of the paddle.
        /// </summary>
        public int Height { get => height; }
        /// <summary>
        /// A property that returns width of the paddle.
        /// </summary>
        public int Width { get => width; }

        /// <summary>
        /// Radius of an imaginary circle that is used as a surface
        /// for calculating the angle of reflection of the ball hitting 
        /// the paddle.
        /// </summary>
        internal int internalCurvatureRadius;

        /// <summary>
        /// Constructor that sets the default position and size of the paddle.
        /// </summary>
        /// <param name="centerX"> The x coordinate of the center of the paddle.</param>
        /// <param name="centerY"> The y coordinate of the center of the paddle.</param>
        /// <param name="width"> Width of the paddle. </param>
        /// <param name="height"> Height of the paddle. </param>
        public Paddle(int centerX, int centerY, int width, int height) {
            this.height = height;
            this.width = width;
            originalWidth = width;
            x = centerX;
            y = centerY;
            internalCurvatureRadius = 3*width;
        }

        /// <summary>
        /// Changes width of the paddle.
        /// </summary>
        /// <param name="newWidth"> new width of the paddle in pixels. </param>
        public void resize(int newWidth)
        {
            width = newWidth;
        }

        /// <summary>
        /// Sets width of the paddle to its default value.
        /// </summary>
        public void resetAttributes()
        {
            width = originalWidth;
        }

    }
}
