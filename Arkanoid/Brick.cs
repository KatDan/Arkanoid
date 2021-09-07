using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    /// <summary>
    /// A class that represents a hittable brick in the level.
    /// </summary>
    public class Brick
    {
        /// <summary>
        /// The thickness of a brick. The higher the number,
        /// the more hits is needed for brick to be destroyed.
        /// </summary>
        internal int thickness;

        /// <summary>
        /// Number of times the brick was hit so far.
        /// When hits==thickness, the brick is destroyed.
        /// </summary>
        private int hits;
        /// <summary>
        /// A property for retrieving the number of hits of a brick.
        /// </summary>
        public int Hits { get => hits; }

        /// <summary>
        /// The number of points the player receives for hitting the brick.
        /// </summary>
        private int score;

        /// <summary>
        /// A property for retrieving the score of the brick.
        /// </summary>
        public int Score { get => score; }

        /// <summary>
        /// A flag that is true if hits is less than thickness;
        /// and false if hits == thickness, thus the brick is destroyed.
        /// </summary>
        internal bool isAlive = true;

        /// <summary>
        /// A constructor that creates a brick.
        /// </summary>
        /// <param name="thickness"> the thickness of a brick, by default 1.</param>
        public Brick(int thickness = 1)
        {
            this.thickness = thickness;
            score = 10 * thickness;
            hits = 0;
        }

        /// <summary>
        /// Updates the attributes of a Brick instance when it is hit by a ball.
        /// The number of hits is incrmeented by 1 and if the brick is destroyed,
        /// flag isAlive is updated.
        /// </summary>
        public void hit()
        {
            hits += 1;
            if(hits >= thickness)
            {
                isAlive = false;
            }
        }
    }
}
