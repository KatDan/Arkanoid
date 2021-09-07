using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    /// <summary>
    /// Contains all supported power up types.
    /// </summary>
    public enum PowerUpType
    {
        NONE,
        SUPERBALL,
        TRIPLEBALL,
        FASTBALL,
        SLOWBALL
    }

    /// <summary>
    /// A class representing a power up appearing in the game.
    /// </summary>
    public class PowerUp
    {
        /// <summary>
        /// Radius of the power up's icon in pixels.
        /// </summary>
        internal int radius = 15;

        /// <summary>
        /// The x coordinate of the centre of the icon.
        /// </summary>
        internal int x;
        /// <summary>
        /// The y coordinate of the centre of the icon.
        /// </summary>
        internal int y;

        /// <summary>
        /// Speed of the icon while falling down.
        /// </summary>
        internal int speed = 1;

        /// <summary>
        /// Type of the power up.
        /// </summary>
        internal PowerUpType type;

        /// <summary>
        /// Time of power up staying in its position before falling down
        /// in ticks of the timer.
        /// </summary>
        internal int sleepTime = 500;

        /// <summary>
        /// A flag that determines whether the power up is visible but
        /// not moving or whether it is falling down.
        /// </summary>
        internal bool isFalling = false;

        /// <summary>
        /// Instance of System.Random used for randomly generating starting position
        /// of the power up.
        /// </summary>
        Random random = new Random();

        /// <summary>
        /// A constructor that randomly generates a starting position of a power up and
        /// its type.
        /// </summary>
        /// <param name="panel"> Rectangle in which the game is played.</param>
        /// <param name="game"> An instance of the Game that creates power up instances.</param>
        public PowerUp(Rectangle panel, Game game) {
            x = random.Next(panel.Location.X + radius, panel.Location.X + panel.Width - radius);

            Ball aliveBall = game.balls.FirstOrDefault(s => s != null);
            y = random.Next(game.brickHeight*game.levelBrickMap.Count + radius, game.paddle.y - game.paddle.Height - 3 * aliveBall.radius - radius);
            type = randomlySelectType();
        }

        /// <summary>
        /// Randomly selects a type of the power up.
        /// </summary>
        /// <returns>PowerUpType type of the power up.</returns>
        private PowerUpType randomlySelectType()
        {
            return (PowerUpType)random.Next(1, Enum.GetValues(typeof(PowerUpType)).Length);
        }

        /// <summary>
        /// Ticks and changes behavior of the power up according to
        /// the time passed.
        /// At start, the power up is not moving, when sleepTime reaches zero,
        /// the power up starts to fall down.
        /// </summary>
        public void tick()
        {
            if (sleepTime > 0)
            {
                sleepTime--;
                if(sleepTime == 0) isFalling = true;
            }
            else y += speed;
        }
    }
}
