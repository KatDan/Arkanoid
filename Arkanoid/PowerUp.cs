using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public enum PowerUpType
    {
        NONE,
        SUPERBALL,
        TRIPLEBALL,
        FASTBALL,
        SLOWBALL
    }

    public class PowerUp
    {
        internal int radius = 15;
        internal int x;
        internal int y;
        internal int speed = 1;
        internal PowerUpType type;

        internal int sleepTime = 500;
        internal bool isFalling = false;

        Random random = new Random();

        public PowerUp(Rectangle panel, Game game) {
            x = random.Next(panel.Location.X + radius, panel.Location.X + panel.Width - radius);

            Ball aliveBall = game.balls.FirstOrDefault(s => s != null);
            y = random.Next(game.brickHeight*game.levelBrickMap.Count + radius, game.paddle.y - game.paddle.Height - 3 * aliveBall.radius - radius);
            type = randomlySelectType();
            
        }

        private PowerUpType randomlySelectType()
        {
            //return (PowerUpType)random.Next(1, Enum.GetValues(typeof(PowerUpType)).Length);
            return PowerUpType.TRIPLEBALL;
        }

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
